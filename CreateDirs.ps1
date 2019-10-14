Write-Host "Creating required folders...."
$folders = @(
    ".\logs\sitecore",
    ".\logs\identity",
    ".\logs\xconnect",
    ".\data\mssql",
    ".\data\solr",
    ".\lic",
    ".\buildoutput"
)
New-Item -ItemType directory -Force -Path $folders
Write-Host "Succesfully created all required folders!" -ForegroundColor Green