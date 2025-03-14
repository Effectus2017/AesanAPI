# Adición de nuevas propiedades a la tabla Agency

Fecha: 2024-05-15
Autor: Equipo de Desarrollo

## Descripción

Se han agregado tres nuevas propiedades a la tabla Agency para mejorar la gestión de información de las agencias:

1. `ServiceTime` (DateTime): Almacena la fecha en que comenzó a operar la organización.
2. `TaxExemptionStatus` (INT): Indica el estado de la exención contributiva (1: En Progreso, 2: Otorgado).
3. `TaxExemptionType` (INT): Indica el tipo de exención contributiva (1: Estatal, 2: Federal).

## Cambios realizados

### Estructura de la base de datos

- Se agregaron las columnas `ServiceTime`, `TaxExemptionStatus` y `TaxExemptionType` a la tabla `Agency`.
- Se actualizaron los registros existentes con valores predeterminados.

### Procedimientos almacenados

- Se actualizó el procedimiento `InsertAgency` para incluir las nuevas propiedades.
- Se actualizó el procedimiento `UpdateAgency` para incluir las nuevas propiedades.
- Se actualizó el procedimiento `GetAgencyById` para incluir las nuevas propiedades en la consulta.
- Se actualizó el procedimiento `GetAgencies` para incluir las nuevas propiedades en la consulta.

### Modelos y repositorios

- Se actualizó el modelo `AgencyRequest` para incluir las nuevas propiedades.
- Se actualizó el modelo `DTOAgency` para incluir las nuevas propiedades.
- Se actualizó el repositorio `AgencyRepository` para manejar las nuevas propiedades.

## Scripts SQL

Los scripts SQL para estos cambios se encuentran en:

- `Api/Database/Tables/Agency/103_AlterTableAgency_AddNewColumns.sql`
- `Api/Database/Tables/Agency/103_UpdateInsertAgency.sql`
- `Api/Database/Tables/Agency/103_UpdateAgency.sql`
- `Api/Database/Tables/Agency/103_GetAgencyById.sql`
- `Api/Database/Tables/Agency/105_GetAgencies.sql`
