#!/bin/bash

# =============================================
# Script de Migración de Base de Datos para Cursor
# Autor: Equipo de Desarrollo
# Fecha: 2025-03-07
# =============================================

# Colores para la salida
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Parámetros por defecto
SERVER=${1:-"192.168.1.144"}
DATABASE=${2:-"NUTRE"}
USERNAME=${3:-"sa"}
PASSWORD=${4:-"d@rio152325"}
MIGRATION_TABLE="DatabaseMigrations"
FORCE=0
DRY_RUN=0

# Mostrar ayuda
show_help() {
    echo "Uso: $0 [servidor] [base_de_datos] [usuario] [contraseña] [opciones]"
    echo ""
    echo "Opciones:"
    echo "  -h, --help          Muestra esta ayuda"
    echo "  -f, --force         Fuerza la ejecución de scripts ya aplicados"
    echo "  -d, --dry-run       Simula la ejecución sin realizar cambios"
    echo ""
    echo "Ejemplo:"
    echo "  $0 localhost AESAN sa MiContraseña"
    exit 0
}

# Procesar argumentos adicionales
shift 4 2>/dev/null || true
while [[ $# -gt 0 ]]; do
    case "$1" in
        -h|--help)
            show_help
            ;;
        -f|--force)
            FORCE=1
            shift
            ;;
        -d|--dry-run)
            DRY_RUN=1
            shift
            ;;
        *)
            echo "Opción desconocida: $1"
            show_help
            ;;
    esac
done

# Configuración de rutas
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DATABASE_ROOT="$(dirname "$SCRIPT_DIR")"
MIGRATIONS_DIRS=("$DATABASE_ROOT/Tables" "$DATABASE_ROOT/StoredProcedures")
LOG_DIR="$DATABASE_ROOT/logs"
LOG_FILE="$LOG_DIR/migrations_$(date +%Y%m%d_%H%M%S).log"

# Crear directorio de logs si no existe
mkdir -p "$LOG_DIR"

# Función para escribir en el log
write_log() {
    local message="$1"
    local level="${2:-INFO}"
    
    local timestamp=$(date +"%Y-%m-%d %H:%M:%S")
    local log_message="[$timestamp] [$level] $message"
    
    # Escribir en el archivo de log
    echo "$log_message" >> "$LOG_FILE"
    
    # Mostrar en la consola con colores según el nivel
    case "$level" in
        "INFO")
            echo -e "${BLUE}$log_message${NC}"
            ;;
        "SUCCESS")
            echo -e "${GREEN}$log_message${NC}"
            ;;
        "WARNING")
            echo -e "${YELLOW}$log_message${NC}"
            ;;
        "ERROR")
            echo -e "${RED}$log_message${NC}"
            ;;
        *)
            echo "$log_message"
            ;;
    esac
}

# Función para ejecutar una consulta SQL
execute_sql_query() {
    local query="$1"
    local output_file="$LOG_DIR/query_result.tmp"
    
    if [ "$DRY_RUN" -eq 1 ]; then
        write_log "DRY RUN: Se ejecutaría la consulta: $query" "INFO"
        return 0
    fi
    
    # Ejecutar la consulta
    /opt/mssql-tools/bin/sqlcmd -S "$SERVER" -d "$DATABASE" -U "$USERNAME" -P "$PASSWORD" -Q "$query" -o "$output_file" -h -1
    
    local exit_code=$?
    
    if [ $exit_code -eq 0 ]; then
        cat "$output_file"
        rm -f "$output_file"
        return 0
    else
        write_log "Error al ejecutar la consulta SQL: $(cat "$output_file")" "ERROR"
        rm -f "$output_file"
        return 1
    fi
}

# Función para ejecutar un script SQL
execute_sql_script() {
    local script_path="$1"
    local script_name=$(basename "$script_path")
    
    if [ "$DRY_RUN" -eq 1 ]; then
        write_log "DRY RUN: Se ejecutaría el script: $script_name" "INFO"
        return 0
    fi
    
    write_log "Ejecutando script: $script_name" "INFO"
    
    # Ejecutar el script usando sqlcmd
    /opt/mssql-tools/bin/sqlcmd -S "$SERVER" -d "$DATABASE" -U "$USERNAME" -P "$PASSWORD" -i "$script_path"
    
    local exit_code=$?
    
    # Verificar si la ejecución fue exitosa
    if [ $exit_code -eq 0 ]; then
        write_log "Script ejecutado correctamente: $script_name" "SUCCESS"
        return 0
    else
        write_log "Error al ejecutar el script: $script_name" "ERROR"
        return 1
    fi
}

