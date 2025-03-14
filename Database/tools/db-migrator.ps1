# =============================================
# Script de Migración de Base de Datos para Cursor
# Autor: Equipo de Desarrollo
# Fecha: 2025-03-07
# =============================================

param(
    [string]$Server = "localhost",
    [string]$Database = "AESAN",
    [string]$Username = "sa",
    [string]$Password = "YourPassword",
    [string]$MigrationTableName = "DatabaseMigrations",
    [switch]$Force = $false,
    [switch]$DryRun = $false
)

# Configuración
$ScriptRoot = $PSScriptRoot
$DatabaseRoot = Split-Path -Parent $ScriptRoot
$MigrationsDir = @(
    "$DatabaseRoot\Tables",
    "$DatabaseRoot\StoredProcedures"
)
$LogFile = "$DatabaseRoot\logs\migrations_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

# Crear directorio de logs si no existe
if (-not (Test-Path "$DatabaseRoot\logs")) {
    New-Item -ItemType Directory -Path "$DatabaseRoot\logs" | Out-Null
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
        "INFO" { Write-Host $logMessage -ForegroundColor White }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
        "WARNING" { Write-Host $logMessage -ForegroundColor Yellow }
        "ERROR" { Write-Host $logMessage -ForegroundColor Red }
        default { Write-Host $logMessage }
    }
}

# Función para ejecutar una consulta SQL
function Invoke-SqlQuery {
    param(
        [string]$Query,
        [hashtable]$Parameters = @{},
        [switch]$NonQuery = $false
    )
    
    try {
        # Crear conexión
        $connectionString = "Server=$Server;Database=$Database;User Id=$Username;Password=$Password;"
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        # Crear comando
        $command = New-Object System.Data.SqlClient.SqlCommand($Query, $connection)
        
        # Agregar parámetros
        foreach ($key in $Parameters.Keys) {
            $null = $command.Parameters.AddWithValue($key, $Parameters[$key])
        }
        
        # Ejecutar comando
        if ($NonQuery) {
            $result = $command.ExecuteNonQuery()
        } else {
            $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
            $dataset = New-Object System.Data.DataSet
            $adapter.Fill($dataset) | Out-Null
            $result = $dataset.Tables[0]
        }
        
        return $result
    }
    catch {
        Write-Log "Error al ejecutar consulta SQL: $_" -Level "ERROR"
        throw $_
    }
    finally {
        if ($connection -and $connection.State -eq 'Open') {
            $connection.Close()
        }
    }
}

# Función para ejecutar un script SQL
function Invoke-SqlScript {
    param(
        [string]$ScriptPath
    )
    
    $scriptName = Split-Path $ScriptPath -Leaf
    
    try {
        if ($DryRun) {
            Write-Log "DRY RUN: Se ejecutaría el script: $scriptName" -Level "INFO"
            return $true
        }
        
        Write-Log "Ejecutando script: $scriptName" -Level "INFO"
        
        # Ejecutar el script usando sqlcmd
        & sqlcmd -S $Server -d $Database -U $Username -P $Password -i $ScriptPath
        
        # Verificar si la ejecución fue exitosa
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Script ejecutado correctamente: $scriptName" -Level "SUCCESS"
            return $true
        } else {
            Write-Log "Error al ejecutar el script: $scriptName" -Level "ERROR"
            return $false
        }
    }
    catch {
        Write-Log "Excepción al ejecutar el script $scriptName`: $_" -Level "ERROR"
        return $false
    }
}

# Función para crear la tabla de migraciones si no existe
function Initialize-MigrationTable {
    $query = @"
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '$MigrationTableName')
    BEGIN
        CREATE TABLE [$MigrationTableName] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [ScriptName] NVARCHAR(255) NOT NULL,
            [ScriptPath] NVARCHAR(500) NOT NULL,
            [AppliedOn] DATETIME NOT NULL DEFAULT GETDATE(),
            [Success] BIT NOT NULL,
            [ErrorMessage] NVARCHAR(MAX) NULL
        );
        
        CREATE UNIQUE INDEX [IX_$($MigrationTableName)_ScriptName] ON [$MigrationTableName] ([ScriptName]);
    END
"@
    
    try {
        if ($DryRun) {
            Write-Log "DRY RUN: Se crearía la tabla de migraciones $MigrationTableName si no existe" -Level "INFO"
            return
        }
        
        Write-Log "Inicializando tabla de migraciones $MigrationTableName..." -Level "INFO"
        Invoke-SqlQuery -Query $query -NonQuery
        Write-Log "Tabla de migraciones inicializada correctamente" -Level "SUCCESS"
    }
    catch {
        Write-Log "Error al inicializar la tabla de migraciones: $_" -Level "ERROR"
        throw $_
    }
}

# Función para obtener los scripts ya ejecutados
function Get-AppliedMigrations {
    $query = "SELECT ScriptName FROM [$MigrationTableName] WHERE Success = 1"
    
    try {
        if ($DryRun) {
            Write-Log "DRY RUN: Se consultarían las migraciones aplicadas" -Level "INFO"
            return @()
        }
        
        Write-Log "Obteniendo migraciones aplicadas..." -Level "INFO"
        $result = Invoke-SqlQuery -Query $query
        
        $appliedMigrations = @()
        foreach ($row in $result) {
            $appliedMigrations += $row.ScriptName
        }
        
        Write-Log "Se encontraron $($appliedMigrations.Count) migraciones aplicadas" -Level "INFO"
        return $appliedMigrations
    }
    catch {
        Write-Log "Error al obtener migraciones aplicadas: $_" -Level "ERROR"
        throw $_
    }
}

