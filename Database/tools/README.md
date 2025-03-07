# Herramienta de Migración de Base de Datos para Cursor

Esta herramienta permite ejecutar automáticamente los scripts SQL nuevos o modificados en la base de datos, manteniendo un registro de los scripts ya aplicados. Está diseñada para integrarse con Cursor y facilitar el proceso de desarrollo y despliegue.

## Características

- Detección automática de scripts SQL nuevos
- Ejecución ordenada de scripts basada en el número de versión
- Registro de scripts aplicados en una tabla de la base de datos
- Soporte para entornos Windows (PowerShell) y Linux/macOS (Bash)
- Modo de simulación (dry-run) para verificar qué scripts se ejecutarían
- Opción para forzar la re-ejecución de scripts ya aplicados
- Registro detallado de todas las operaciones

## Requisitos

- SQL Server instalado y configurado
- Herramientas de línea de comandos de SQL Server (mssql-tools) instaladas
- Acceso a la base de datos con permisos suficientes para crear tablas y ejecutar scripts

## Estructura de Directorios

La herramienta busca scripts SQL en los siguientes directorios:

```
/Database/Tables/
/Database/StoredProcedures/
```

## Convención de Nombres

Para que la herramienta funcione correctamente, los scripts SQL deben seguir esta convención de nombres:

```
<número_de_versión>_<descripción>.sql
```

Ejemplos:

- `100_CreateUserTable.sql`
- `101_AddEmailColumn.sql`
- `102_UpdateGetUserProcedure.sql`

El número de versión se utiliza para determinar el orden de ejecución de los scripts.

## Uso

### En Windows (PowerShell)

```powershell
# Ejecutar con parámetros por defecto
.\db-migrator.ps1

# Ejecutar con parámetros personalizados
.\db-migrator.ps1 -Server "miservidor" -Database "mibasededatos" -Username "miusuario" -Password "micontraseña"

# Ejecutar en modo simulación (sin realizar cambios)
.\db-migrator.ps1 -DryRun

# Forzar la re-ejecución de scripts ya aplicados
.\db-migrator.ps1 -Force
```

### En Linux/macOS (Bash)

```bash
# Dar permisos de ejecución al script
chmod +x db-migrator.sh

# Ejecutar con parámetros por defecto
./db-migrator.sh

# Ejecutar con parámetros personalizados
./db-migrator.sh miservidor mibasededatos miusuario micontraseña

# Ejecutar en modo simulación (sin realizar cambios)
./db-migrator.sh localhost AESAN sa micontraseña --dry-run

# Forzar la re-ejecución de scripts ya aplicados
./db-migrator.sh localhost AESAN sa micontraseña --force
```

## Integración con Cursor

Para integrar esta herramienta con Cursor, puedes configurar un atajo de teclado o un comando personalizado que ejecute el script de migración. Esto te permitirá ejecutar automáticamente los nuevos scripts SQL cada vez que los crees o modifiques.

### Configuración en Cursor

1. Abre la configuración de Cursor
2. Busca la sección de comandos personalizados o atajos de teclado
3. Agrega un nuevo comando con la siguiente configuración:

   **Nombre:** Ejecutar Migraciones SQL

   **Comando (Windows):**

   ```
   powershell -ExecutionPolicy Bypass -File "${workspaceFolder}/Database/tools/db-migrator.ps1"
   ```

   **Comando (Linux/macOS):**

   ```
   "${workspaceFolder}/Database/tools/db-migrator.sh"
   ```

   **Atajo de teclado:** (Elige el que prefieras, por ejemplo, Ctrl+Alt+M)

4. Guarda la configuración

Ahora, cada vez que presiones el atajo de teclado configurado, Cursor ejecutará la herramienta de migración, que detectará y aplicará automáticamente los nuevos scripts SQL.

## Tabla de Migraciones

La herramienta crea una tabla llamada `DatabaseMigrations` en la base de datos para registrar los scripts aplicados. Esta tabla tiene la siguiente estructura:

```sql
CREATE TABLE [DatabaseMigrations] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ScriptName] NVARCHAR(255) NOT NULL,
    [ScriptPath] NVARCHAR(500) NOT NULL,
    [AppliedOn] DATETIME NOT NULL DEFAULT GETDATE(),
    [Success] BIT NOT NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL
);
```

## Logs

La herramienta genera archivos de log detallados en el directorio `/Database/logs/`. Estos logs incluyen información sobre cada script ejecutado, errores encontrados y el resultado general del proceso de migración.

## Solución de Problemas

Si encuentras algún problema al usar la herramienta, verifica lo siguiente:

1. Asegúrate de que tienes permisos suficientes en la base de datos.
2. Verifica que la base de datos existe y es accesible.
3. Comprueba que los scripts SQL están en las rutas correctas.
4. Revisa los archivos de log para identificar errores específicos.
5. Asegúrate de que los scripts SQL son válidos y no contienen errores de sintaxis.

## Ejemplo de Flujo de Trabajo

1. Creas un nuevo script SQL en `/Database/StoredProcedures/105_UpdateAgencyStatusOrder.sql`
2. Ejecutas la herramienta de migración con el atajo de teclado configurado en Cursor
3. La herramienta detecta el nuevo script, lo ejecuta y registra en la tabla de migraciones
4. La próxima vez que ejecutes la herramienta, este script será omitido porque ya está registrado como aplicado

Este flujo de trabajo te permite concentrarte en escribir los scripts SQL sin preocuparte por su ejecución manual, ya que la herramienta se encarga de detectar y aplicar automáticamente los cambios.
