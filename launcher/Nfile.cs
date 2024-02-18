using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ntools
{
    public static class Nfile
    {
        private static List<string> AllowedExtensions { get; set; } = new List<string> ();
        private static List<string> TrustedHosts { get; set; } = new List<string> ();

        // Getter and setter for TrustedHosts
        public static List<string> GetTrustedHosts()
        {
            return TrustedHosts;
        }

        public static void SetTrustedHosts(List<string> trustedHosts)
        {
            TrustedHosts = trustedHosts;
        }

        // Gettter and Setter for AllowedExtensions
        
        public static List<string> GetAllowedExtensions()
        {
            return AllowedExtensions;
        }

        public static void SetAllowedExtensions(List<string> allowedExtensions)
        {
            AllowedExtensions = allowedExtensions;
        }

        public static async Task<ResultDownload> DownloadAsync(this HttpClient client, Uri uri, string downloadedFilename)
        {
            if (!ValidUri(uri.ToString())) throw new ArgumentException("Invalid uri", nameof(uri));

            if (!TrustedHost(uri)) throw new ArgumentException("Untrusted host", nameof(uri));

            if (!ValidExtension(uri.ToString())) throw new ArgumentException("Invalid uri extension", nameof(uri));

            if (string.IsNullOrEmpty(downloadedFilename)) throw new ArgumentException("Invalid file name.", nameof(downloadedFilename));

            if (!Path.IsPathRooted(downloadedFilename)) throw new ArgumentException("Invalid file name: Path must be rooted.", nameof(downloadedFilename));

            // Check if downloadedFilename contains invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            if (Path.GetFileName(downloadedFilename).IndexOfAny(invalidChars) >= 0) throw new ArgumentException($"Invalid file name: downloadedFilename contains one or more invalid characters {invalidChars}.", nameof(downloadedFilename));

            var resultDownload = new ResultDownload (downloadedFilename, uri);
            try
            {
                // check if uri exists
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    resultDownload.Fail($"{response}");
                }
                else
                {
                    // Set file size in the return object.  This could be used to check if size is within a range at a later release
                    resultDownload.SetFileSize();
                    if (File.Exists(downloadedFilename)) File.Delete(downloadedFilename);

                    using (var s = await client.GetStreamAsync(uri))
                    using (var fs = new FileStream(downloadedFilename, FileMode.Create))
                    {
                        await s.CopyToAsync(fs);
                    }
                    resultDownload.Success();
                }
            }
            catch (ArgumentNullException ex)
            {
                resultDownload.Fail(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                resultDownload.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                resultDownload.Fail(ex.Message);
            }
            finally
            {
                if (resultDownload.IsSuccess())
                {
                    // Check if file got downnloaed and do any cleanup here
                    if (!File.Exists(downloadedFilename))
                    {
                        resultDownload.Fail($"File {downloadedFilename} does not exist.");
                    }
                    else
                    {
                        // get downloaded file size from the local file
                        var fileInfo = new FileInfo(downloadedFilename);
                        long fileSize = fileInfo.Length;

                        if (fileSize != resultDownload.FileSize) resultDownload.Fail($"File size mismatch. Expected: {resultDownload.FileSize}, Actual: {fileSize}.");

                        // and compare with the size from the server
                        resultDownload.SetFileSize();

                        //Set digital signature and file size
                        resultDownload.SetFileSigned();
                    }
                }
            }

            return resultDownload;
        }

        // Add a method to check if a Uri exists
        public static async Task<HttpResponseMessage> UriExistsAsync(this HttpClient client, string uri)
        {
            if (!ValidUri(uri)) throw new ArgumentException("Invalid uri", nameof(uri));

            try
            {
                return await client.GetAsync(uri);
            }
            catch (HttpRequestException)
            {
                // pass through exception to caller
                throw;
            }
        }

        // Add a method to get the file size of a Uri
        public static async Task<long> GetFileSizeAsync(this HttpClient client, string uri)
        {
            if (!ValidUri(uri.ToString())) throw new ArgumentException("Invalid uri", nameof(Uri));

            try
            {
                var response = await client.GetAsync(uri);
                return response.Content.Headers.ContentLength ?? 0;
            }
            catch (HttpRequestException)
            {
                // pass through exception to caller
                throw;
            }
        }

        public static bool TrustedHost(Uri uri)
        {
            foreach (var trustedHost in TrustedHosts)
            {
                if (uri.Host == trustedHost)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidUri(string uri)
        {
            var result = ValidUriInternal(uri, out Uri uriResult);

            // Disallow traversal paths
            if (result)
            {
                result = !uriResult.AbsolutePath.Contains("..");
            }

            return result;
        }

        private static bool ValidUriInternal(string uri, out Uri uriResult)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            return Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                        && uriResult.Scheme == Uri.UriSchemeHttps;
        }

        public static bool ValidExtension(string uri)
        {
            var result = ValidUriInternal(uri, out Uri uriResult);
            if (result)
            {
                string fileExtension = Path.GetExtension(uriResult.AbsolutePath);
                result = AllowedExtensions.Contains(fileExtension);
            }

            return result;
        }

        public static Uri CreateSafeUri(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(userInput));
            }

            string encodedInput = Uri.EscapeDataString(userInput);

            Uri uri;
            if (!Uri.TryCreate(encodedInput, UriKind.Absolute, out uri))
            {
                throw new ArgumentException("Invalid URI", nameof(userInput));
            }

            if (uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Non-secure URI", nameof(userInput));
            }

            // Add your trusted hosts here
            List<string> trustedHosts = new List<string> { "trustedhost1.com", "trustedhost2.com" };
            if (!trustedHosts.Contains(uri.Host))
            {
                throw new ArgumentException("Untrusted host", nameof(userInput));
            }

            if (uri.AbsolutePath.Contains(".."))
            {
                throw new ArgumentException("Path traversal detected", nameof(userInput));
            }

            return uri;
        }
    }
}
