##
### ntools-launcher

The `ntools-launcher` is a NuGet package library that simplifies common tasks related to launching executables, downloading files, executing shell commands, and checking process elevation. This library is designed to be easy to use, while providing robust functionality for a variety of tasks.

**Boost your productivity with `ntools-launcher`**, automating common development tasks, including:

* Launching executables directly from your code.
* Downloading files securely with ease.
* Executing shell commands for automated scripting.
* Checking process elevation for enhanced security awareness.

### Key benefits

* **Improved Efficiency:** Automate repetitive tasks and focus on core development activities.
* **Simplified Workflow:** User-friendly API streamlines integration into your projects.
* **Enhanced Control:** Manage processes and ensure proper elevation levels for secure development.

This library is used by the [ntools's](https://github.com/naz-hage/ntools) repo

- **Nbackup** launches `robocopy` and wait for it to complete, ensuring that the backup process is fully completed before the program continues.
- **Nbuild** launches `msbuild` to build and test .NET projects.  It also builds selected development tools for the project.  It utilizes the `NFile` class to download the tools from the web.
- **Ngit** launches `git` to perform git operations.

### Features

- **Launcher:** A class with methods to launch a process and wait for it to complete. This includes methods for locking a file and verifying its digital signature before launching, and launching a process in a separate thread.
- **ResultHelper:** A helper class for retrieving the result `Code` and `Output` of the launched executable.
- **NFile:** A class with a method for downloading files from the web. This method for checks Web download Url integrity, the downloaded file signature, file size, and VirusTotal check.
- **ResultDownload:** A helper class for retrieving the result of a downloaded file.
- **CurrentProcess:** A class that provides a method to determine if the current process is elevated.
- **ShellUtility:** A helper class for executing shell commands and retrieving the full path of a file from the Path environment variable.