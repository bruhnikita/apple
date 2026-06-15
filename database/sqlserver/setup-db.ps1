$ErrorActionPreference = 'Stop'
$server = if ($env:SQLSERVER) { $env:SQLSERVER } else { '.\SQLEXPRESS' }
Get-ChildItem $PSScriptRoot -Filter '*.sql' | Sort-Object Name | ForEach-Object {
    sqlcmd -S $server -E -b -i $_.FullName
}
