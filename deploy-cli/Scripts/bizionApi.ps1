# Rutas
$carpetaOrigen = "C:\\Users\\PC\\Desktop\\Workspace\\bizion\\administration\\Admin.ApiService\\Admin.ApiService\\Admin.ApiService"
$carpetaDest = "C:\\Users\\PC\\Desktop\\Workspace\\Deploy"

Write-Host "Verificando la existencia de la carpeta de destino..."
if(-not(Test-Path $carpetaDest)) {
    Write-Host "La carpeta de destino no existe. Creando la carpeta..."
    New-Item -ItemType Directory -Path $carpetaDest | Out-Null
}

Write-Host "Eliminando carpetas en la carpeta de origen..."
# Busca y elimina carpetas
Get-ChildItem $carpetaOrigen -Directory | Remove-Item -Recurse -Force

Write-Host "Eliminando archivos por extensión en la carpeta de origen..."
# Busca y Eliminar archivos por extension
$extensions = "*.xml", "*.pdb", "*.zip", "appsettings.json", "appsettings.Development.json", "log4net.config", "web.config", "app.config", "Konetic.DabaseContext.dll.config", "Konetic.Services.dll.config", "Konetic.Biz.dll.config", "Bizlogit.SCE.dll.config"

foreach ($extension in $extensions) {
    Write-Host "Eliminando archivos con la extensión $extension..."
    Get-ChildItem $carpetaOrigen -Recurse -Include $extension | Remove-Item -Force
}

Write-Host "Obteniendo los archivos filtrados..."
# Obtener los archivos filtrados
$archivosFiltrados = Get-ChildItem $carpetaOrigen -File

$nombreZip = "Administration_1.0.0.0.zip"
$rutaZip = Join-Path -Path $carpetaDest -ChildPath $nombreZip

if(Test-Path $rutaZip) {
    Write-Host "El archivo zip '$nombreZip' ya existe. Por favor, elimine antes de crear uno nuevo."
} else {
    if($archivosFiltrados.Count -gt 0) {
        Write-Host "Comprimiendo archivos en $rutaZip..."
        Compress-Archive -Path $archivosFiltrados.FullName -DestinationPath $rutaZip
    }
    else {
        Write-Host "No hay archivos para comprimir."
    }
}

Write-Host "Script de PowerShell finalizado."
