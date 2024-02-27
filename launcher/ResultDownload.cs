using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Ntools
{
    /// <summary>
    /// Represents the result of a download operation.
    /// </summary>
    public class ResultDownload : ResultHelper
    {
        /// <summary>
        /// Gets the name of the downloaded file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the URI of the downloaded file.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the downloaded file is digitally signed.
        /// </summary>
        public bool DigitallySigned { get; private set; } = false;

        /// <summary>
        /// Gets or sets the size of the downloaded file in bytes.
        /// </summary>
        public long FileSize { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the X509 certificate associated with the downloaded file.
        /// </summary>
        public X509Certificate2 X509Certificate2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultDownload"/> class.
        /// </summary>
        /// <param name="uri">The URI of the downloaded file.</param>
        /// <param name="fileName">The name of the downloaded file.</param>
        public ResultDownload(Uri uri, string fileName)
        {
            New();
            FileName = fileName;
            Uri = uri;
        }

        /// <summary>
        /// Sets the result as success.
        /// </summary>
        public void Success()
        {
            Code = 0;
            Output.Add(SuccessMessage);
        }

        /// <summary>
        /// Sets the result as failure with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void Fail(string message)
        {
            Code = -1;
            Output.Add(message);
        }

        /// <summary>
        /// Sets the size of the downloaded file in bytes.
        /// </summary>
        public void SetFileSize()
        {
            try
            {
                FileSize = Task.Run(async () => await Nfile.GetFileSizeAsync(Uri.ToString())).Result;
            }
            catch (Exception)
            {
                FileSize = 0;
            }
        }

        /// <summary>
        /// Sets whether the downloaded file is digitally signed.
        /// </summary>
        public void SetFileSigned()
        {
            DigitallySigned = SignatureVerifier.VerifyDigitalSignature(FileName);
            if (DigitallySigned)
            {
                X509Certificate2 = new X509Certificate2(FileName);
            }
        }

        /// <summary>
        /// Displays the certificate associated with the downloaded file.
        /// </summary>
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
