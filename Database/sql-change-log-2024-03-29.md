# Registro de Cambios SQL - 29/03/2024

## Tablas Creadas

- **AgencyFiles**: Tabla para almacenar archivos asociados a agencias
  - Se creó con la estructura básica para gestionar archivos (nombre, ruta, tipo, etc.)
  - Se agregó relación con la tabla Agency
  - Se implementó borrado lógico con el campo IsActive

## Stored Procedures Creados

| Nombre                | Versión | Descripción                                                                                 |
| --------------------- | ------- | ------------------------------------------------------------------------------------------- |
| 100_GetAgencyFiles    | 1.0     | Obtiene los archivos asociados a una agencia, con paginación y filtro por tipo de documento |
| 100_GetAgencyFileById | 1.0     | Obtiene un archivo específico por su ID                                                     |
| 100_InsertAgencyFile  | 1.0     | Inserta un nuevo archivo asociado a una agencia                                             |
| 100_DeleteAgencyFile  | 1.0     | Realiza eliminación lógica de un archivo (marca como inactivo)                              |
| 100_UpdateAgencyFile  | 1.0     | Actualiza la información de un archivo (descripción y tipo de documento)                    |

## Cambios en APIs

- Se creó un nuevo controlador `AgencyFilesController` con endpoints para:
  - Obtener archivos de una agencia
  - Obtener un archivo por ID
  - Agregar un nuevo archivo
  - Actualizar información de un archivo
  - Eliminar un archivo

## Cambios en el Modelo de Datos

- Se añadieron los siguientes modelos:
  - `AgencyFile`: Modelo principal para los archivos de agencia
  - `DTOAgencyFile`: DTO para transferir datos de archivos de agencia
  - `AgencyFileRequest`: Modelo de solicitud para subir o actualizar archivos

## Notas para la Implementación

- La tabla y SPs creados siguen las convenciones de nombres establecidas
- Se implementó el patrón de repositorio para gestionar los archivos
- Se registró el nuevo repositorio en el DI container
- Cuando se añada código frontend, se deberá implementar un componente para mostrar y gestionar los archivos asociados a cada agencia

# Cambios en la Base de Datos - 29 de marzo de 2024

## Cambios en la tabla Agency

### Nuevo Campo

- Se agregó el campo `BasicEducationRegistry` (INT) para almacenar el estado del Registro de Educación Básica
  - Valores posibles:
    - 0 = No definido
    - 1 = Otorgado
    - 2 = En Proceso
    - 3 = No

### Stored Procedures Actualizados

1. `109_AlterTableAgency_AddBasicEducationRegistry.sql`

   - Agrega el nuevo campo `BasicEducationRegistry` a la tabla Agency
   - Actualiza los registros existentes con valor por defecto
   - Agrega documentación del campo

2. `109_InsertAgency.sql`

   - Actualizado para incluir el nuevo campo `BasicEducationRegistry` en la inserción de agencias

3. `109_UpdateAgency.sql`

   - Actualizado para incluir el nuevo campo `BasicEducationRegistry` en la actualización de agencias

4. `109_GetAgencyById.sql`

   - Actualizado para incluir el nuevo campo `BasicEducationRegistry` en la consulta de agencia por ID

5. `109_GetAgencies.sql`
   - Actualizado para incluir el nuevo campo `BasicEducationRegistry` en la consulta de todas las agencias

## Orden de Ejecución

1. Ejecutar `109_AlterTableAgency_AddBasicEducationRegistry.sql`
2. Ejecutar `109_InsertAgency.sql`
3. Ejecutar `109_UpdateAgency.sql`
4. Ejecutar `109_GetAgencyById.sql`
5. Ejecutar `109_GetAgencies.sql`

## Notas Adicionales

- El campo `BasicEducationRegistry` es nullable con valor por defecto 0
- Se mantiene compatibilidad con los stored procedures existentes
- Se agregó documentación del campo usando extended properties

## Cambios en Tablas

### Agency

- Se agregó la columna `BasicEducationRegistry` de tipo INT para almacenar el estado del registro de educación básica
- Se agregó la columna `ServiceTime` de tipo DATETIME para almacenar la fecha de inicio de servicios
- Se agregó la columna `TaxExemptionStatus` de tipo INT para almacenar el estado de la exención contributiva
- Se agregó la columna `TaxExemptionType` de tipo INT para almacenar el tipo de exención contributiva
- Se modificó el tipo de dato de las columnas `ZipCode` y `PostalZipCode` de INT a NVARCHAR(20)

## Cambios en Stored Procedures

### Agency

#### Versión 109

1. `109_GetAgencyById`

   - Se agregaron los nuevos campos:
     - BasicEducationRegistry
     - ServiceTime
     - TaxExemptionStatus
     - TaxExemptionType
   - Se actualizaron los tipos de datos:
     - ZipCode: NVARCHAR(20)
     - PostalZipCode: NVARCHAR(20)

2. `109_GetAgencies`

   - Se agregaron los nuevos campos:
     - BasicEducationRegistry
     - ServiceTime
     - TaxExemptionStatus
     - TaxExemptionType
   - Se actualizaron los tipos de datos:
     - ZipCode: NVARCHAR(20)
     - PostalZipCode: NVARCHAR(20)
   - Se actualizó la tabla temporal #TempResults con los nuevos campos

3. `109_GetAgencyByIdAndUserId`

   - Se agregaron los nuevos campos:
     - BasicEducationRegistry
     - ServiceTime
     - TaxExemptionStatus
     - TaxExemptionType
   - Se actualizaron los tipos de datos:
     - ZipCode: NVARCHAR(20)
     - PostalZipCode: NVARCHAR(20)

4. `109_InsertAgency`

   - Se agregaron los nuevos campos:
     - BasicEducationRegistry
     - ServiceTime
     - TaxExemptionStatus
     - TaxExemptionType
   - Se actualizaron los tipos de datos:
     - ZipCode: NVARCHAR(20)
     - PostalZipCode: NVARCHAR(20)
   - Se eliminaron los campos no utilizados:
     - Comment
     - AppointmentCoordinated
     - AppointmentDate

5. `109_UpdateAgency`
   - Se agregaron los nuevos campos:
     - BasicEducationRegistry
     - ServiceTime
     - TaxExemptionStatus
     - TaxExemptionType
   - Se actualizaron los tipos de datos:
     - ZipCode: NVARCHAR(20)
     - PostalZipCode: NVARCHAR(20)
   - Se eliminaron los campos no utilizados:
     - Comment
     - AppointmentCoordinated
     - AppointmentDate

## Notas Adicionales

- Los campos de appointment (AppointmentCoordinated y AppointmentDate) se mantienen en la tabla AgencyProgram ya que son específicos para cada programa
- El campo Comment se ha renombrado a RejectionJustification para mejor claridad
- Los nuevos campos son requeridos para el proceso de registro de agencias
- Los tipos de datos NVARCHAR(20) para códigos postales permiten formatos más flexibles
