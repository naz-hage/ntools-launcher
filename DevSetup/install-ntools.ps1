# .\InstallNtools.ps1

$nbExePath = "$env:ProgramFiles\Nbuild\nb.exe"

<#
.SYNOPSIS: Main function to install NTools.
.DESCRIPTION: This function is the entry point for the script. It installs NTools using the nb.exe tool.
.PARAMETER devDrive The drive letter where the development tools are installed.
.PARAMETER mainDir The directory where the main development tools are installed.
.RETURN This function does not return any value.
.EXAMPLE Main -devDrive "C:" -mainDir "C:\Nbuild"
#>
function Main {
    param (
        [Parameter(Mandatory=$true)]
        [string]$devDrive,
        [Parameter(Mandatory=$true)]
        [string]$mainDir)

    # set DevDrive and MainDir environment variables
    if (-not (Test-Path Env:\DevDrive)) {
        Set-Item -Path Env:\DevDrive -Value $devDrive
    }

    if (-not (Test-Path Env:\MainDir)) {
        Set-Item -Path Env:\MainDir -Value $mainDir
    }

    Write-Host "devDrive: $devDrive"
    Write-Host "mainDir: $mainDir"
    
    
    & $nbExePath -c install -json ntools.json
    & $nbExePath -c list -json ntools.json
}

# Call the Main function with the provided or default values
Main -devDrive $args[0] -mainDir $args[1]
