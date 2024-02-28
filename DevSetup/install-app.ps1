# .\Install-app.ps1

$downloadsDirectory = "c:\NToolsDownloads"

function PrepareDownloadsDirectory {
    param (
        [Parameter(Mandatory=$true)]
        [string]$directory)

    # Create the Downloads directory if it doesn't exist
    if (!(Test-Path -Path $directory)) {
        New-Item -ItemType Directory -Path $directory | Out-Null
    }

    # Grant Administrators full control of the Downloads directory
    icacls.exe $directory /grant 'Administrators:(OI)(CI)F' /inheritance:r
    
}

<#
.SYNOPSIS Get the file version.
.DESCRIPTION This function gets the file version of the specified file.
.PARAMETER FilePath The file path.
.RETURN This function returns the file version.
#>
function GetFileVersion {
    param (
        [Parameter(Mandatory=$true)]
        [string]$FilePath
    )   

    $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($FilePath)
    # return the all file version parts joined by a dot
    return ($versionInfo.FileMajorPart, $versionInfo.FileMinorPart, $versionInfo.FileBuildPart, $versionInfo.FilePrivatePart) -join "."
}

<#
.SYNOPSIS: Get the app information from the json file.
.DESCRIPTION: This function reads the json file and returns the app information.
.PARAMETER jsonFile The json file containing the app information.
.RETURN This function returns the app information.
.EXAMPLE GetAppInfo -jsonFile "git.json"
#>
function GetAppInfo {
    param (
        [Parameter(Mandatory=$true)]
        [string]$jsonFile
    )
    # read file git.json and convert to json object
    $json = Get-Content -Path $jsonFile -Raw
    
    # $config = $json | ConvertFrom-Json
    # Retrieve elements using dot notation

    $config = $json | ConvertFrom-Json | Select-Object -ExpandProperty NbuildAppList | Select-Object -First 1

    $appInfo = @{
        Name = $config.Name
        Version = $config.Version
        AppFileName = $config.AppFileName
        WebDownloadFile = $config.WebDownloadFile
        DownloadedFile = $config.DownloadedFile
        InstallCommand = $config.InstallCommand
        InstallArgs = $config.InstallArgs
        InstallPath = $config.InstallPath
        UninstallCommand = $config.UninstallCommand
        UninstallArgs = $config.UninstallArgs            
    }

    # Update the InstallPath and AppFileName with the actual path
    $appInfo.InstallPath = $appInfo.InstallPath -replace '\$\(ProgramFiles\)', $env:ProgramFiles
    $appInfo.AppFileName = $appInfo.AppFileName -replace '\$\(InstallPath\)', $appInfo.InstallPath
    $appInfo.AppFileName = $appInfo.AppFileName -replace '\$\(ProgramFiles\)', $env:ProgramFiles

    $appInfo.InstallCommand = $appInfo.InstallCommand -replace '\$\(Version\)', $appInfo.Version
    $appInfo.InstallArgs = $appInfo.InstallArgs -replace '\$\(InstallPath\)', $appInfo.InstallPath
    $appInfo.InstallArgs = $appInfo.InstallArgs -replace '\$\(Version\)', $appInfo.Version

    $appInfo.UninstallArgs = $appInfo.UninstallArgs -replace '\$\(InstallPath\)', $appInfo.InstallPath

    $appInfo.DownloadedFile = $appInfo.DownloadedFile -replace '\$\(Version\)', $appInfo.Version

    $appInfo.WebDownloadFile = $appInfo.WebDownloadFile -replace '\$\(Version\)', $appInfo.Version

    return $appInfo
}

<#
.SYNOPSIS: Check if the app is installed.
.DESCRIPTION: This function checks if the app is installed and if the version is correct.
.PARAMETER json The json file containing the app information.
.RETURN This function returns true if the app is installed and the version is correct; otherwise, it returns false.
.EXAMPLE CheckIfAppInstalled -json "app.json"
#>
function CheckIfAppInstalled {
    param (
        [Parameter(Mandatory=$true)]
        [string]$json
    )
   
    $appInfo = GetAppInfo $json
    # check if app is installed
     if (-not (Test-Path -Path $appInfo.AppFileName)) {
        Write-Host "$($appInfo.Name) file: $($appInfo.AppFileName) is not found."
         return $false
     }
     else
     {
        # check if the version is correct
        $installedVersion = GetFileVersion $appInfo.AppFileName
        $targetVersion = $appInfo.Version
        Write-Host "$($appInfo.Name)  version: $($appInfo.Version) is found."

        # check if the installed version is greater than or equal to the target version
        if ([version]$installedVersion -ge [version]$targetVersion) {
            return $true
        }
        else
        {
            return $false
        }
     }
}

<#
.SYNOPSIS: Install the app.
.DESCRIPTION: This function installs the app.
.PARAMETER json The json file containing the app information.
.RETURN This function returns the exit code from the installation process.
.EXAMPLE Install -json "app.json"
#>
function  Install {
    param (
        [Parameter(Mandatory=$true)]
        [string]$json
    )
     # Retrieve elements using dot notation
     $appInfo = GetAppInfo $json

    # download the App
    $output = $downloadsDirectory + "\\" + $appInfo.DownloadedFile
    
    # replace $(Version) with the version number
    $output = $output -replace '\$\(Version\)', $appInfo.Version
    $webUri = $appInfo.WebDownloadFile -replace '\$\(Version\)', $appInfo.Version
    Write-Host "Downloading $($webUri) to : $($output) ..."
    Invoke-WebRequest -Uri $webUri -OutFile $output

    # install the App
    $installCommand = $appInfo.InstallCommand -replace '\$\(Version\)', $appInfo.Version
    write-host "Installing $($appInfo.Name) version: $($appInfo.Version) ..."
    write-host "Install command: $installCommand"
    
    $installArgs=$appInfo.InstallArgs
    write-host "Install arguments: $installArgs"
    $workingFolder = "C:\NToolsDownloads"
    
    $process = Start-Process -FilePath $installCommand -ArgumentList $installArgs -WorkingDirectory $workingFolder -PassThru
    $process.WaitForExit()
    # check return code from the start-process
    if ($process.ExitCode -eq 0 -and (CheckIfAppInstalled $json)) {
        Write-Host "App $($appInfo.Name) version: $($appInfo.Version) is installed successfully."
        return $process.ExitCode
    }
    else {
        Write-Host "App $($appInfo.Name) version: $($appInfo.Version) installation failed with exit code $($process.ExitCode)."
        return $process.ExitCode
    }
}
    
<#
.SYNOPSIS: Main function to install the app.
.DESCRIPTION: This function is the entry point for the script. It installs the app.
.PARAMETER json The json file containing the app information.
.RETURN This function does not return any value.
.EXAMPLE Main -command "install" -json "git.json"
#>
function Main {
    param (
        [Parameter(Mandatory=$true)]
        [string]$json
        )

    PrepareDownloadsDirectory $downloadsDirectory

    $app = GetAppInfo $json
    # check if Git is installed
    $installed = CheckIfAppInstalled $json
    if ($installed) {
        Write-Host "App: $($app.Name) version: $($app.Version) or greater is already installed."
    }
    else {
        Install $json

    }
}

<#
    The following command runs the Main function with the provided or default values.
    The json file is passed as the second argument. 
#>
Main -json $args[0]