# Función para crear la tabla de migraciones si no existe
initialize_migration_table() {
    local query="
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '$MIGRATION_TABLE')
    BEGIN
        CREATE TABLE [$MIGRATION_TABLE] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [ScriptName] NVARCHAR(255) NOT NULL,
            [ScriptPath] NVARCHAR(500) NOT NULL,
            [AppliedOn] DATETIME NOT NULL DEFAULT GETDATE(),
            [Success] BIT NOT NULL,
            [ErrorMessage] NVARCHAR(MAX) NULL
        );
        
        CREATE UNIQUE INDEX [IX_${MIGRATION_TABLE}_ScriptName] ON [$MIGRATION_TABLE] ([ScriptName]);
        
        PRINT 'Tabla de migraciones creada correctamente';
    END
    ELSE
    BEGIN
        PRINT 'La tabla de migraciones ya existe';
    END
    "
    
    if [ "$DRY_RUN" -eq 1 ]; then
        write_log "DRY RUN: Se crearía la tabla de migraciones $MIGRATION_TABLE si no existe" "INFO"
        return 0
    fi
    
    write_log "Inicializando tabla de migraciones $MIGRATION_TABLE..." "INFO"
    execute_sql_query "$query"
    
    if [ $? -eq 0 ]; then
        write_log "Tabla de migraciones inicializada correctamente" "SUCCESS"
        return 0
    else
        write_log "Error al inicializar la tabla de migraciones" "ERROR"
        return 1
    fi
}

# Función para obtener los scripts ya ejecutados
get_applied_migrations() {
    local query="SELECT ScriptName FROM [$MIGRATION_TABLE] WHERE Success = 1"
    local output_file="$LOG_DIR/applied_migrations.tmp"
    
    if [ "$DRY_RUN" -eq 1 ]; then
        write_log "DRY RUN: Se consultarían las migraciones aplicadas" "INFO"
        echo ""
        return 0
    fi
    
    write_log "Obteniendo migraciones aplicadas..." "INFO"
    
    # Ejecutar la consulta
    /opt/mssql-tools/bin/sqlcmd -S "$SERVER" -d "$DATABASE" -U "$USERNAME" -P "$PASSWORD" -Q "$query" -o "$output_file" -h -1
    
    local exit_code=$?
    
    if [ $exit_code -eq 0 ]; then
        # Eliminar líneas en blanco y encabezados
        grep -v "^$" "$output_file" | grep -v "^-" | grep -v "ScriptName" > "$output_file.clean"
        local count=$(wc -l < "$output_file.clean")
        
        write_log "Se encontraron $count migraciones aplicadas" "INFO"
        cat "$output_file.clean"
        
        local result=$(cat "$output_file.clean")
        rm -f "$output_file" "$output_file.clean"
        echo "$result"
        return 0
    else
        write_log "Error al obtener migraciones aplicadas" "ERROR"
        rm -f "$output_file" "$output_file.clean" 2>/dev/null
        echo ""
        return 1
    fi
}

# Función para registrar una migración ejecutada
register_migration() {
    local script_name="$1"
    local script_path="$2"
    local success="$3"
    local error_message="${4:-NULL}"
    
    # Escapar comillas simples en el mensaje de error
    error_message="${error_message//\'/\'\'}"
    
    local success_bit=0
    if [ "$success" -eq 0 ]; then
        success_bit=1
    fi
    
    local query="
    INSERT INTO [$MIGRATION_TABLE] (ScriptName, ScriptPath, AppliedOn, Success, ErrorMessage)
    VALUES ('$script_name', '$script_path', GETDATE(), $success_bit, '$error_message')
    "
    
    if [ "$DRY_RUN" -eq 1 ]; then
        write_log "DRY RUN: Se registraría la migración: $script_name (Éxito: $success_bit)" "INFO"
        return 0
    fi
    
    write_log "Registrando migración: $script_name (Éxito: $success_bit)" "INFO"
    execute_sql_query "$query" > /dev/null
    
    if [ $? -eq 0 ]; then
        write_log "Migración registrada correctamente" "SUCCESS"
        return 0
    else
        write_log "Error al registrar migración" "ERROR"
        return 1
    fi
}