# Función para registrar una migración ejecutada
function Register-Migration {
    param(
        [string]$ScriptName,
        [string]$ScriptPath,
        [bool]$Success,
        [string]$ErrorMessage = $null
    )
    
    $query = @"
    INSERT INTO [$MigrationTableName] (ScriptName, ScriptPath, AppliedOn, Success, ErrorMessage)
    VALUES (@ScriptName, @ScriptPath, GETDATE(), @Success, @ErrorMessage)
"@
    
    $parameters = @{
        "@ScriptName" = $ScriptName
        "@ScriptPath" = $ScriptPath
        "@Success" = $Success
        "@ErrorMessage" = $ErrorMessage
    }
    
    try {
        if ($DryRun) {
            Write-Log "DRY RUN: Se registraría la migración: $ScriptName (Éxito: $Success)" -Level "INFO"
            return
        }
        
        Write-Log "Registrando migración: $ScriptName (Éxito: $Success)" -Level "INFO"
        Invoke-SqlQuery -Query $query -Parameters $parameters -NonQuery
        Write-Log "Migración registrada correctamente" -Level "SUCCESS"
    }
    catch {
        Write-Log "Error al registrar migración: $_" -Level "ERROR"
    }
}

# Función para obtener todos los scripts SQL en los directorios de migración
function Get-MigrationScripts {
    $allScripts = @()
    
    foreach ($dir in $MigrationsDir) {
        if (Test-Path $dir) {
            # Buscar recursivamente todos los archivos .sql
            $scripts = Get-ChildItem -Path $dir -Filter "*.sql" -Recurse | Sort-Object Name
            $allScripts += $scripts
        }
    }
    
    return $allScripts
}

# Función para extraer el número de versión de un nombre de script
function Get-ScriptVersion {
    param(
        [string]$ScriptName
    )
    
    if ($ScriptName -match '^\d+_') {
        $versionPart = $ScriptName -replace '^(\d+)_.*$', '$1'
        return [int]$versionPart
    }
    
    return 0
}

# Función principal para ejecutar las migraciones
function Invoke-DatabaseMigration {
    try {
        Write-Log "Iniciando proceso de migración de base de datos..." -Level "INFO"
        Write-Log "Servidor: $Server, Base de datos: $Database" -Level "INFO"
        
        # Inicializar tabla de migraciones
        Initialize-MigrationTable
        
        # Obtener migraciones ya aplicadas
        $appliedMigrations = Get-AppliedMigrations
        
        # Obtener todos los scripts de migración
        $allScripts = Get-MigrationScripts
        
        Write-Log "Se encontraron $($allScripts.Count) scripts de migración en total" -Level "INFO"
        
        # Filtrar scripts no aplicados
        $pendingScripts = $allScripts | Where-Object { 
            $scriptName = $_.Name
            -not ($appliedMigrations -contains $scriptName) -or $Force
        }
        
        Write-Log "Se encontraron $($pendingScripts.Count) scripts pendientes de aplicar" -Level "INFO"
        
        if ($pendingScripts.Count -eq 0) {
            Write-Log "No hay scripts pendientes de aplicar" -Level "SUCCESS"
            return
        }
        
        # Ordenar scripts por versión y nombre
        $orderedScripts = $pendingScripts | Sort-Object { 
            $version = Get-ScriptVersion -ScriptName $_.Name
            $name = $_.Name
            return "$version-$name"
        }
        
        # Ejecutar scripts pendientes
        $successCount = 0
        $errorCount = 0
        
        foreach ($script in $orderedScripts) {
            $scriptName = $script.Name
            $scriptPath = $script.FullName
            
            # Si ya se aplicó y no se fuerza, saltar
            if ($appliedMigrations -contains $scriptName -and -not $Force) {
                Write-Log "Script ya aplicado, saltando: $scriptName" -Level "INFO"
                continue
            }
            
            # Ejecutar script
            $success = Invoke-SqlScript -ScriptPath $scriptPath
            
            # Registrar resultado
            if ($success) {
                Register-Migration -ScriptName $scriptName -ScriptPath $scriptPath -Success $true
                $successCount++
            } else {
                Register-Migration -ScriptName $scriptName -ScriptPath $scriptPath -Success $false -ErrorMessage "Error al ejecutar el script"
                $errorCount++
                
                # Si hay un error, detener el proceso a menos que se fuerce
                if (-not $Force) {
                    Write-Log "Error al ejecutar el script $scriptName. Deteniendo el proceso." -Level "ERROR"
                    break
                }
            }
        }
        
        # Resumen
        Write-Log "Proceso de migración completado" -Level "INFO"
        Write-Log "Scripts ejecutados correctamente: $successCount" -Level "SUCCESS"
        
        if ($errorCount -gt 0) {
            Write-Log "Scripts con errores: $errorCount" -Level "ERROR"
            return $false
        }
        
        return $true
    }
    catch {
        Write-Log "Error en el proceso de migración: $_" -Level "ERROR"
        return $false
    }
}

# Cargar ensamblados necesarios
Add-Type -AssemblyName System.Data.SqlClient

# Ejecutar migración
$result = Invoke-DatabaseMigration

# Salir con código de error apropiado
if ($result) {
    exit 0
} else {
    exit 1
} 