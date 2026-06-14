# Quick Runbook

This repository contains a ready WPF + SQL Server application. Use these commands on a clean classroom machine.

## 1. Clone from Git Bash

```bash
mkdir -p /c/ExamDemo
cd /c/ExamDemo
git clone https://github.com/bruhnikita/apple.git
cd apple
```

If Git asks for credentials, use GitHub username `bruhnikita`. For the password field use a GitHub token, not the account password. If Git Credential Manager opens a browser, finish the browser login and repeat the clone if needed.

## Optional cleanup after cloning

If you want to remove this helper file from the local exam folder after cloning, run:

```bash
rm RUNBOOK.md
```

This only removes the local instruction file. It does not affect GitHub.
## 2. Check tools

```bash
git --version
dotnet --version
```

For database deployment SQL Server must be running, and `sqlcmd` must be available. By default scripts use `.\SQLEXPRESS`.

To use another instance from Git Bash:

```bash
export SQLSERVER='localhost'
```

## 3. Deploy database

From Git Bash:

```bash
powershell.exe -ExecutionPolicy Bypass -File "database/sqlserver/setup-db.ps1"
```

To recreate the database from scratch:

```bash
powershell.exe -ExecutionPolicy Bypass -File "database/sqlserver/reset-db.ps1"
```

## 4. Build

```bash
dotnet build "src/BolshayaPachkaMaterials.sln"
```

## 5. Run checks

```bash
dotnet run --project "src/BolshayaPachkaMaterials.TestRunner/BolshayaPachkaMaterials.TestRunner.csproj"
```

Expected output: `All tests passed`.

## 6. Start the application

```bash
dotnet run --project "src/BolshayaPachkaMaterials.App/BolshayaPachkaMaterials.App.csproj"
```

## 7. Demonstration checklist

- Show the main materials list window.
- Check that records are visible immediately after launch.
- Demonstrate search in the top input.
- Demonstrate sorting and type filtering.
- Move pages with the bottom navigation and show the counter.
- Select a record and use edit/delete behavior.
- Select one or more records and use the mass update panel.
- Use `Обновить` after database changes. Use `reset-db.ps1` from the console to recreate the SQL Server database.

## 8. Useful full-path commands after cloning to C:\ExamDemo

```powershell
dotnet build "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.sln"
dotnet run --project "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.TestRunner\BolshayaPachkaMaterials.TestRunner.csproj"
powershell -ExecutionPolicy Bypass -File "C:\ExamDemo\apple\database\sqlserver\setup-db.ps1"
dotnet run --project "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.App\BolshayaPachkaMaterials.App.csproj"
```





