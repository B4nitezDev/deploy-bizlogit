# Ruta de la carpeta donde se encuentran los archivos a procesar
$carpetaOrigen = "c:\workspace\bizlogit\konetic\trunk\konetic.web\bin\release\net472\win7-x86\publish"

# Ruta de la carpeta donde se guardará el archivo zip
$carpetaDestino = "C:\workspace\bizlogit\Deploy"

# Crear la carpeta de destino si no existe
if (-not (Test-Path $carpetaDestino)) {
    New-Item -ItemType Directory -Path $carpetaDestino | Out-Null
}

# Buscar y eliminar carpetas
Get-ChildItem $carpetaOrigen -Directory | Remove-Item -Recurse -Force

# Buscar y eliminar archivos por extensión
$extensiones = "*.xml", "*.pdb", "*.zip","appsettings.json", "appsettings.Development.json", "log4net.config", "web.config", "app.config", "Konetic.DatabaseContext.dll.config", "Konetic.Services.dll.config", "Konetic.Biz.dll.config", "Bizlogit.SCE.dll.config"

foreach ($extension in $extensiones) {
    Get-ChildItem $carpetaOrigen -Recurse -Include $extension | Remove-Item -Force
}

# Obtener archivos restantes
$archivosFiltrados = Get-ChildItem $carpetaOrigen -File


# Nombre del archivo zip
$nombreZip = "IdmsApi_r1.0.0.0.zip"

# Ruta completa del archivo zip
$rutaZip = Join-Path -Path $carpetaDestino -ChildPath $nombreZip

# Comprobar si el archivo zip ya existe
if (Test-Path $rutaZip) {
    Write-Host "El archivo zip '$nombreZip' ya existe. Por favor, elimine antes de crear uno nuevo."
} else {
# Comprobar si hay archivos para comprimir
if ($archivosFiltrados.Count -gt 0) {
    # Comprimir archivos restantes en un archivo zip
    Compress-Archive -Path $archivosFiltrados.FullName -DestinationPath $rutaZip
}
else {
    Write-Host "No hay archivos para comprimir."
}
}