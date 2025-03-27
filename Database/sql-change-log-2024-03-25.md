# Cambios en SQL - 25 de marzo de 2024

## Stored Procedures

### Usuarios

- Se creó `108_GetAllUsersFromDb.sql`:
  - Nueva versión del SP que usa la tabla `AgencyUsers` en lugar de `AspNetUsers.AgencyId`
  - Mantiene la misma funcionalidad pero actualiza la relación usuario-agencia
  - Usa `LEFT JOIN` con `AgencyUsers` para obtener la relación
  - Mantiene la paginación y búsqueda original

### Agencias

- Se creó `108_GetAgencies.sql`:
  - Nueva versión del SP que usa la tabla `AgencyUsers` en lugar de `AspNetUsers.AgencyId`
  - Mantiene la misma funcionalidad y estructura de la versión 107
  - Usa `LEFT JOIN` con `AgencyUsers` para obtener los usuarios owner y monitor
  - Mantiene los mismos nombres de propiedades y estructura de la consulta
  - Actualiza solo la relación con usuarios usando `AgencyUsers` en lugar de `AspNetUsers.AgencyId`
  - Mantiene los parámetros originales: `@take`, `@skip`, `@name`, `@regionId`, `@cityId`, `@programId`, `@statusId`, `@userId`, `@alls`
  - Retorna dos conjuntos de resultados:
    1. Datos de las agencias con información de usuarios owner y monitor
    2. Conteo total de registros
