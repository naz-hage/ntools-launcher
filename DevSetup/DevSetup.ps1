# FILEPATH: /c:/source/ntools-launcher/DevSetup/DevSetup.ps1
$DEV_DRIVE = "c:"
$MAIN_DIR = "source"

# Check if admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "Please run this script as an administrator."
    exit 1
}

# Install DotNet Runtime
Invoke-Expression -Command ".\install-app.ps1 .\app-Dotnet_Runtime.json"
if ($LASTEXITCODE -ne 0) {
    Write-Host "An error occurred during the installation of Ntools."
    exit 1
}

# Install Ntools
Invoke-Expression -Command ".\install-app.ps1 .\app-Ntools.json"
if ($LASTEXITCODE -ne 0) {
    Write-Host "An error occurred during the installation of Ntools."
    exit 1
}

# Install Development Tools
Invoke-Expression -Command ".\install-ntools.ps1 $DEV_DRIVE $MAIN_DIR"
if ($LASTEXITCODE -ne 0) {
    Write-Host "An error occurred during the installation of Ntools."
    exit 1
}

Write-Output "Development environment has been successfully installed."