# Actualización del Orden de Estados de Agencia

**Fecha:** 2025-03-07  
**Autor:** Equipo de Desarrollo  
**Descripción:** Actualización del orden y nombres de los estados de agencia según los requisitos especificados.

## Cambios Realizados

### 1. Estructura de la Base de Datos

Se ha actualizado la tabla `AgencyStatus` con el siguiente orden de estados:

1. Pendiente a validar (estado por defecto cuando una agencia se registra)
2. Coordinar Visita Pre-Operacional
3. Orientación de Programa
4. Orientación de Contabilidad
5. Cumple con los requisitos
6. No cumple con los requisitos

### 2. Procedimientos Almacenados

Se han actualizado los siguientes procedimientos almacenados:

- `104_GetAllAgencyStatus`: Actualizado para ordenar los estados por Id, manteniendo el orden especificado.
- `104_UpdateAgencyStatus`: Actualizado para validar que el estado exista y manejar correctamente la justificación de rechazo.

### 3. Migración de Datos

Se ha creado un script para actualizar las referencias a los estados de agencia en la tabla `Agency`, mapeando los estados antiguos a los nuevos:

- Estado 1: 'Pendiente de validar' -> 1: 'Pendiente a validar'
- Estado 2: 'Orientación' -> 3: 'Orientación de Programa'
- Estado 3: 'Visita Pre-operacional' -> 2: 'Coordinar Visita Pre-Operacional'
- Estado 4: 'No cumple con los requisitos' -> 6: 'No cumple con los requisitos'
- Estado 5: 'Cumple con los requisitos' -> 5: 'Cumple con los requisitos'
- Estado 6: 'Rechazado' -> 6: 'No cumple con los requisitos'
- Estado 7: 'Aprobado' -> 5: 'Cumple con los requisitos'

## Scripts SQL

Los siguientes scripts deben ejecutarse en el orden especificado:

1. `/Database/Tables/AgencyStatus/104_UpdateAgencyStatusOrder.sql` - Actualiza la tabla AgencyStatus con el nuevo orden.
2. `/Database/StoredProcedures/AgencyStatus/104_UpdateGetAllAgencyStatus.sql` - Actualiza el procedimiento almacenado para obtener todos los estados de agencia.
3. `/Database/StoredProcedures/AgencyStatus/104_UpdateAgencyStatusProcedure.sql` - Actualiza el procedimiento almacenado para actualizar el estado de una agencia.
4. `/Database/Tables/Agency/104_UpdateAgencyStatusReferences.sql` - Actualiza las referencias a los estados de agencia en la tabla Agency.

## Impacto

Este cambio afecta a todas las agencias existentes en el sistema, actualizando sus estados según el nuevo orden y nomenclatura. También afecta a la forma en que se muestran y gestionan los estados de agencia en la interfaz de usuario.
