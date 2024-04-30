#The [cmdletbinding()] attribute is used to make the script function like a cmdlet
# it is a lightweight command used in the PowerShell environment. This attribute allows the script to use cmdlet 
# features such as common parameters (like -Verbose, -Debug, etc.) and the ability to be used in pipelines.
[cmdletbinding()]
param(
    [Parameter(Mandatory = $false)]
    [String]
    $DevDrive = "c:",

    [Parameter(Mandatory = $false)]
    [String]
    $MainDir = "source"
)

# Get the common Install module and import it
#########################
$url = "https://raw.githubusercontent.com/naz-hage/ntools/main/DevSetup/install.psm1"
$output = "./install.psm1"
Invoke-WebRequest -Uri $url -OutFile $output
Import-Module ./install.psm1 -Force

$fileName = Split-Path -Leaf $PSCommandPath

Write-OutputMessage $fileName "Started installation script."

# install Ntools
#########################
MainInstallApp -command install -json .\app-Ntools.json
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage $fileName "Error: Installation of app-Ntools.json failed. Exiting script."
    exit 1

}

# Set the development environment variables
#########################
SetDevEnvironmentVariables -devDrive $DevDrive -mainDir $MainDir


Write-OutputMessage $fileName "Completed installation script."
Write-OutputMessage $fileName "EmtpyLine"
