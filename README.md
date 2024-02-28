# Software Tools Collection

## Introduction
This repository contains a .NET Core class library that wraps around the .NET class `Process` to provide secure launching of executables. It provides a `ResultHelper` class that returns the code and output of the launcher executable. A new feature is added to securily download file from the wen via the NFile class.  A `ResultDownload` class is provided details the download result such as file signature, file size, and VirusTotal check.

The library is used by the [ntools's]() repo
- `Nbackup` launches `robocopy` and wait for it to complete, ensuring that the backup process is fully completed before the program continues.
- `Nbuild` launches `msbuild` to build and test .NET projects.  It also builds selected development tools for the project.  It utlilies the `NFile` class to download the tools from the web.
- `Ngit' launches `git` to perform git operations.

## Table of Contents
1. [Project Dependencies](#Project-Dependencies)
3. [Development Environment Setup](#Development-Environment-Setup)
3. [Build and Test](#Build-and-Test)
4. [Distribution](#Distribution)
5. [Usage](./launcher/README.md)

## Project Dependencies
- This project depens on the developer to have a valid `VirusTotal` API key.  The key is used to check the downloaded file for virus.  The key is stored in the environment variable `VTAPIKEY `.  The key is used by the `NFile` class to check the downloaded file for virus.  Use the following command to set the environment variable:
```bash
setx VTAPIKEY "your_valid_virustotal_api_key"
```

## Development Environment Setup
- The project is built using the `NTools` Standardized build tools.  To setup the development environment
1. Open a powershell in administrator mode
2. Run the following commands in the `DevSetup` directory of the solution:
```bash
cd DevSetup
.\DevSetup.ps1
```

## Build and Test
- The project is built and tested using the `NTools` Standardized build tools.  The `Nb` tool is used to build msbuild targets defined in the `nbuild.targets` file. To build and test
1. Open a Developer command prompt for VS 2022
2. Run the following command in directory of the solution to create a local build to deploy and test:
```bash
nb staging
```
3. Run the following command in directory of the solution to list the available build targets:
```bash
nb -c targets
```
The [targets](./targets.md) file contains the list of available targets.

## Distribution
- This class library is released to nuget.org as `ntools-launcher` package. 
