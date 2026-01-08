# Get the common Install module and import it
#########################
$url = "https://raw.githubusercontent.com/naz-hage/ntools/main/dev-setup/install.psm1"
$output = "./install.psm1"
Invoke-WebRequest -Uri $url -OutFile $output
Import-Module ./install.psm1 -Force

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
$result = InstallNTools
if (-not $result) {
    Write-Host "Failed to install NTools. Please check the logs for more details." -ForegroundColor Red
    exit 1
}

# install Nuget
#########################
Write-OutputMessage $fileName "Installing Nuget..."
& $global:NbExePath install --json .\nuget.json
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage $fileName "Error: Installation of nuget.json failed. Exiting script."
    exit 1
}

Write-OutputMessage $fileName "Completed installation script."
Write-OutputMessage $fileName "EmtpyLine"