using System;
using System.Diagnostics;
using System.Reflection;

namespace Launcher.Services
{
    public enum AssemblyType
    {
        Entry,
        Executing
    }

    public static class Nversion
    {
        /// <summary>
        /// Retrieves the file version information for the entry or executing assembly.
        /// </summary>
        /// <param name="type">
        /// Specifies which assembly to retrieve the file version information from.
        /// <see cref="AssemblyType.Entry"/> gets the entry assembly; <see cref="AssemblyType.Executing"/> gets the currently executing assembly.
        /// </param>
        /// <returns>
        /// The formatted string containing the file description, product name, company name, legal copyright, and file version.
        /// </returns>
        public static string Get(AssemblyType type = AssemblyType.Entry)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(
                type == AssemblyType.Executing
                    ? Assembly.GetExecutingAssembly().Location
                    : (Assembly.GetEntryAssembly() != null
                        ? Assembly.GetEntryAssembly().Location
                        : Assembly.GetExecutingAssembly().Location)
            );

            string updatedCopyright = fileVersionInfo.LegalCopyright.Replace("XXXX", DateTime.Now.Year.ToString());

            return $"{fileVersionInfo.FileDescription} v{fileVersionInfo.ProductMajorPart}.{fileVersionInfo.ProductMinorPart}.{fileVersionInfo.ProductBuildPart} - {fileVersionInfo.ProductName} by {fileVersionInfo.CompanyName} ({updatedCopyright})";
        }
    }
}
