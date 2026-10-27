<#
  One-time setup for the College Complaint Management System (Windows PowerShell).

  What it does:
    1. Checks the .NET SDK and installs the EF Core tool (dotnet-ef) if needed
    2. Asks for your MySQL details and the first admin account, and stores them in
       .NET User Secrets (outside the project folder, never committed to Git)
    3. Restores NuGet packages
    4. Creates the EF Core migration (if the Migrations folder does not exist yet)
    5. Creates the CollegeComplaintDB database and all tables

  Run it from the project folder:   .\setup.ps1      (or double-click setup.bat)
#>

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

function Read-Plain([string]$prompt) {
    $secure = Read-Host $prompt -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }
}

function Read-WithDefault([string]$prompt, [string]$default) {
    $value = Read-Host "$prompt [$default]"
    if ([string]::IsNullOrWhiteSpace($value)) { return $default }
    return $value.Trim()
}

Write-Host ""
Write-Host "=== College Complaint Management System: setup ===" -ForegroundColor Cyan

# 1. .NET SDK + dotnet-ef
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK not found. Install the .NET 8 SDK from https://dotnet.microsoft.com/download and run this script again."
}
Write-Host "Using .NET SDK $(dotnet --version)"

$env:PATH += ";$env:USERPROFILE\.dotnet\tools"
if (-not (dotnet tool list --global | Select-String "dotnet-ef")) {
    Write-Host "Installing dotnet-ef 8.0.11 ..."
    dotnet tool install --global dotnet-ef --version 8.0.11
}

# 2. Secrets
Write-Host ""
Write-Host "MySQL connection (stored in User Secrets, not in the project)" -ForegroundColor Cyan
$dbHost = Read-WithDefault "MySQL host" "localhost"
$dbPort = Read-WithDefault "MySQL port" "3306"
$dbUser = Read-WithDefault "MySQL user" "root"
$dbPass = Read-Plain "MySQL password"
if ($dbPass.Contains(";")) { throw "The MySQL password must not contain a semicolon (;). Change it or edit the secret manually." }

Write-Host ""
Write-Host "First administrator account" -ForegroundColor Cyan
$adminEmail = Read-WithDefault "Admin email" "admin@college.edu"
$adminPass = Read-Plain "Admin password (min 8 chars with upper, lower, digit and symbol)"
if ($adminPass.Length -lt 8) { throw "The admin password must be at least 8 characters." }

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=$dbHost;port=$dbPort;database=CollegeComplaintDB;user=$dbUser;password=$dbPass" | Out-Null
dotnet user-secrets set "AdminSeed:Email" $adminEmail | Out-Null
dotnet user-secrets set "AdminSeed:Password" $adminPass | Out-Null
Write-Host "Secrets saved."

# 3. Restore
Write-Host ""
Write-Host "Restoring packages ..." -ForegroundColor Cyan
dotnet restore

# 4 + 5. Migration and database (User Secrets are only read in the Development environment)
$env:ASPNETCORE_ENVIRONMENT = "Development"

if (-not (Test-Path (Join-Path $PSScriptRoot "Migrations"))) {
    Write-Host ""
    Write-Host "Creating the InitialCreate migration ..." -ForegroundColor Cyan
    dotnet ef migrations add InitialCreate
}

Write-Host ""
Write-Host "Creating the database and tables ..." -ForegroundColor Cyan
dotnet ef database update

Write-Host ""
Write-Host "Setup complete." -ForegroundColor Green
Write-Host "Start the app with:  dotnet run   (or press F5 in Visual Studio)"
Write-Host "Admin login: $adminEmail"
