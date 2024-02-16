using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Ntools
{
    public class ResultDownload : ResultHelper
    {
        public string FileName { get; } = string.Empty;
        public Uri Uri { get; } = new Uri("http://localhost");

        public bool DigitallySigned { get; private set; }

        public long FileSize { get; private set; }

        public X509Certificate2 X509Certificate2 { get; private set; }

        public ResultDownload(string fileName, Uri uri)
        {
            New();
            FileName = fileName;
            Uri = uri;
            DigitallySigned = false;
        }

        public void Success()
        {
            Code = 0;
            Output.Add("Success");
        }

        public void Fail(string message)
        {
            Code = -1;
            Output.Add(message);
        }

        public void SetFileSize()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                FileSize = Task.Run(async () => await httpClient.GetFileSizeAsync(Uri.ToString())).Result;
            }
            catch (Exception)
            {
                FileSize = 0;
            }
        }

        public void SetFileSigned()
        {
            DigitallySigned = SignatureVerifier.VerifyDigitalSignature(FileName);
            if (DigitallySigned)
            {
                X509Certificate2 = new X509Certificate2(FileName);
            }
        }

        public void DisplayCertificate()
        {
            if (DigitallySigned)
            {

                try
                {
                    SignatureVerifier.DisplayCertificate(FileName);
                }
                catch (CryptographicException ex)
                {
                    // display the error message
                    Console.WriteLine($"CryptographicException: {ex.Message}");
                    DigitallySigned = false;
                }
            }
            else
            {
                Console.WriteLine($"File `{FileName}` is not digitally signed.");
            }
        }
    }
}
