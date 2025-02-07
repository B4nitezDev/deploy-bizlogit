# Ruta de la carpeta donde se encuentran los archivos a procesar
$carpetaOrigen = "C:\Users\PC\Desktop\Workspace\bizion\operation\Bizion.Operation.Mobile\www"

# Ruta de la carpeta donde se guardará el archivo zip
$carpetaDestino = "C:\Users\PC\Desktop\Workspace\Deploy"

# Crear la carpeta de destino si no existe
if (-not (Test-Path $carpetaDestino)) {
    New-Item -ItemType Directory -Path $carpetaDestino | Out-Null
}

# Obtener archivos restantes
$archivosFiltrados = Get-ChildItem $carpetaOrigen


# Nombre del archivo zip
$nombreZip = "IdmsMobile_r1.0.0.0.zip"

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