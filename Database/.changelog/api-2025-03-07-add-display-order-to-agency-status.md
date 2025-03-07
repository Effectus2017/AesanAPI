# Adición de Propiedad de Orden a Estados de Agencia

**Fecha:** 2025-03-07  
**Autor:** Equipo de Desarrollo  
**Descripción:** Adición de una propiedad de orden (DisplayOrder) a la tabla AgencyStatus para permitir la organización flexible de los estados.

## Cambios Realizados

### 1. Estructura de la Base de Datos

Se ha añadido una nueva columna `DisplayOrder` a la tabla `AgencyStatus` para permitir la organización flexible de los estados sin depender del Id. Esta columna es de tipo INT y tiene un valor predeterminado de 0.

Los valores iniciales de DisplayOrder se han establecido de la siguiente manera:

- Id 1 (Pendiente a validar): DisplayOrder = 10
- Id 2 (Coordinar Visita Pre-Operacional): DisplayOrder = 20
- Id 3 (Orientación de Programa): DisplayOrder = 30
- Id 4 (Orientación de Contabilidad): DisplayOrder = 40
- Id 5 (Cumple con los requisitos): DisplayOrder = 50
- Id 6 (No cumple con los requisitos): DisplayOrder = 60

### 2. Procedimientos Almacenados

Se han creado o actualizado los siguientes procedimientos almacenados:

- `105_GetAllAgencyStatus`: Actualizado para ordenar los estados por DisplayOrder en lugar de por Id.
- `105_UpdateAgencyStatusDisplayOrder`: Nuevo procedimiento para actualizar el orden de visualización de un estado de agencia.

### 3. Modelo y Repositorio

- Se ha actualizado el modelo `DTOAgencyStatus` para incluir la propiedad `DisplayOrder`.
- Se ha actualizado el repositorio `AgencyStatusRepository` para incluir un nuevo método `UpdateAgencyStatusDisplayOrder`.
- Se ha actualizado la interfaz `IAgencyStatusRepository` para incluir el nuevo método.

### 4. Controlador

Se ha añadido un nuevo endpoint al controlador `AgencyStatusController`:

- `PUT /agency-status/update-agency-status-display-order`: Permite actualizar el orden de visualización de un estado de agencia.

## Scripts SQL

Los siguientes scripts deben ejecutarse en el orden especificado:

1. `/Database/Tables/AgencyStatus/105_AlterTableAgencyStatus_AddOrderColumn.sql` - Añade la columna DisplayOrder a la tabla AgencyStatus.
2. `/Database/StoredProcedures/AgencyStatus/105_UpdateGetAllAgencyStatus.sql` - Actualiza el procedimiento almacenado para obtener todos los estados de agencia, ordenándolos por DisplayOrder.
3. `/Database/StoredProcedures/AgencyStatus/105_UpdateAgencyStatusOrder.sql` - Crea un procedimiento almacenado para actualizar el orden de visualización de un estado de agencia.

## Impacto

Este cambio permite una mayor flexibilidad en la organización de los estados de agencia, ya que ahora se pueden reordenar sin tener que modificar los Ids o las referencias a ellos en otras tablas. Esto facilita la adaptación del sistema a cambios en los procesos de negocio sin requerir migraciones complejas de datos.
