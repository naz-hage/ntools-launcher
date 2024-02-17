using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ntools
{
    public static class Nfile
    {
        public static async Task<ResultDownload> DownloadAsync(this HttpClient client, Uri uri, string downloadedFilename)
        {
            if (!ValidUri(uri.ToString())) throw new ArgumentException("Invalid uri", nameof(uri));

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

        public static bool ValidUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            bool result = Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult)
                        && uriResult.Scheme == Uri.UriSchemeHttps;
            return result;
        }
    }
}
