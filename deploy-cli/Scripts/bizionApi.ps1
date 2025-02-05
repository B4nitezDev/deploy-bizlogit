# Rutas
$carpetaOrigen = "C:\\Workspace\\Bizlogit\\bizion\\administration\\Admin.ApiService\\Admin.ApiService\\Admin.ApiService"
$carpetaDest = "C:\\Workspace\\Bizlogit\\Deploy"

if(-not(Test-Path $carpetaDest)) {
    New-Item -ItemType Directory -Path $carpetaDest | Out-Null
}

# Busca y elimina carpetas
Get-ChildItem $carpetaOrigen -Directory | Remove-Item -Recurse -Force

# Busca y Eliminar archivos por extension
$extension = "*.xml", "*.pdb", "*.zip", "appsettings.json", "appsettings.Development.json", "log4net.config", "web.config", "app.config", "Konetic.DabaseContext.dll.config", "Konetic.Services.dll.config", "Konetic.Biz.dll.config", "Bizlogit.SCE.dll.config"

foreach ($extension in $extensions) {
    Get-ChildItem $carpetaOrigen -Recurse -Include $extension | Remove-Item -Force
}

# Obtener los archivos filtrados
$archivosFiltrados = Get-ChildItem $carpetaOrigen -File

$nombreZip = "Administration_1.0.0.0.zip"
$rutaZip = Join-Path -Path $carpetaDest -ChildPath $nombreZip

if(Test-Path $rutaZip) {
    Write-Host "El archivo zip '$nombreZip' ya existe. Por favor, elimine antes de crear uno nuevo."
} else {
    if($archivosFiltrados.Count -gt 0) {
        Compress-Archive -Path $archivosFiltrados.FullName -DestinationPath $rutaZip
    }
    else {
        Write-Host "No Hay archivos para comprimir"
    }
}