# Función para obtener todos los scripts SQL en los directorios de migración
get_migration_scripts() {
    local all_scripts=()
    
    for dir in "${MIGRATIONS_DIRS[@]}"; do
        if [ -d "$dir" ]; then
            # Buscar recursivamente todos los archivos .sql
            while IFS= read -r -d '' script; do
                all_scripts+=("$script")
            done < <(find "$dir" -type f -name "*.sql" -print0 | sort -z)
        fi
    done
    
    printf '%s\n' "${all_scripts[@]}"
}

# Función para extraer el número de versión de un nombre de script
get_script_version() {
    local script_name="$1"
    local script_basename=$(basename "$script_name")
    
    if [[ $script_basename =~ ^([0-9]+)_ ]]; then
        echo "${BASH_REMATCH[1]}"
    else
        echo "0"
    fi
}

# Función principal para ejecutar las migraciones
run_database_migration() {
    write_log "Iniciando proceso de migración de base de datos..." "INFO"
    write_log "Servidor: $SERVER, Base de datos: $DATABASE" "INFO"
    
    # Inicializar tabla de migraciones
    initialize_migration_table
    if [ $? -ne 0 ] && [ "$DRY_RUN" -eq 0 ]; then
        write_log "Error al inicializar la tabla de migraciones. Abortando." "ERROR"
        return 1
    fi
    
    # Obtener migraciones ya aplicadas
    local applied_migrations=$(get_applied_migrations)
    
    # Obtener todos los scripts de migración
    local all_scripts=($(get_migration_scripts))
    
    write_log "Se encontraron ${#all_scripts[@]} scripts de migración en total" "INFO"
    
    # Filtrar scripts no aplicados y ordenarlos por versión
    local pending_scripts=()
    local ordered_scripts=()
    
    for script in "${all_scripts[@]}"; do
        local script_name=$(basename "$script")
        local is_applied=0
        
        # Verificar si el script ya fue aplicado
        if echo "$applied_migrations" | grep -q "^$script_name$"; then
            is_applied=1
        fi
        
        # Si no está aplicado o se fuerza la ejecución, agregarlo a la lista
        if [ $is_applied -eq 0 ] || [ "$FORCE" -eq 1 ]; then
            pending_scripts+=("$script")
        fi
    done
    
    write_log "Se encontraron ${#pending_scripts[@]} scripts pendientes de aplicar" "INFO"
    
    if [ ${#pending_scripts[@]} -eq 0 ]; then
        write_log "No hay scripts pendientes de aplicar" "SUCCESS"
        return 0
    fi
    
    # Ordenar scripts por versión y nombre
    # Crear un archivo temporal con versión y ruta
    local temp_file="$LOG_DIR/scripts_to_sort.tmp"
    for script in "${pending_scripts[@]}"; do
        local version=$(get_script_version "$script")
        echo "$version|$script" >> "$temp_file"
    done
    
    # Ordenar el archivo y obtener las rutas ordenadas
    while IFS="|" read -r version script; do
        ordered_scripts+=("$script")
    done < <(sort -t"|" -k1,1n -k2,2 "$temp_file")
    
    rm -f "$temp_file"
    
    # Ejecutar scripts pendientes
    local success_count=0
    local error_count=0
    
    for script in "${ordered_scripts[@]}"; do
        local script_name=$(basename "$script")
        
        # Si ya se aplicó y no se fuerza, saltar
        if echo "$applied_migrations" | grep -q "^$script_name$" && [ "$FORCE" -eq 0 ]; then
            write_log "Script ya aplicado, saltando: $script_name" "INFO"
            continue
        fi
        
        # Ejecutar script
        execute_sql_script "$script"
        local exit_code=$?
        
        # Registrar resultado
        if [ $exit_code -eq 0 ]; then
            register_migration "$script_name" "$script" 0
            ((success_count++))
        else
            register_migration "$script_name" "$script" 1 "Error al ejecutar el script"
            ((error_count++))
            
            # Si hay un error, detener el proceso a menos que se fuerce
            if [ "$FORCE" -eq 0 ]; then
                write_log "Error al ejecutar el script $script_name. Deteniendo el proceso." "ERROR"
                break
            fi
        fi
    done
    
    # Resumen
    write_log "Proceso de migración completado" "INFO"
    write_log "Scripts ejecutados correctamente: $success_count" "SUCCESS"
    
    if [ $error_count -gt 0 ]; then
        write_log "Scripts con errores: $error_count" "ERROR"
        return 1
    fi
    
    return 0
}

# Ejecutar migración
run_database_migration
exit $? 