# Software Tools Collection

## Introduction
This repository contains a .NET Core class library that wraps around the .NET class `Process` to provide secure launching of executables. It provides a `ResultHelper` class that returns the code and output of the launcher executable. A new feature is added to securily download file from the wen via the NFile class.  A `ResultDownload` class is provided details the download result such as file signature, file size, and VirusTotal check.

The library is used by the [ntools's]() repo
- `Nbackup` launches `robocopy` and wait for it to complete, ensuring that the backup process is fully completed before the program continues.
- `Nbuild` launches `msbuild` to build and test .NET projects.  It also builds selected development tools for the project.  It utlilies the `NFile` class to download the tools from the web.
- `Ngit' launches `git` to perform git operations.

## Table of Contents
1. [Distribution](#Distribution)
2. [Usage](./launcher/README.md)
3. [Project Dependencies](#Project-Dependencies)
4. [Prerequisite](#Prerequisite)
5. [Build and Test](#Build-and-Test)

## Distribution
- This class library is released to nuget.org as `ntools-launcher` package. 

## Project Dependencies
- This project depens on the developer to have a valid `VirusTotal` API key.  The key is used to check the downloaded file for virus.  The key is stored in the environment variable `VTAPIKEY `.  The key is used by the `NFile` class to check the downloaded file for virus.  Use the following command to set the environment variable:
```bash
setx VTAPIKEY "your_valid_virustotal_api_key"
```

## Prerequisite
- The project is built using the `NTools` Standardized build tools.  The following command is used to install the `NTools` :
```bash
SetupDev.bat
```

## Build and Test
- The project is built and tested using the `NTools` Standardized build tools.  The following command is used to build, test and build artifacts of the project:
```bash
nb staging
```
