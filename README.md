# FitnessTracker Admin Panel

ASP.NET Core MVC Admin Panel для PostgreSQL (existing schema, no migrations).

## NuGet packages
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.10
dotnet add package Microsoft.AspNetCore.Authentication.Cookies --version 8.0.10
```

## Setup
1. Configure DB connection in `AdminPanel/appsettings.json`:
   - `ConnectionStrings:Default`
2. Configure admin credentials in `AdminPanel/appsettings.Development.json` or user-secrets:
   - `AdminCredentials:Username`
   - `AdminCredentials:Password`

## Run
```bash
dotnet restore
cd AdminPanel
dotnet run
```

Open `/Account/Login` and sign in with configured admin credentials.
