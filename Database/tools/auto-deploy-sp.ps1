# =============================================
# Script para Auto-Despliegue de Procedimientos Almacenados
# Autor: Equipo de Desarrollo
# Fecha: 2025-03-07
# =============================================

param(
    [string]$Environment = "local",  # Por defecto, usa el entorno local
    [switch]$DryRun = $false         # Por defecto, ejecuta los cambios
)

# Configuración
$ScriptRoot = $PSScriptRoot
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptRoot)
$DatabaseDir = Split-Path -Parent $ScriptRoot
$SpDir = Join-Path $DatabaseDir "StoredProcedures"
$FlywayDir = Join-Path $ProjectRoot ".flyway"
$LocalMigrationsDir = Join-Path $FlywayDir "migrations-local-db"
$AzureMigrationsDir = Join-Path $FlywayDir "migrations-azure-db"
$LogDir = Join-Path $DatabaseDir "logs"
$LogFile = Join-Path $LogDir "auto_deploy_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

# Crear directorio de logs si no existe
if (-not (Test-Path $LogDir)) {
    New-Item -ItemType Directory -Path $LogDir | Out-Null
}

# Función para escribir en el log
function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    # Escribir en el archivo de log
    Add-Content -Path $LogFile -Value $logMessage
    
    # Mostrar en la consola con colores según el nivel
    switch ($Level) {
        "INFO" { Write-Host $logMessage -ForegroundColor Cyan }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
        "WARNING" { Write-Host $logMessage -ForegroundColor Yellow }
        "ERROR" { Write-Host $logMessage -ForegroundColor Red }
        default { Write-Host $logMessage }
    }
}

# Función para obtener la versión más alta de las migraciones existentes
function Get-LatestVersion {
    param(
        [string]$MigrationsDir
    )
    
    $latestVersion = "1.0.0"
    
    if (Test-Path $MigrationsDir) {
        # Buscar todos los archivos de migración y extraer la versión
        $migrationFiles = Get-ChildItem -Path $MigrationsDir -Filter "V*.sql" -Recurse
        $versions = @()
        
        foreach ($file in $migrationFiles) {
            if ($file.Name -match 'V([0-9]+\.[0-9]+\.[0-9]+)__') {
                $versions += $Matches[1]
            }
        }
        
        # Ordenar versiones y obtener la última
        if ($versions.Count -gt 0) {
            $latestVersion = $versions | Sort-Object { [version]$_ } | Select-Object -Last 1
        }
    }
    
    return $latestVersion
}

# Función para incrementar la versión
function Increment-Version {
    param(
        [string]$Version,
        [string]$IncrementType = "patch"  # Por defecto, incrementa el número de parche
    )
    
    $versionParts = $Version.Split('.')
    $major = [int]$versionParts[0]
    $minor = [int]$versionParts[1]
    $patch = [int]$versionParts[2]
    
    switch ($IncrementType) {
        "major" {
            $major++
            $minor = 0
            $patch = 0
        }
        "minor" {
            $minor++
            $patch = 0
        }
        "patch" {
            $patch++
        }
    }
    
    return "$major.$minor.$patch"
}

# Función para generar un nombre de archivo de migración Flyway
function Get-MigrationFilename {
    param(
        [string]$SpFile,
        [string]$Version
    )
    
    $spName = [System.IO.Path]::GetFileNameWithoutExtension($SpFile)
    
    # Extraer el número de versión del nombre del SP (si existe)
    $spVersion = ""
    if ($spName -match '^(\d+)_') {
        $spVersion = $Matches[1]
        $spName = $spName -replace '^\d+_', ''
    }
    
    # Generar un nombre descriptivo para la migración
    $description = $spName -replace '_', ' ' -replace '([A-Z])', ' $1' -replace '^ ', '' -replace ' ', '_'
    
    return "V${Version}__${description}.sql"
}

# Función para crear un archivo de migración Flyway a partir de un SP
function New-MigrationFile {
    param(
        [string]$SpFile,
        [string]$MigrationsDir,
        [string]$Version
    )
    
    # Generar nombre de archivo de migración
    $migrationFilename = Get-MigrationFilename -SpFile $SpFile -Version $Version
    $migrationFile = Join-Path $MigrationsDir $migrationFilename
    
    # Crear el archivo de migración
    $content = @"
-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
-- Archivo original: $SpFile
-- =============================================

$(Get-Content -Path $SpFile -Raw)
GO
"@
    
    Set-Content -Path $migrationFile -Value $content
    
    Write-Log "Creado archivo de migración: $migrationFilename" -Level "SUCCESS"
    
    return $migrationFile
}

