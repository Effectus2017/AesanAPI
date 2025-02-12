# Changelog de Base de Datos

## [1.0.0] - 2025-02-12

### Añadido

- Creación de tabla `AuditLog` para tracking de cambios
- Función `100_GetCurrentUserId` para obtener el usuario actual
- Triggers de auditoría para la tabla `Agency`:
  - `100_TR_Agency_Insert`
  - `100_TR_Agency_Update`
  - `100_TR_Agency_Delete`
- Procedimiento `100_GetAuditLogHistory` para consultar el historial de cambios

### Modificado

- Actualizado SP `101_UpdateAgency` para incluir el UserId en el contexto de sesión
- Modificada tabla `Agency` para incluir campo `IsActive` para soft delete

### Características

- Tracking automático de cambios en tablas
- Registro de usuario que realiza los cambios
- Historial completo de modificaciones
- Soporte para soft delete
- Consulta de historial con filtros

flyway -configFiles=.flyway/flyway-local-db.conf migrate
flyway -configFiles=.flyway/flyway-azure-db.conf migrate
