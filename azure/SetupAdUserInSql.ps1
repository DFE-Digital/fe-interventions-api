param ($serverName, $databaseName, $appName, $appUserId)


function ConvertTo-Sid {
    param (
        [string]$appId
    )
    [guid]$guid = [System.Guid]::Parse($appId)
    foreach ($byte in $guid.ToByteArray()) {
        $byteGuid += [System.String]::Format("{0:X2}", $byte)
    }
    return "0x" + $byteGuid
}


Write-Host "Getting access token"
$accountToken = az account get-access-token --resource "https://database.windows.net" | ConvertFrom-Json

$appUserSid = ConvertTo-Sid($appUserId)

$sql = "IF NOT EXISTS (select * from sys.database_principals where name = '${appName}') BEGIN "
$sql = $sql + "CREATE USER [${appName}] WITH DEFAULT_SCHEMA=[dbo], SID = ${appUserSid}, TYPE = E; "
$sql = $sql + "ALTER ROLE db_datareader ADD MEMBER [${appName}]; "
$sql = $sql + "ALTER ROLE db_datawriter ADD MEMBER [${appName}]; "
$sql = $sql + "END"

Write-Host "Executing SQL: ${sql}"
Invoke-SqlCmd -ServerInstance "${serverName}.database.windows.net" -Database $databaseName -Query $sql -AccessToken $accountToken.accessToken

