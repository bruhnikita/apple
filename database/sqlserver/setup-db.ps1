$server=if($env:SQLSERVER){$env:SQLSERVER}else{'.\SQLEXPRESS'}; Get-ChildItem $PSScriptRoot -Filter '*.sql'|Sort Name|%{sqlcmd -S $server -E -i $_.FullName}
