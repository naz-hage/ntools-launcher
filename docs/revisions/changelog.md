### Version 1.6.0 - 07-may-24
- [issue #14](https://github.com/naz-hage/home/issues/14) - Feature: Publish documentation using mkdocs


### Version 1.5.0 - 29-feb-24
- Add DownloadAsync method to download file from the web.
    - The method uses the VirusTotal API to check the downloaded file for virus.
    - The method returns a ResultDownload object that details the download result such as file signature, file size, and VirusTotal check.

    [Next](#next)

### Version 1.2.0 - 28-jan-24

### Version 1.1.0 - 25-jan-24
- Add lock file to prevent file being changed before it is launched.
    - If file cannot be locked, then it is not launched. An error is returned.
    - After returnning from `launch`, the file is unlocked.
- Add option to check for digital signature of file.
    - If file is not signed, then it is not launched. An error is returned.
    - Digital signature check is only performed when the file is locked.

### Version 1.0.18 - jan-24
- Initial release Nuget package release.

