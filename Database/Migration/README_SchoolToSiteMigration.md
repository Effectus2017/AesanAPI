# Migración de School a Site

## Descripción

Este documento describe la migración completa de la nomenclatura de "Escuelas" (Schools) a "Sitios" (Sites) en el sistema AESAN.

## Archivos de Migración

### 1. Script Principal de Migración

- **Archivo**: `300_SchoolToSiteMigration.sql`
- **Descripción**: Script principal que realiza la migración completa de la base de datos
- **Incluye**:
  - Creación de nuevas tablas Site
  - Migración de datos de School a Site
  - Creación de Foreign Keys
  - Creación de Índices
  - Creación de Stored Procedures
  - Verificación de integridad

### 2. Script de Pruebas

- **Archivo**: `301_TestSiteMigration.sql`
- **Descripción**: Script de pruebas para verificar que la migración fue exitosa
- **Incluye**:
  - Verificación de existencia de tablas
  - Verificación de Stored Procedures
  - Verificación de integridad de datos
  - Pruebas de funcionalidad
  - Verificación de Foreign Keys e Índices

## Instrucciones de Ejecución

### Paso 1: Preparación

1. **Backup de la base de datos**: Realizar un backup completo antes de ejecutar la migración
2. **Entorno de prueba**: Ejecutar primero en un entorno de desarrollo/pruebas
3. **Verificar dependencias**: Asegurar que no hay procesos activos usando las tablas School

### Paso 2: Ejecutar Migración

```sql
-- Ejecutar el script principal de migración
EXEC sp_executesql N'-- Contenido del archivo 300_SchoolToSiteMigration.sql'
```

### Paso 3: Verificar Migración

```sql
-- Ejecutar el script de pruebas
EXEC sp_executesql N'-- Contenido del archivo 301_TestSiteMigration.sql'
```

### Paso 4: Actualizar Backend

1. **Compilar el proyecto**: Asegurar que el backend compile correctamente
2. **Probar endpoints**: Verificar que los nuevos endpoints de Site funcionen
3. **Actualizar frontend**: Coordinar con el equipo de frontend para actualizar las llamadas a la API

## Cambios Realizados

### Base de Datos

- **Tablas creadas**:

  - `Site` (reemplaza `School`)
  - `SiteSatellite` (reemplaza `SchoolSatellite`)
  - `SiteService` (reemplaza `SchoolService`)
  - `SiteEducationLevel` (reemplaza `SchoolEducationLevel`)
  - `SiteDayCareHome` (reemplaza `SchoolDayCareHome`)
  - `SiteStaff` (reemplaza `SchoolStaff`)
  - `SiteParticipant` (reemplaza `SchoolParticipant`)
  - `SiteChildGroup` (reemplaza `SchoolChildGroup`)
  - `SiteOperatingDays` (reemplaza `SchoolOperatingDays`)

- **Stored Procedures creados**:
  - `104_InsertSite`
  - `105_GetSiteById`
  - `104_GetSites`
  - `102_DeleteSite`
  - `104_UpdateSite`
  - `102_HasMainSite`
  - `103_UpdateSiteActiveStatus`

### Backend

- **Controladores**:
  - `SiteController.cs` (reemplaza `SchoolController.cs`)
- **Repositorios**:

  - `SiteRepository.cs` (reemplaza `SchoolRepository.cs`)
  - `ISiteRepository.cs` (reemplaza `ISchoolRepository.cs`)

- **Modelos**:

  - `SiteRequest.cs` (reemplaza `SchoolRequest.cs`)
  - `SiteResponse.cs` (reemplaza `SchoolResponse.cs`)
  - Modelos de Request y Response relacionados

- **Mappers**:

  - `SiteMapper.cs` (reemplaza `SchoolMapper.cs`)
  - Actualización de `MappingService.cs`

- **Configuración**:
  - Actualización de `Program.cs` para registrar nuevos servicios
  - Actualización de `UnitOfWork.cs` e `IUnitOfWork.cs`
  - Actualización de `ApplicationSettings.cs` para claves de caché

### Endpoints de API

- **Antes**:

  - `GET /school/get-school-by-id`
  - `GET /school/get-all-schools-from-db`
  - `POST /school/insert-school`
  - `PUT /school/update-school`
  - `DELETE /school/delete-school`
  - `GET /school/has-main-school`
  - `PUT /school/update-active-status`

- **Después**:
  - `GET /site/get-site-by-id`
  - `GET /site/get-all-sites-from-db`
  - `POST /site/insert-site`
  - `PUT /site/update-site`
  - `DELETE /site/delete-site`
  - `GET /site/has-main-site`
  - `PUT /site/update-active-status`

## Verificación Post-Migración

### 1. Verificar Base de Datos

- Ejecutar el script de pruebas `301_TestSiteMigration.sql`
- Verificar que todos los datos se migraron correctamente
- Confirmar que las Foreign Keys e Índices funcionan

### 2. Verificar Backend

- Compilar el proyecto sin errores
- Probar todos los endpoints de Site
- Verificar que los mappers funcionan correctamente
- Confirmar que la caché se actualiza correctamente

### 3. Verificar Frontend

- Actualizar las llamadas a la API para usar endpoints de Site
- Probar todas las funcionalidades relacionadas con sitios
- Verificar que los formularios funcionan correctamente

## Rollback (En caso de problemas)

### Si es necesario revertir la migración:

1. **Restaurar backup**: Restaurar el backup de la base de datos
2. **Revertir código**: Volver a la versión anterior del backend
3. **Revertir frontend**: Volver a la versión anterior del frontend

### Nota importante:

Las tablas School originales se mantienen durante un período de transición para permitir rollback si es necesario.

## Consideraciones de Rendimiento

- **Índices**: Se crearon índices optimizados para las nuevas tablas Site
- **Caché**: Se actualizaron las claves de caché para incluir Sites
- **Consultas**: Los Stored Procedures están optimizados para rendimiento

## Mantenimiento

### Limpieza Post-Migración

Una vez que se confirme que la migración fue exitosa y el sistema funciona correctamente:

1. **Eliminar tablas School** (después de confirmar que no se necesitan):

   ```sql
   -- Ejecutar solo después de confirmar que la migración fue exitosa
   DROP TABLE SchoolOperatingDays;
   DROP TABLE SchoolChildGroup;
   DROP TABLE SchoolParticipant;
   DROP TABLE SchoolStaff;
   DROP TABLE SchoolDayCareHome;
   DROP TABLE SchoolEducationLevel;
   DROP TABLE SchoolService;
   DROP TABLE SchoolSatellite;
   DROP TABLE School;
   ```

2. **Eliminar Stored Procedures School**:
   ```sql
   -- Eliminar Stored Procedures obsoletos
   DROP PROCEDURE IF EXISTS 104_InsertSchool;
   DROP PROCEDURE IF EXISTS 105_GetSchoolById;
   DROP PROCEDURE IF EXISTS 104_GetSchools;
   DROP PROCEDURE IF EXISTS 102_DeleteSchool;
   DROP PROCEDURE IF EXISTS 104_UpdateSchool;
   DROP PROCEDURE IF EXISTS 102_HasMainSchool;
   DROP PROCEDURE IF EXISTS 103_UpdateSchoolActiveStatus;
   ```

## Contacto y Soporte

Para preguntas o problemas relacionados con esta migración, contactar al equipo de desarrollo.

## Historial de Cambios

- **2025-01-15**: Migración inicial de School a Site
- **Versión**: 1.0
- **Estado**: Completado
