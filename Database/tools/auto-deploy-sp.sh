#!/bin/bash

# =============================================
# Script para Auto-Despliegue de Procedimientos Almacenados
# Autor: Equipo de Desarrollo
# Fecha: 2025-03-07
# =============================================

# Colores para la salida
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuración
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
DATABASE_DIR="$(dirname "$SCRIPT_DIR")"
SP_DIR="$DATABASE_DIR/StoredProcedures"
FLYWAY_DIR="$PROJECT_ROOT/.flyway"
LOCAL_MIGRATIONS_DIR="$FLYWAY_DIR/migrations-local-db"
AZURE_MIGRATIONS_DIR="$FLYWAY_DIR/migrations-azure-db"
LOG_DIR="$DATABASE_DIR/logs"
LOG_FILE="$LOG_DIR/auto_deploy_$(date +%Y%m%d_%H%M%S).log"

# Parámetros
ENVIRONMENT=${1:-"local"}  # Por defecto, usa el entorno local
EXECUTE=${2:-"true"}       # Por defecto, ejecuta los cambios

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

# Función para obtener la versión más alta de las migraciones existentes
get_latest_version() {
    local migrations_dir="$1"
    local latest_version="1.0.0"
    
    if [ -d "$migrations_dir" ]; then
        # Buscar todos los archivos de migración y extraer la versión
        local versions=$(find "$migrations_dir" -name "V*.sql" | sed -E 's/.*V([0-9]+\.[0-9]+\.[0-9]+)__.*/\1/' | sort -V)
        
        # Obtener la última versión
        if [ -n "$versions" ]; then
            latest_version=$(echo "$versions" | tail -n 1)
        fi
    fi
    
    echo "$latest_version"
}

# Función para incrementar la versión
increment_version() {
    local version="$1"
    local increment_type="${2:-patch}"  # Por defecto, incrementa el número de parche
    
    IFS='.' read -r major minor patch <<< "$version"
    
    case "$increment_type" in
        "major")
            major=$((major + 1))
            minor=0
            patch=0
            ;;
        "minor")
            minor=$((minor + 1))
            patch=0
            ;;
        "patch"|*)
            patch=$((patch + 1))
            ;;
    esac
    
    echo "$major.$minor.$patch"
}

# Función para generar un nombre de archivo de migración Flyway
generate_migration_filename() {
    local sp_file="$1"
    local version="$2"
    local sp_name=$(basename "$sp_file" | sed 's/\.sql$//')
    
    # Extraer el número de versión del nombre del SP (si existe)
    local sp_version=""
    if [[ $sp_name =~ ^([0-9]+)_ ]]; then
        sp_version="${BASH_REMATCH[1]}"
        sp_name=$(echo "$sp_name" | sed -E 's/^[0-9]+_//')
    fi
    
    # Generar un nombre descriptivo para la migración
    local description=$(echo "$sp_name" | sed 's/_/ /g' | sed -E 's/([A-Z])/ \1/g' | sed 's/^ //' | sed 's/ /_/g')
    
    echo "V${version}__${description}.sql"
}

# Función para crear un archivo de migración Flyway a partir de un SP
create_migration_file() {
    local sp_file="$1"
    local migrations_dir="$2"
    local version="$3"
    
    # Generar nombre de archivo de migración
    local migration_filename=$(generate_migration_filename "$sp_file" "$version")
    local migration_file="$migrations_dir/$migration_filename"
    
    # Crear el archivo de migración
    cat > "$migration_file" << EOF
-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: $(date +"%Y-%m-%d %H:%M:%S")
-- Archivo original: $sp_file
-- =============================================

$(cat "$sp_file")
GO
EOF
    
    write_log "Creado archivo de migración: $migration_filename" "SUCCESS"
    
    echo "$migration_file"
}

# Función para ejecutar Flyway
run_flyway() {
    local command="$1"
    local environment="$2"
    
    local config_file=""
    case "$environment" in
        "local")
            config_file="$FLYWAY_DIR/flyway-local-db.conf"
            ;;
        "azure")
            config_file="$FLYWAY_DIR/flyway-azure-db.conf"
            ;;
        *)
            write_log "Entorno desconocido: $environment" "ERROR"
            return 1
            ;;
    esac
    
    write_log "Ejecutando: flyway -configFiles=$config_file $command" "INFO"
    
    if [ "$EXECUTE" = "true" ]; then
        # Ejecutar Flyway
        flyway -configFiles="$config_file" $command
        
        if [ $? -eq 0 ]; then
            write_log "Flyway $command ejecutado correctamente" "SUCCESS"
            return 0
        else
            write_log "Error al ejecutar Flyway $command" "ERROR"
            return 1
        fi
    else
        write_log "Simulación: flyway -configFiles=$config_file $command" "INFO"
        return 0
    fi
}

