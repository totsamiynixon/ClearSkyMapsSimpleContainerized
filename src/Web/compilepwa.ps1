
$rootPath = $env:PWA_ROOT_PATH;
[PSCustomObject]$configJSON = Get-Content -Path ($rootPath + "appsettings.Development.json") | ConvertFrom-Json;

$version = $configJSON."Settings"."Version";

$manifestJSON = Get-Content -Path ($rootPath + "Areas/PWA/manifest.json") | ConvertFrom-Json;
$manifestJSON."name" = $manifestJSON."name" + " " + $env."ASPNETCORE_ENVIRONMENT";
$manifestJSON."short_name" = $manifestJSON."short_name" + " " + $env."ASPNETCORE_ENVIRONMENT";
$manifestJSON | Add-Member -NotePropertyName gsm_sender_id -NotePropertyValue  $configJSON."FirebaseCloudMessaging"."MessagingSenderId";

$manifestJSON | ConvertTo-Json -depth 100 | Out-File ($rootPath  + "wwwroot/pwa/manifest.json");


$serviceWorker =  Get-Content -Path ($rootPath  + "Areas/PWA/service-worker.js");
$serviceWorker = $serviceWorker -replace '%MessagingSenderId%', $configJSON."FirebaseCloudMessaging"."MessagingSenderId";
$serviceWorker = $serviceWorker -replace '%Version%', $version;

$serviceWorker | Set-Content -Path ($rootPath + "wwwroot/pwa/service-worker.js");
