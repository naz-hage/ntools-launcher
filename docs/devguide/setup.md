
## Development Environment Setup

Run the dev setup script in administrator PowerShell:
```powershell
cd dev-setup
.\dev-setup.ps1
```

The script accepts `-nToolsVersion` to select the NTools release and
`-installMongoDB` when MongoDB is required by the local environment. It
downloads the shared `ntools-scripts` PowerShell module, installs NTools, and
installs the NuGet development tool manifest.