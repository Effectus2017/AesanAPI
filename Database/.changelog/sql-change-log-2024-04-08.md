# Cambios en la Base de Datos - 2024-04-08

## Modificaciones en Agency y AgencyInscription

### Nuevas Versiones de Stored Procedures

- 110_InsertAgency
- 110_UpdateAgency
- 110_GetAgencyById
- 110_GetAgencies
- 110_GetAgencyByIdAndUserId
- 110_MigrateAgencyToAgencyInscription

### Cambios Realizados

- Se movieron campos relacionados con la inscripción de Agency a AgencyInscription
- Se agregó relación entre Agency y AgencyInscription
- Se actualizaron todos los SPs para trabajar con la nueva estructura
- Se agregó manejo de transacciones en Insert y Update
- Se creó script de migración para mover datos existentes

### Campos Movidos a AgencyInscription

- NonProfit
- FederalFundsDenied
- StateFundsDenied
- OrganizedAthleticPrograms
- AtRiskService
- BasicEducationRegistry
- ServiceTime
- TaxExemptionStatus
- TaxExemptionType
- IsPropietary

### Notas Adicionales

- Se mantiene compatibilidad con las consultas existentes mediante JOINs
- Se agregó manejo de transacciones para garantizar la integridad de los datos
- Se actualizaron los SPs para incluir los campos de la nueva tabla AgencyInscription
- El script de migración incluye validaciones y manejo de errores
- Se agregó registro de errores en tabla ErrorLog durante la migración

### Orden de Ejecución

1. Crear tabla AgencyInscription (110-AgencyTable.sql)
2. Ejecutar script de migración (110_MigrateAgencyToAgencyInscription.sql)
3. Actualizar los stored procedures
4. Verificar la integridad de los datos migrados
