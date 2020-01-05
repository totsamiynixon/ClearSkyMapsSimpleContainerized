[PSCustomObject]$configJSON = Get-Content -Path "./config.json" | ConvertFrom-Json;

$version = "";
if (!$configJSON."Application:Version") { 
    $version = ([TimeSpan] (Get-Date).ToShortTimeString()).TotalDays + ([TimeSpan] (Get-Date).ToShortTimeString()).TotalMilliseconds;
} 
else { $version = $configJSON."Application:Version" }

$manifestJSON = Get-Content -Path "./Areas/PWA/manifest.json" | ConvertFrom-Json;
$manifestJSON."name" = $manifestJSON."name" + " " + $configJSON."Application:Environment";
$manifestJSON."short_name" = $manifestJSON."short_name" + " " + $configJSON."Application:Environment";
$manifestJSON | Add-Member -NotePropertyName gsm_sender_id -NotePropertyValue  $configJSON."FirebaseCloudMessaging:MessagingSenderId";

$manifestJSON | ConvertTo-Json -depth 100 | Out-File "./wwwroot/pwa/manifest.json";


$serviceWorker =  Get-Content -Path "./Areas/PWA/service-worker.js";
$serviceWorker = $serviceWorker -replace '%MessagingSenderId%', $configJSON."FirebaseCloudMessaging:MessagingSenderId";
$serviceWorker = $serviceWorker -replace '%Version%', $version;

$serviceWorker | Set-Content -Path "./wwwroot/pwa/service-worker.js";
