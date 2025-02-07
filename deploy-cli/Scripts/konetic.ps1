Write-Host "Entro al script"

# Ruta de la carpeta donde se encuentran los archivos a procesar
$carpetaOrigen = "C:\Users\PC\Desktop\Workspace\konetic\trunk\konetic.web\bin\release\net472\win7-x86\publish"

# Ruta de la carpeta donde se guardará el archivo zip
$carpetaDestino = "C:\Users\PC\Desktop\Workspace\Deploy"

Write-Host "Verificando la existencia de la carpeta de destino..."
if (-not (Test-Path $carpetaDestino)) {
    Write-Host "La carpeta de destino no existe. Creando la carpeta..."
    New-Item -ItemType Directory -Path $carpetaDestino | Out-Null
}

Write-Host "Eliminando carpetas en la carpeta de origen..."
# Buscar y eliminar carpetas
Get-ChildItem $carpetaOrigen -Directory | Remove-Item -Recurse -Force

Write-Host "Eliminando archivos por extensión en la carpeta de origen..."
# Buscar y eliminar archivos por extensión
$extensiones = "*.xml", "*.pdb", "*.zip","appsettings.json", "appsettings.Development.json", "log4net.config", "web.config", "app.config", "Konetic.DatabaseContext.dll.config", "Konetic.Services.dll.config", "Konetic.Biz.dll.config", "Bizlogit.SCE.dll.config"

foreach ($extension in $extensiones) {
    Write-Host "Eliminando archivos con la extensión $extension..."
    Get-ChildItem $carpetaOrigen -Recurse -Include $extension | Remove-Item -Force
}

Write-Host "Obteniendo archivos restantes..."
# Obtener archivos restantes
$archivosFiltrados = Get-ChildItem $carpetaOrigen -File

# Nombre del archivo zip
$nombreZip = "IdmsApi_r1.0.0.0.zip"

# Ruta completa del archivo zip
$rutaZip = Join-Path -Path $carpetaDestino -ChildPath $nombreZip

Write-Host "Comprobando si el archivo zip ya existe..."
# Comprobar si el archivo zip ya existe
if (Test-Path $rutaZip) {
    Write-Host "El archivo zip '$nombreZip' ya existe. Por favor, elimine antes de crear uno nuevo."
} else {
    Write-Host "Comprobando si hay archivos para comprimir..."
    # Comprobar si hay archivos para comprimir
    if ($archivosFiltrados.Count -gt 0) {
        Write-Host "Comprimiendo archivos en $rutaZip..."
        Compress-Archive -Path $archivosFiltrados.FullName -DestinationPath $rutaZip
    } else {
        Write-Host "No hay archivos para comprimir."
    }
}

Write-Host "Script de PowerShell finalizado."
