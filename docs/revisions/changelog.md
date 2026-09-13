## [Latest Release](https://github.com/naz-hage/ntools-launcher/releases)

### Unreleased - September 2026
- [issue #27] Upgrade the project and test dependencies to .NET 10 / MSTest 4.
- [issue #31] Add YamlDotNet 18.0.0 and the YAML launcher foundation.
- [issue #33] Add YAML launcher configuration and result models, including `steps:`, `tasks:`, and `apps:` aliases.
- [issue #35] Add YAML loading and validation from files, strings, and streams.
- [issue #36] Add single-step execution with output capture, timeouts, environment variables, working directories, exit-code checks, and optional signature verification.
- [issue #37] Add comprehensive Phase 1 YAML launcher tests, including deterministic timeout coverage.

### Version 1.6.0 - 14-nov-24
- [issue #14](https://github.com/naz-hage/ntools-launcher/issues/14) - Feature: Publish documentation using mkdocs
- [issue #17](https://github.com/naz-hage/ntools-launcher/issues/14) - Bug:Microsoft Security Advisory CVE-2024-43485 | .NET Denial of Service Vulnerability
- [issue #21](https://github.com/naz-hage/ntools-launcher/issues/21) - Feature: Update Target Frameworks to 9.0 and Package References


### Version 1.5.0 - 29-feb-24
- Add DownloadAsync method to download file from the web.
    - The method uses the VirusTotal API to check the downloaded file for virus.
    - The method returns a ResultDownload object that details the download result such as file signature, file size, and VirusTotal check.

    

### Version 1.2.0 - 28-jan-24

### Version 1.1.0 - 25-jan-24
- Add lock file to prevent file being changed before it is launched.
    - If file cannot be locked, then it is not launched. An error is returned.
    - After returning from `launch`, the file is unlocked.
- Add option to check for digital signature of file.
    - If file is not signed, then it is not launched. An error is returned.
    - Digital signature check is only performed when the file is locked.

### Version 1.0.18 - jan-24
- Initial release Nuget package release.

