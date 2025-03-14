# SQL Change Log

## 2024-02-25

### Added

- Nuevo Stored Procedure:
  - `103_GetAgencies`: Extiende `102_GetAgencies` para filtrar agencias por usuario asignado a través de la tabla `UserAgencyAssignment`

### Modified

- Mejorada la lógica de filtrado en `103_GetAgencies` para incluir agencias asignadas directamente a un usuario a través de `UserAgencyAssignment`
- Agregado soporte para mostrar programas asociados a las agencias asignadas al usuario

## 2024-02-24

### Added

- Nueva tabla `UserAgencyAssignment` para gestionar la asignación de agencias a usuarios monitores
- Nuevos Stored Procedures:
  - `100_AssignAgencyToUser`: Para asignar una agencia a un usuario
  - `100_UnassignAgencyFromUser`: Para desasignar una agencia de un usuario
  - `100_GetUserAssignedAgencies`: Para obtener las agencias asignadas a un usuario

### Modified

- `100_GetAllProgramInscriptions` (V1.0.1):
  - Agregado parámetro `@userId` para filtrar por usuario monitor
  - Agregado JOIN con tabla `UserAgencyAssignment` para filtrar agencias asignadas
- `100_GetAllProgramInscriptions` (V1.0.2):
  - Agregado parámetro `@alls` para permitir obtener todas las inscripciones sin filtrar por asignación de usuario
  - Mejorada la lógica de filtrado para mostrar todas las agencias cuando @alls=1
- `100_GetAllProgramInscriptions` (V1.0.3):
  - Corregida la referencia a la tabla AgencyStatus en lugar de Status que no existe
  - Ajustado el alias de la tabla AgencyStatus para evitar conflictos con la palabra reservada AS
