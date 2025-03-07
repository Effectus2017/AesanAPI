# Herramienta de Auto-Despliegue de Procedimientos Almacenados

Esta herramienta permite detectar automáticamente los nuevos procedimientos almacenados (SPs) en tu proyecto y desplegarlos en la base de datos utilizando Flyway. Está diseñada para integrarse con Cursor y facilitar el proceso de desarrollo, ahorrándote tiempo al automatizar la creación de migraciones y su ejecución.

## Características

- Detección automática de nuevos procedimientos almacenados
- Creación automática de archivos de migración Flyway
- Ejecución automática de migraciones en entornos locales y/o Azure
- Versionado semántico de migraciones (major.minor.patch)
- Registro detallado de todas las operaciones
- Soporte para entornos Windows (PowerShell) y Linux/macOS (Bash)
- Modo de simulación (dry-run) para verificar qué migraciones se crearían

## Requisitos

- Flyway instalado y configurado
- Acceso a la base de datos con permisos suficientes para ejecutar procedimientos almacenados
- Estructura de directorios de Flyway existente (`.flyway/migrations-local-db/` y `.flyway/migrations-azure-db/`)

## Uso

### En Windows (PowerShell)

```powershell
# Ejecutar para entorno local
.\auto-deploy-sp.ps1

# Ejecutar para entorno Azure
.\auto-deploy-sp.ps1 -Environment "azure"

# Ejecutar para ambos entornos
.\auto-deploy-sp.ps1 -Environment "both"

# Ejecutar en modo simulación (sin realizar cambios)
.\auto-deploy-sp.ps1 -DryRun
```

### En Linux/macOS (Bash)

```bash
# Dar permisos de ejecución al script
chmod +x auto-deploy-sp.sh

# Ejecutar para entorno local
./auto-deploy-sp.sh local

# Ejecutar para entorno Azure
./auto-deploy-sp.sh azure

# Ejecutar para ambos entornos
./auto-deploy-sp.sh both

# Ejecutar en modo simulación (sin realizar cambios)
./auto-deploy-sp.sh local false
```

## Integración con Cursor

Para integrar esta herramienta con Cursor y automatizar completamente el proceso, puedes configurar un atajo de teclado o un comando personalizado:

1. Abre la configuración de Cursor
2. Busca la sección de comandos personalizados o atajos de teclado
3. Agrega un nuevo comando con la siguiente configuración:

   **Nombre:** Auto-Despliegue de SPs

   **Comando (Windows):**

   ```
   powershell -ExecutionPolicy Bypass -File "${workspaceFolder}/Api/Database/tools/auto-deploy-sp.ps1"
   ```

   **Comando (Linux/macOS):**

   ```
   "${workspaceFolder}/Api/Database/tools/auto-deploy-sp.sh"
   ```

   **Atajo de teclado:** (Elige el que prefieras, por ejemplo, Ctrl+Alt+D)

4. Guarda la configuración

Ahora, cada vez que crees o modifiques un procedimiento almacenado, puedes presionar el atajo de teclado configurado para que Cursor ejecute la herramienta de auto-despliegue, que detectará el nuevo SP, creará una migración Flyway y la ejecutará automáticamente.

### Configuración Avanzada con Script JavaScript

Para una integración más avanzada con Cursor, hemos creado un script JavaScript (`cursor-integration.js`) que puede ser configurado para ejecutarse automáticamente cuando se guarda un archivo SQL en el directorio de procedimientos almacenados.

#### Características del Script de Integración

- Detecta automáticamente si el archivo guardado es un procedimiento almacenado
- Ejecuta el script de auto-despliegue apropiado según el sistema operativo
- Proporciona retroalimentación en la consola sobre el proceso

#### Configuración en Cursor

Para configurar Cursor para ejecutar este script cuando se guarda un archivo SQL:

