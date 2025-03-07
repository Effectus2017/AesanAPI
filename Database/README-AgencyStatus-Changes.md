# Instrucciones para Ejecutar los Cambios en AgencyStatus

Este documento proporciona instrucciones detalladas sobre cómo ejecutar los scripts de cambios en la tabla AgencyStatus y sus procedimientos almacenados relacionados.

## Requisitos Previos

- SQL Server instalado y configurado
- Herramientas de línea de comandos de SQL Server (mssql-tools) instaladas
- Acceso a la base de datos AESAN con permisos suficientes para modificar tablas y procedimientos almacenados

## Opciones de Ejecución

Hay dos scripts disponibles para ejecutar los cambios:

1. **Script Bash (para entornos Linux/macOS)**: `deploy-agency-status-changes.sh`
2. **Script PowerShell (para entornos Windows)**: `deploy-agency-status-changes.ps1`

## Ejecución en Linux/macOS

1. Asegúrate de que el script tenga permisos de ejecución:

   ```bash
   chmod +x deploy-agency-status-changes.sh
   ```

2. Ejecuta el script con los parámetros de conexión:

   ```bash
   ./deploy-agency-status-changes.sh [servidor] [base_de_datos] [usuario] [contraseña]
   ```

   Por ejemplo:

   ```bash
   ./deploy-agency-status-changes.sh localhost AESAN sa MiContraseña
   ```

   Si no se proporcionan parámetros, se utilizarán los valores predeterminados:

   - Servidor: localhost
   - Base de datos: AESAN
   - Usuario: sa
   - Contraseña: YourPassword

## Ejecución en Windows

1. Abre PowerShell como administrador.

2. Navega hasta el directorio donde se encuentra el script:

   ```powershell
   cd C:\ruta\a\Api\Database
   ```

3. Ejecuta el script con los parámetros de conexión:

   ```powershell
   .\deploy-agency-status-changes.ps1 -Server "servidor" -Database "base_de_datos" -Username "usuario" -Password "contraseña"
   ```

   Por ejemplo:

   ```powershell
   .\deploy-agency-status-changes.ps1 -Server "localhost" -Database "AESAN" -Username "sa" -Password "MiContraseña"
   ```

   Si no se proporcionan parámetros, se utilizarán los valores predeterminados:

   - Servidor: localhost
   - Base de datos: AESAN
   - Usuario: sa
   - Contraseña: YourPassword

## Ejecución Manual con sqlcmd

Si prefieres ejecutar los scripts manualmente, puedes utilizar el comando `sqlcmd` directamente:

```bash
sqlcmd -S servidor -d base_de_datos -U usuario -P contraseña -i ruta/al/script.sql
```

Los scripts deben ejecutarse en el siguiente orden:

1. `/Database/Tables/AgencyStatus/104_UpdateAgencyStatusOrder.sql`
2. `/Database/StoredProcedures/AgencyStatus/104_UpdateGetAllAgencyStatus.sql`
3. `/Database/StoredProcedures/AgencyStatus/104_UpdateAgencyStatusProcedure.sql`
4. `/Database/Tables/Agency/104_UpdateAgencyStatusReferences.sql`
5. `/Database/Tables/AgencyStatus/105_AlterTableAgencyStatus_AddOrderColumn.sql`
6. `/Database/StoredProcedures/AgencyStatus/105_UpdateGetAllAgencyStatus.sql`
7. `/Database/StoredProcedures/AgencyStatus/105_UpdateAgencyStatusOrder.sql`

## Verificación

Después de ejecutar los scripts, puedes verificar que los cambios se hayan aplicado correctamente ejecutando las siguientes consultas:

```sql
-- Verificar la estructura de la tabla AgencyStatus
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AgencyStatus';

-- Verificar los valores de la tabla AgencyStatus
SELECT Id, Name, DisplayOrder
FROM AgencyStatus
ORDER BY DisplayOrder;

-- Verificar la existencia de los procedimientos almacenados
SELECT name, create_date, modify_date
FROM sys.objects
WHERE type = 'P' AND name LIKE '%AgencyStatus%';
```

## Solución de Problemas

Si encuentras algún error durante la ejecución de los scripts, verifica lo siguiente:

1. Asegúrate de que tienes permisos suficientes en la base de datos.
2. Verifica que la base de datos AESAN existe y es accesible.
3. Comprueba que los scripts SQL están en las rutas correctas.
4. Revisa los mensajes de error para identificar el problema específico.

Si necesitas ayuda adicional, contacta al equipo de desarrollo.
