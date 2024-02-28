# .\InstallNtools.ps1

$downloadsDirectory = "c:\NToolsDownloads"
$nbExePath = "$env:ProgramFiles\Nbuild\nb.exe"

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

    # prepare the downloads directory
    PrepareDownloadsDirectory $downloadsDirectory


    
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
}

# Call the Main function with the provided or default values
Main -devDrive $args[0] -mainDir $args[1]
