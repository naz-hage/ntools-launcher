## Project Dependencies
- .NET 10 SDK
- YamlDotNet 18.0.0
- PowerShell and the `sdo` build tool for the documented development workflow

`Nfile.DownloadAsync` does not require a VirusTotal API key. The standalone
`VirusTotalChecker` class remains available for callers that explicitly want
to submit a file for scanning and provide their own API key.