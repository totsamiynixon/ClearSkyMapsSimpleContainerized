param([Parameter(Mandatory=$true)][string]$path)
try
{
    $jsonRequest = @{
        "FirebaseCloudMessaging:ServerKey" = $env:FIREBASE_CLOUD_MESSAGING__SERVER_KEY;
        "FirebaseCloudMessaging:MessagingSenderId" = $env:FIREBASE_CLOUD_MESSAGING__MESSAGING_SENDER_ID;
        "ConnectionString" = $env:CONNECTION_STRING;
        "Emulation:ConnectionString" = $env:EMULATION_CONNECTION_STRING;
        "Emulation:Enabled" = $env:EMULATION_ENABLED;
        "Application:Environment" = $env:APPLICATION_ENVIRONMENT;
        "Application:Version" = $env:APPLICATION_VERSION;
        "Yandex:MapsJavaScriptAPIKey" = $env:YANDEX__MAPS_JAVASCRIPT_API_KEY
   }
   $jsonRequest | ConvertTo-Json -depth 100 | Out-File $path;
}
catch
{
    Write-Host "$($_.Exception.Message)"  -ForegroundColor Red
    Write-Host "$($_.InvocationInfo.PositionMessage)" -ForegroundColor Red
    exit 1
}