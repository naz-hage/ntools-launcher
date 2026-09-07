param(
    [string]$nToolsVersion = "1.78.0",  # default version of NTools to install
    [switch]$installMongoDB  # Add this switch to control whether MongoDB should be installed
)
# Get the common Install module and import it
#########################
$url = "https://raw.githubusercontent.com/naz-hage/ntools/main/scripts/module-package/ntools-scripts.psm1"
$output = "./ntools-scripts.psm1"
Invoke-WebRequest -Uri $url -OutFile $output
Import-Module $output -Force

$fileName = Split-Path -Leaf $PSCommandPath

Write-OutputMessage $fileName "Started installation script."

# Check if admin
#########################
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-OutputMessage $fileName "Error: Please run this script as an administrator."
    exit 1
} else {
    Write-OutputMessage $fileName "Admin rights detected"
}

# install Ntools
#########################
Write-OutputMessage $fileName "Installing NTools..."
$result = Install-NTools -Version $nToolsVersion
if (-not $result) {
    Write-Host "Failed to install NTools. Please check the logs for more details." -ForegroundColor Red
    exit 1
}

# install Nuget
#########################
Write-OutputMessage $fileName "Installing Nuget..."
& $"$env:ProgramFiles/nbuild/sdo.exe" tool install -j .\nuget.json
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage $fileName "Error: Installation of nuget.json failed. Exiting script."
    exit 1
}

Write-OutputMessage $fileName "Completed installation script."
Write-OutputMessage $fileName "EmtpyLine"