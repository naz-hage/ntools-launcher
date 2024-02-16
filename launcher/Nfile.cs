using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ntools
{
    public static class Nfile
    {
        public static async Task<ResultDownload> DownloadAsync(this HttpClient client, Uri uri, string fileName)
        {
            var resultDownload = new ResultDownload (fileName, uri);
            try
            {
                if (!ValidUri(resultDownload.Uri.ToString())) throw new ArgumentException("Invalid uri", nameof(resultDownload.Uri));

                if (string.IsNullOrEmpty(resultDownload.FileName)) throw new ArgumentException("Invalid file name.", nameof(resultDownload.FileName));

                if (!Path.IsPathRooted(resultDownload.FileName)) throw new ArgumentException("Invalid file name. Must be a valid full path.", nameof(resultDownload.FileName));

                // Check if fileName contains invalid characters
                var invalidChars = Path.GetInvalidFileNameChars();

                if (Path.GetFileName(resultDownload.FileName).IndexOfAny(invalidChars) >= 0) throw new ArgumentException("Invalid file name. Contains invalid characters.", nameof(resultDownload.FileName));

                if (File.Exists(resultDownload.FileName)) File.Delete(resultDownload.FileName);

                var response = await client.GetAsync(resultDownload.Uri);
                if (!response.IsSuccessStatusCode) throw new FileNotFoundException($"'{resultDownload.Uri}' not found. status: {response.StatusCode}", nameof(resultDownload.Uri));

                using (var s = await client.GetStreamAsync(resultDownload.Uri))
                using (var fs = new FileStream(resultDownload.FileName, FileMode.Create))
                {
                    await s.CopyToAsync(fs);
                }
                resultDownload.Success();
                
            }
            catch (ArgumentNullException ex)
            {
                resultDownload.Fail(ex.Message);
            }
            catch (ArgumentException ex)
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
                    if (!File.Exists(resultDownload.FileName)) resultDownload.Fail($"File {resultDownload.FileName} does not exist.");

                    //Set digital signature and file size
                    resultDownload.SetFileSigned();
                    resultDownload.SetFileSize();
                }
            }

            return resultDownload;
        }

        // Add a method to check if a Uri exists
        public static async Task<bool> UriExistsAsync(this HttpClient client, string uri)
        {
            if (!ValidUri(uri)) throw new ArgumentException("Invalid uri", nameof(uri));

            try
            {
                var response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
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

        public static async Task<bool> UriExistsAsync(string uri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(uri);
                    return response.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    return false;
                }
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
