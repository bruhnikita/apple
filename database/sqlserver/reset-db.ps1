$ErrorActionPreference = 'Stop'
$server = if ($env:SQLSERVER) { $env:SQLSERVER } else { '.\SQLEXPRESS' }
sqlcmd -S $server -E -b -Q "IF DB_ID(N'BolshayaPachkaMaterials') IS NOT NULL BEGIN ALTER DATABASE [BolshayaPachkaMaterials] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [BolshayaPachkaMaterials]; END"
& $PSScriptRoot\setup-db.ps1
