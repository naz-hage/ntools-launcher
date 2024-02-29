# FILEPATH: /c:/source/ntools-launcher/DevSetup/DevSetup.ps1
$DEV_DRIVE = "c:"
$MAIN_DIR = "source"
$downloadsDirectory = "c:\NToolsDownloads"



<#
.SYNOPSIS: Prepares the downloads directory.
.DESCRIPTION: This function prepares the downloads directory by performing necessary setup tasks.
.PARAMETER None This function does not accept any parameters.
.RETURN This function does not return any value.
.EXAMPLE PrepareDownloadsDirectory
#>
function PrepareDownloadsDirectory {
    # Create the Downloads directory if it doesn't exist
    if (!(Test-Path -Path $downloadsDirectory)) {
        New-Item -ItemType Directory -Path $downloadsDirectory | Out-Null
    }

    # Grant Administrators full control of the Downloads directory
    icacls.exe $downloadsDirectory /grant 'Administrators:(OI)(CI)F' /inheritance:r
}

<#
.SYNOPSIS: Main function to install the app.
.DESCRIPTION: This function is the entry point for the script. It sets up the development environment by installing the apps.
.PARAMETER json The json file containing the app information.
.RETURN This function does not return any value.
.EXAMPLE Main
#>
function Main {

    # Check if admin
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
    if (-not $isAdmin) {
        Write-Host "Please run this script as an administrator."
        exit 1
    }
    else {
        Write-Host "Running as administrator" -ForegroundColor Yellow -BackgroundColor Black
    }
    
    PrepareDownloadsDirectory $downloadsDirectory

    # Install DotNet Runtime
    Invoke-Expression -Command ".\install-app.ps1 .\app-Dotnet_Runtime.json"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "An error occurred during the installation of Ntools."  -ForegroundColor Red -BackgroundColor Black
        exit 1
    }

    # Install Ntools
    Invoke-Expression -Command ".\install-app.ps1 .\app-Ntools.json"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "An error occurred during the installation of Ntools."  -ForegroundColor Red -BackgroundColor Black
        exit 1
    }

    Write-Host "Setting up the development environment..."  -ForegroundColor Yellow -BackgroundColor Black

    # Install Development Tools
    Invoke-Expression -Command ".\install-ntools.ps1 $DEV_DRIVE $MAIN_DIR"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "An error occurred during the installation of Ntools." -ForegroundColor Red -BackgroundColor Black
        exit 1
    }

    Write-Host "Development environment has been successfully setup." -ForegroundColor Green -BackgroundColor Black -NoNewline
    Write-Host "" -ForegroundColor Green -BackgroundColor Black
}

<#
    The following command runs the Main function with the provided or default values.
#>
Main