# Función para ejecutar Flyway
function Invoke-Flyway {
    param(
        [string]$Command,
        [string]$Environment
    )
    
    $configFile = ""
    switch ($Environment) {
        "local" {
            $configFile = Join-Path $FlywayDir "flyway-local-db.conf"
        }
        "azure" {
            $configFile = Join-Path $FlywayDir "flyway-azure-db.conf"
        }
        default {
            Write-Log "Entorno desconocido: $Environment" -Level "ERROR"
            return $false
        }
    }
    
    Write-Log "Ejecutando: flyway -configFiles=$configFile $Command" -Level "INFO"
    
    if (-not $DryRun) {
        # Ejecutar Flyway
        & flyway -configFiles="$configFile" $Command
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Flyway $Command ejecutado correctamente" -Level "SUCCESS"
            return $true
        } else {
            Write-Log "Error al ejecutar Flyway $Command" -Level "ERROR"
            return $false
        }
    } else {
        Write-Log "Simulación: flyway -configFiles=$configFile $Command" -Level "INFO"
        return $true
    }
}

# Función para procesar un directorio de SPs
function Process-SpDirectory {
    param(
        [string]$SpDir,
        [string]$MigrationsDir
    )
    
    # Obtener la última versión de las migraciones
    $latestVersion = Get-LatestVersion -MigrationsDir $MigrationsDir
    Write-Log "Última versión de migración: $latestVersion" -Level "INFO"
    
    # Buscar todos los archivos SQL en el directorio de SPs
    $spFiles = Get-ChildItem -Path $SpDir -Filter "*.sql" -Recurse | Sort-Object Name
    
    # Contador para nuevas migraciones
    $newMigrations = 0
    
    # Procesar cada archivo SQL
    foreach ($spFile in $spFiles) {
        $spName = $spFile.Name
        
        # Verificar si el SP ya tiene una migración
        $migrationExists = $false
        
        # Extraer el contenido del SP
        $spContent = Get-Content -Path $spFile.FullName -Raw
        $spContentStart = ($spContent -split "`n" | Select-Object -First 10) -join "`n"
        
        # Buscar migraciones existentes que contengan el mismo SP
        $migrationFiles = Get-ChildItem -Path $MigrationsDir -Filter "V*.sql" -Recurse
        
        foreach ($migrationFile in $migrationFiles) {
            $migrationContent = Get-Content -Path $migrationFile.FullName -Raw
            
            # Si el contenido del SP está en la migración, marcar como existente
            if ($migrationContent -match [regex]::Escape($spContentStart)) {
                $migrationExists = $true
                Write-Log "SP ya migrado: $spName -> $($migrationFile.Name)" -Level "INFO"
                break
            }
        }
        
        # Si el SP no tiene migración, crear una nueva
        if (-not $migrationExists) {
            # Incrementar la versión
            $latestVersion = Increment-Version -Version $latestVersion -IncrementType "patch"
            
            # Crear archivo de migración
            New-MigrationFile -SpFile $spFile.FullName -MigrationsDir $MigrationsDir -Version $latestVersion
            
            # Incrementar contador
            $newMigrations++
        }
    }
    
    return $newMigrations
}

# Función principal
function Start-AutoDeploy {
    Write-Log "Iniciando auto-despliegue de procedimientos almacenados..." -Level "INFO"
    Write-Log "Entorno: $Environment" -Level "INFO"
    
    # Determinar el directorio de migraciones según el entorno
    $migrationsDir = ""
    switch ($Environment) {
        "local" {
            $migrationsDir = $LocalMigrationsDir
        }
        "azure" {
            $migrationsDir = $AzureMigrationsDir
        }
        "both" {
            # Procesar para ambos entornos
            $localMigrations = Process-SpDirectory -SpDir $SpDir -MigrationsDir $LocalMigrationsDir
            $azureMigrations = Process-SpDirectory -SpDir $SpDir -MigrationsDir $AzureMigrationsDir
            
            Write-Log "Nuevas migraciones creadas para entorno local: $localMigrations" -Level "SUCCESS"
            Write-Log "Nuevas migraciones creadas para entorno Azure: $azureMigrations" -Level "SUCCESS"
            
            # Ejecutar Flyway para ambos entornos si hay nuevas migraciones
            if ($localMigrations -gt 0) {
                Invoke-Flyway -Command "migrate" -Environment "local"
            }
            
            if ($azureMigrations -gt 0) {
                Invoke-Flyway -Command "migrate" -Environment "azure"
            }
            
            Write-Log "Auto-despliegue completado" -Level "SUCCESS"
            return
        }
        default {
            Write-Log "Entorno desconocido: $Environment" -Level "ERROR"
            return
        }
    }
    
    # Procesar SPs para el entorno seleccionado
    $newMigrations = Process-SpDirectory -SpDir $SpDir -MigrationsDir $migrationsDir
    
    Write-Log "Nuevas migraciones creadas: $newMigrations" -Level "SUCCESS"
    
    # Ejecutar Flyway si hay nuevas migraciones
    if ($newMigrations -gt 0) {
        Invoke-Flyway -Command "migrate" -Environment $Environment
    } else {
        Write-Log "No hay nuevos procedimientos almacenados para desplegar" -Level "INFO"
    }
    
    Write-Log "Auto-despliegue completado" -Level "SUCCESS"
}

# Ejecutar función principal
Start-AutoDeploy 