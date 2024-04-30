## Version 1.4.1 - 29-feb-24
- Add DownloadAsync method to download file from the web.
    - The method uses the VirusTotal API to check the downloaded file for virus.
    - The method returns a ResultDownload object that details the download result such as file signature, file size, and VirusTotal check.