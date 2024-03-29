using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Ntools
{
        /// <summary>
        /// Provides methods for verifying and extracting digital signatures from files.
        /// </summary>
    public static class SignatureVerifier
    {
        // Structure to hold WinTrustData
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WinTrustData
        {
            public uint cbStruct;
            public IntPtr pPolicyCallbackData;
            public IntPtr pSIPClientData;
            public uint dwUIChoice;
            public uint fdwRevocationChecks;
            public uint dwUnionChoice;
            public IntPtr pFile;
            public uint dwStateAction;
            public IntPtr hWVTStateData;
            public string pwszURLReference;
            public uint dwProvFlags;
            public uint dwUIContext;
            public IntPtr pSignatureSettings;
        }

        // Import the WinVerifyTrust function from wintrust.dll
        [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern int WinVerifyTrust(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
            WinTrustData pWVTData
        );

        /// <summary>
        /// Verifies the digital signature of a file.
        /// </summary>
        /// <param name="fileName">The name of the file to verify.</param>
        /// <returns>True if the digital signature is valid, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when the file name is null or empty, or when the file does not exist.</exception>
        public static bool VerifyDigitalSignature(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.", nameof(fileName));
            }

            Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");

            // Create a WINTRUST_FILE_INFO structure
            WINTRUST_FILE_INFO fileInfo = new WINTRUST_FILE_INFO
            {
                cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)),
                pcwszFilePath = fileName
            };

            // Create a WinTrustData structure
            var data = new WinTrustData
            {
                cbStruct = (uint)Marshal.SizeOf(typeof(WinTrustData)),
                dwUIChoice = 2, //WTD_UI_NONE,
                dwUnionChoice = 1, //WTD_CHOICE_FILE,
                pFile = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)))
            };

            int result = -1;
            try
            {
                // Marshal the fileInfo structure to a pointer
                Marshal.StructureToPtr(fileInfo, data.pFile, false);

                // Call the WinVerifyTrust function to verify the digital signature
                result = WinVerifyTrust(IntPtr.Zero, WINTRUST_ACTION_GENERIC_VERIFY_V2, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                // Free the allocated memory
                Marshal.FreeCoTaskMem(data.pFile);
            }

            // Return true if the digital signature is valid, false otherwise
            return result == 0;
        }


        /// <summary>
        /// Extracts the digital signature from a file and prints the details of the digital signature.
        /// </summary>
        /// <param name="fileName">The name of the file to extract the digital signature from.</param>
        public static void ExtractDigitalSignature(string fileName)
        {
            // Read the signed file into a byte array
            byte[] signedFile = File.ReadAllBytes(fileName);

            // Create a SignedCms object from the signed file
            SignedCms signedCms = new SignedCms();
            signedCms.Decode(signedFile);

            // Get the signer information
            SignerInfo signerInfo = signedCms.SignerInfos[0];

            // Get the X509 certificate
            X509Certificate2 certificate = signerInfo.Certificate;

            // Print the details of the digital signature
            Console.WriteLine("Subject: " + certificate.Subject);
            Console.WriteLine("Issuer: " + certificate.Issuer);
            Console.WriteLine("Valid From: " + certificate.NotBefore);
            Console.WriteLine("Valid To: " + certificate.NotAfter);
            Console.WriteLine("Thumbprint: " + certificate.Thumbprint);
        }


        /// <summary>
        /// Displays the properties of a Digital certificate.
        /// </summary>
        /// <param name="fileName">The name of the file to display the certificate from.</param>
        /// <exception cref="ArgumentException">Thrown when the file name is null or empty, or when the file does not exist.</exception>
        public static void DisplayCertificate(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.", nameof(fileName));
            }
            // Create an X509Certificate2 object to represent the certificate of the signed file.
            var cert = new X509Certificate2(fileName);

            // Display the properties of the certificate.
            Console.WriteLine($"Certificate Properties ");
            Console.WriteLine($"---------------------- ");
            Console.WriteLine($"SignatureAlgorithm: {cert.SignatureAlgorithm.FriendlyName}");
            Console.WriteLine($"Subject: {cert.Subject}");
            Console.WriteLine($"Issuer: {cert.Issuer}");
            Console.WriteLine($"Version: {cert.Version}");
            Console.WriteLine($"Valid From: {cert.NotBefore}");
            Console.WriteLine($"Valid To: {cert.NotAfter}");
            Console.WriteLine($"Serial Number: {cert.SerialNumber}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
        }
    }

    // Structure to hold information about a file for verification
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINTRUST_FILE_INFO
    {
        public uint cbStruct;
        public string pcwszFilePath;
        public IntPtr hFile;
        public IntPtr pgKnownSubject;
    }

    // Structure to hold WinTrustData
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINTRUST_DATA
    {
        public uint cbStruct;
        public IntPtr pPolicyCallbackData;
        public IntPtr pSIPClientData;
        public uint dwUIChoice;
        public uint fdwRevocationChecks;
        public uint dwUnionChoice;
        public IntPtr pFile;
        public uint dwStateAction;
        public IntPtr hWVTStateData;
        public string pwszURLReference;
        public uint dwProvFlags;
        public uint dwUIContext;
        public IntPtr pSignatureSettings;
    }
}