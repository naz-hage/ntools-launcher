# Get the common Install module and import it
#########################
$url = "https://raw.githubusercontent.com/naz-hage/ntools/main/DevSetup/install.psm1"
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
MainInstallApp -command install -json .\app-Ntools.json
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage $fileName "Error: Installation of app-Ntools.json failed. Exiting script."
    exit 1

}


Write-OutputMessage $fileName "Completed installation script."
Write-OutputMessage $fileName "EmtpyLine"