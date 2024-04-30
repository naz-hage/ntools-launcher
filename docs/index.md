
## Introduction

### ntools-launcher

The `ntools-launcher` is a NuGet package library that simplifies common tasks related to launching executables, downloading files, executing shell commands, and checking process elevation. This library is designed to be easy to use, while providing robust functionality for a variety of tasks.

## Features

- **Launcher:** A class with methods to launch a process and wait for it to complete. This includes methods for locking a file and verifying its digital signature before launching, and launching a process in a separate thread.
- **ResultHelper:** A helper class for retrieving the result `Code` and `Output` of the launched executable.

- **NFile:** A class with a method for downloading files from the web. This method for checks Web download Url inegrity, the downloaded file signature, file size, and VirusTotal check.
- **ResultDownload:** A helper class for retrieving the result of a downloaded file.

- **CurrentProcess:** A class that provides a method to determine if the current process is elevated.

- **ShellUtility:** A helper class for executing shell commands and retrieving the full path of a file from the Path environment variable.







