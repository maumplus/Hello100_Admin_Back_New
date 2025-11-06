# Hello100Admin API 실행 스크립트 (Windows PowerShell)

Write-Host "🚀 Starting Hello100Admin API..." -ForegroundColor Green
Write-Host ""

Set-Location -Path "$PSScriptRoot\src\API"
dotnet run
