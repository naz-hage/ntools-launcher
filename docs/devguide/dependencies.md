## Project Dependencies
- This project depends on the developer to have a valid `VirusTotal` API key.  The key is used to check the downloaded file for virus.  The key is stored in the environment variable `VTAPIKEY `.  The key is used by the `NFile` class to check the downloaded file for virus.  Use the following command to set the environment variable:
```bash
setx VTAPIKEY "your_valid_virustotal_api_key"
```