# Función para procesar un directorio de SPs
process_sp_directory() {
    local sp_dir="$1"
    local migrations_dir="$2"
    
    # Obtener la última versión de las migraciones
    local latest_version=$(get_latest_version "$migrations_dir")
    write_log "Última versión de migración: $latest_version" "INFO"
    
    # Buscar todos los archivos SQL en el directorio de SPs
    local sp_files=$(find "$sp_dir" -type f -name "*.sql" | sort)
    
    # Contador para nuevas migraciones
    local new_migrations=0
    
    # Procesar cada archivo SQL
    for sp_file in $sp_files; do
        local sp_name=$(basename "$sp_file")
        
        # Verificar si el SP ya tiene una migración
        local migration_exists=false
        
        # Extraer el contenido del SP
        local sp_content=$(cat "$sp_file")
        
        # Buscar migraciones existentes que contengan el mismo SP
        for migration_file in $(find "$migrations_dir" -type f -name "V*.sql"); do
            local migration_content=$(cat "$migration_file")
            
            # Si el contenido del SP está en la migración, marcar como existente
            if echo "$migration_content" | grep -q "$(echo "$sp_content" | head -n 10)"; then
                migration_exists=true
                write_log "SP ya migrado: $sp_name -> $(basename "$migration_file")" "INFO"
                break
            fi
        done
        
        # Si el SP no tiene migración, crear una nueva
        if [ "$migration_exists" = false ]; then
            # Incrementar la versión
            latest_version=$(increment_version "$latest_version" "patch")
            
            # Crear archivo de migración
            create_migration_file "$sp_file" "$migrations_dir" "$latest_version"
            
            # Incrementar contador
            new_migrations=$((new_migrations + 1))
        fi
    done
    
    echo $new_migrations
}

# Función principal
main() {
    write_log "Iniciando auto-despliegue de procedimientos almacenados..." "INFO"
    write_log "Entorno: $ENVIRONMENT" "INFO"
    
    # Determinar el directorio de migraciones según el entorno
    local migrations_dir=""
    case "$ENVIRONMENT" in
        "local")
            migrations_dir="$LOCAL_MIGRATIONS_DIR"
            ;;
        "azure")
            migrations_dir="$AZURE_MIGRATIONS_DIR"
            ;;
        "both")
            # Procesar para ambos entornos
            local local_migrations=$(process_sp_directory "$SP_DIR" "$LOCAL_MIGRATIONS_DIR")
            local azure_migrations=$(process_sp_directory "$SP_DIR" "$AZURE_MIGRATIONS_DIR")
            
            write_log "Nuevas migraciones creadas para entorno local: $local_migrations" "SUCCESS"
            write_log "Nuevas migraciones creadas para entorno Azure: $azure_migrations" "SUCCESS"
            
            # Ejecutar Flyway para ambos entornos si hay nuevas migraciones
            if [ "$local_migrations" -gt 0 ]; then
                run_flyway "migrate" "local"
            fi
            
            if [ "$azure_migrations" -gt 0 ]; then
                run_flyway "migrate" "azure"
            fi
            
            write_log "Auto-despliegue completado" "SUCCESS"
            return 0
            ;;
        *)
            write_log "Entorno desconocido: $ENVIRONMENT" "ERROR"
            return 1
            ;;
    esac
    
    # Procesar SPs para el entorno seleccionado
    local new_migrations=$(process_sp_directory "$SP_DIR" "$migrations_dir")
    
    write_log "Nuevas migraciones creadas: $new_migrations" "SUCCESS"
    
    # Ejecutar Flyway si hay nuevas migraciones
    if [ "$new_migrations" -gt 0 ]; then
        run_flyway "migrate" "$ENVIRONMENT"
    else
        write_log "No hay nuevos procedimientos almacenados para desplegar" "INFO"
    fi
    
    write_log "Auto-despliegue completado" "SUCCESS"
    return 0
}

# Ejecutar función principal
main
exit $? 