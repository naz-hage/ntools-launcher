# Software Tools Collection

## Introduction
This repository contains a .NET Core class library that wraps around the .NET class Process. It provides a `ResultHelper` class that returns the code and output of the launcher executable. The library is used by the ntools `Nbackup` program to launch `robocopy` and wait for it to complete, ensuring that the backup process is fully completed before the program continues.

## Table of Contents
1. [Distribution](#Distribution)
2. [Usage](#usage)
3. [Launcher Usage](./launcher/README.md)

## Distribution
- This class library is released to nuget.org as `Ntools-Launcher`. 

## Usage
To use the `Ntools-Launcher` library in your project, follow these steps:

1. Install the `Ntools-Launcher` package from nuget.org.
2. Import the `Ntools.Launcher` namespace in your code.
3. Create an instance of the `ResultHelper` class.
4. Use the `Launch` method to execute the launcher executable.
5. Retrieve the result code and output using the `ResultCode` and `Output` properties of the `ResultHelper` instance.

For more detailed information on how to use the launcher, refer to the [Launcher Usage](./launcher/README.md) documentation.