1. Abra la configuración de Cursor
2. Busque la sección de "File Watchers" o "On Save Actions"
3. Agregue una nueva acción para archivos con patrón `*.sql`
4. Configure la acción para ejecutar: `node /ruta/a/Database/tools/cursor-integration.js $FilePath$`

> **Nota**: La sintaxis exacta puede variar según la versión de Cursor. Consulte la documentación de Cursor para obtener instrucciones específicas sobre cómo configurar acciones al guardar archivos.

#### Uso del Script de Integración

Una vez configurado, el flujo de trabajo es el siguiente:

1. Cree o modifique un procedimiento almacenado en el directorio `Database/StoredProcedures`
2. Guarde el archivo
3. El script de integración detectará automáticamente el cambio y ejecutará el script de auto-despliegue
4. Revise la consola de Cursor para ver el resultado del proceso

Este enfoque proporciona una experiencia de desarrollo más fluida, ya que no es necesario ejecutar manualmente el script de auto-despliegue cada vez que se modifica un procedimiento almacenado.

#### Prueba de la Integración

Para probar la integración sin necesidad de configurar Cursor, hemos creado un script de prueba (`test-integration.js`) que simula la ejecución del script de integración:

```bash
# Windows
node Database\tools\test-integration.js ..\StoredProcedures\NombreDeTuProcedimiento.sql

# Linux/macOS
node Database/tools/test-integration.js ../StoredProcedures/NombreDeTuProcedimiento.sql
```

Este script es útil para verificar que la integración funciona correctamente antes de configurarla en Cursor.

## Flujo de Trabajo

1. Creas o modificas un procedimiento almacenado en el directorio `/Database/StoredProcedures/`
2. Ejecutas la herramienta de auto-despliegue con el atajo de teclado configurado en Cursor
3. La herramienta detecta el nuevo SP y crea un archivo de migración Flyway con el formato `V1.0.x__Nombre_Del_SP.sql`
4. La herramienta ejecuta Flyway para aplicar la migración en la base de datos
5. La próxima vez que ejecutes la herramienta, este SP será omitido porque ya tiene una migración asociada

## Cómo Funciona

1. **Detección**: La herramienta busca todos los archivos SQL en el directorio de procedimientos almacenados.
2. **Comparación**: Para cada SP, verifica si ya existe una migración que contenga el mismo código.
3. **Creación**: Si el SP no tiene una migración asociada, crea un nuevo archivo de migración Flyway con el contenido del SP.
4. **Versionado**: Incrementa automáticamente el número de versión para cada nueva migración.
5. **Ejecución**: Ejecuta Flyway para aplicar las nuevas migraciones en la base de datos.

## Logs

La herramienta genera archivos de log detallados en el directorio `/Database/logs/`. Estos logs incluyen información sobre cada SP procesado, las migraciones creadas y el resultado de la ejecución de Flyway.

## Solución de Problemas

Si encuentras algún problema al usar la herramienta, verifica lo siguiente:

1. Asegúrate de que Flyway está instalado y configurado correctamente.
2. Verifica que los archivos de configuración de Flyway (`.flyway/flyway-local-db.conf` y `.flyway/flyway-azure-db.conf`) existen y contienen la información correcta.
3. Comprueba que tienes permisos suficientes en la base de datos.
4. Revisa los archivos de log para identificar errores específicos.
5. Asegúrate de que los procedimientos almacenados son válidos y no contienen errores de sintaxis.

## Personalización

Si necesitas personalizar la herramienta, puedes modificar los siguientes aspectos:

- **Directorios de búsqueda**: Modifica la variable `$SpDir` para buscar SPs en otros directorios.
- **Formato de versión**: Cambia la función `Increment-Version` para usar un esquema de versionado diferente.
- **Formato de migración**: Modifica la función `New-MigrationFile` para cambiar el formato de los archivos de migración.
- **Configuración de Flyway**: Ajusta las rutas a los archivos de configuración de Flyway según tu entorno.
