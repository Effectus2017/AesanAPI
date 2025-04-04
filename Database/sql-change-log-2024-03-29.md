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
