# Changelog

# Lista de Tareas Realizadas

## [2024-09-01] y [2024-10-29]

### Cambios Realizados:

- **Archivos de Configuración:**

  - Se añadió un archivo `.gitignore` con configuraciones para ignorar archivos temporales y de compilación específicos de Visual Studio y otras herramientas de desarrollo.

- **Proyecto API:**

  - Se creó el archivo de proyecto `Api.csproj` configurando el SDK de .NET, versiones de paquetes y propiedades del proyecto como el marco de trabajo objetivo y la habilitación de usos implícitos.

- **Solución Visual Studio:**

  - Se configuró el archivo de solución `Api.sln` para incluir el proyecto API y definir configuraciones de compilación.

- **Controladores:**

  - Se implementó `AuthController` con métodos para registro y login, utilizando un repositorio de autenticación y un patrón UnitOfWork.

- **Contexto de Datos:**

  - Se configuró `ApplicationDbContext` para la gestión de usuarios y roles con Entity Framework.
  - Se añadió `DapperContext` para operaciones de base de datos utilizando Dapper.

- **Interfaces y Repositorios:**

  - Se definieron interfaces como `IAuthRepository` y `IUnitOfWork`.
  - Se implementaron repositorios correspondientes para manejar la lógica de autenticación y la persistencia de datos.

- **Modelos:**

  - Se crearon modelos para la autenticación (`RegisterModel`, `LoginModel`) y para la gestión de roles y usuarios.

- **Servicios:**

  - Se implementó `EmailService` para enviar correos electrónicos utilizando SendGrid.

- **Configuración de Inicio:**

  - Se configuró el archivo `Startup.cs` para la inicialización de servicios, middleware, autenticación JWT y Swagger.

- **Estructura de Directorios:**

  - Se definió la estructura de directorios del proyecto en `api-structure.txt`, organizando los controladores, modelos, repositorios, servicios y configuraciones.

- **Configuraciones de Desarrollo:**
  - Se añadieron configuraciones específicas de desarrollo en `appsettings.Development.json` para la conexión a la base de datos y niveles de log.

Estos cambios establecen la base inicial del proyecto API, configurando el entorno de desarrollo, dependencias, y estructura básica para la autenticación y gestión de usuarios.

## [2024-10-29]

### Cambios Realizados:

- **Archivos de Configuración:**

  - Se añadió el archivo `.hintrc` con configuraciones para la validación de código.
  - Se actualizaron las configuraciones en `.vscode/launch.json` para el lanzamiento de .NET Core.
  - Se añadieron configuraciones en `.vscode/settings.json` para mejorar la integración con Visual Studio Code.
  - Se configuraron tareas en `.vscode/tasks.json` para comandos de construcción y publicación.

- **Controladores:**

  - Se añadió `AgencyController` con métodos para obtener todas las agencias, obtener una agencia por ID y obtener el estado de todas las agencias.
  - Se modificó `AuthController` para utilizar `IUnitOfWork` y mejorar la gestión de autenticación.
  - Se añadió `GeoController` con métodos para obtener ciudades y regiones.
  - Se añadió `ProgramController` con métodos para obtener programas.
  - Se añadió `UserController` con métodos para la gestión de usuarios y agencias.

- **Modelos y DTOs:**

  - Se añadieron clases DTO para agencias, estados de agencias, ciudades, regiones, programas, roles y usuarios.
  - Se añadieron modelos de solicitud para login, registro de agencia y registro de usuario-agencia.

- **Repositorios:**

  - Se añadió `AgencyRepository` para la gestión de datos de agencias.
  - Se añadió `GeoRepository` para la gestión de datos geográficos.
  - Se añadió `ProgramRepository` para la gestión de programas.
  - Se añadió `UserRepository` para la gestión de usuarios.

- **Servicios:**

  - Se añadió `EmailService` con métodos para enviar correos electrónicos y contraseñas temporales.

- **Configuración de Inicio y Middleware:**

  - Se actualizaron las configuraciones de middleware para desarrollo y producción.
  - Se añadió configuración para CORS y Swagger.

- **Utilidades:**

  - Se añadió una clase `Utilities` para manejar errores y generar contraseñas temporales.

- **Base de Datos:**

  - Se añadieron procedimientos almacenados para la gestión de ciudades, regiones, programas y agencias.
  - Se actualizaron las tablas y se añadieron nuevas tablas para soportar los nuevos modelos y funcionalidades.

- **Interfaces:**

  - Se añadieron nuevas interfaces para los repositorios y servicios añadidos.

- **Eliminaciones:**
  - Se eliminó la interfaz `IUnitOfWork` y la clase `AuthRepository`.
  - Se eliminó el modelo `AuthModels`.

Estos cambios amplían significativamente la funcionalidad del sistema, mejorando la gestión de datos y la configuración del entorno de desarrollo.

## [2024-10-30]

### Descripción de los cambios:

- **Procedimientos Almacenados:**

  - Se cambió `ALTER PROCEDURE [100_InsertAgency]` a `CREATE OR ALTER PROCEDURE [100_InsertAgency]` para permitir la creación o modificación del procedimiento.
  - Se añadieron nuevos parámetros `@SdrNumber`, `@UieNumber`, `@EinNumber`, `@ZipCode`, y `@PostalAddress` al procedimiento `100_InsertAgency`.
  - Se actualizó la inserción en la tabla `Agency` para incluir los nuevos campos `SdrNumber`, `UieNumber`, `EinNumber`, `ZipCode`, y `PostalAddress`.

- **Modelos:**

  - Se añadieron las propiedades `SdrNumber`, `UieNumber`, `EinNumber`, `ZipCode`, y `PostalAddress` en el modelo `DTOAgency`.
  - Se actualizó el modelo `AgencyRequest` para incluir los nuevos campos `CityId`, `RegionId`, `ZipCode`, y `PostalAddress`.

- **Repositorios:**
  - Se actualizaron los métodos en `AgencyRepository` para manejar los nuevos campos de la agencia.
  - Se añadieron los nuevos parámetros de elegibilidad en el `UserRepository` para manejar los datos de `SdrNumber`, `UieNumber`, `EinNumber`, `ZipCode`, y `PostalAddress`.

Estos cambios mejoran la gestión de agencias al incluir nuevos campos, permitiendo un registro más completo de la información de las agencias.

## [2024-10-31]

### Descripción de los cambios:

- **Procedimientos Almacenados:**

  - Se cambió `ALTER PROCEDURE [100_InsertAgency]` a `CREATE OR ALTER PROCEDURE [100_InsertAgency]` para permitir la creación o modificación del procedimiento.
  - Se añadieron nuevos parámetros `@NonProfit`, `@FederalFundsDenied`, y `@StateFundsDenied` al procedimiento `100_InsertAgency`.
  - Se actualizó la inserción en la tabla `Agency` para incluir los nuevos campos `NonProfit`, `FederalFundsDenied`, y `StateFundsDenied`.

- **Modelos:**

  - Se añadieron las propiedades `NonProfit`, `FederalFundsDenied`, y `StateFundsDenied` en el modelo `AgencyRequest`.

- **Repositorios:**
  - Se añadieron los nuevos parámetros de elegibilidad en el `UserRepository` para manejar los datos de `NonProfit`, `FederalFundsDenied`, y `StateFundsDenied`.

Estos cambios mejoran la gestión de agencias al incluir nuevos campos de elegibilidad, permitiendo un registro más completo de la información de las agencias.

## [2024-10-01]

### Descripción de los cambios:

- **Seguridad y Autenticación:**

  - Se implementó la autorización en los controladores de agencias, geografía, programas y usuarios utilizando JWT.
  - Se agregó un nuevo esquema de autenticación para asegurar los endpoints.

- **Gestión de Usuarios:**

  - Se introdujo un sistema para manejar contraseñas temporales, incluyendo la creación y eliminación de contraseñas temporales en la base de datos.
  - Se mejoró el registro de usuarios, permitiendo la asignación de roles y el envío de correos electrónicos con contraseñas temporales.

- **Servicios de Email:**

  - Se agregó la capacidad de enviar correos electrónicos utilizando Gmail a través de MailKit.
  - Se implementó un nuevo servicio de correo electrónico que incluye métodos para enviar correos con contraseñas temporales.

- **Base de Datos:**

  - Se crearon nuevos procedimientos almacenados para insertar y eliminar contraseñas temporales.
  - Se realizaron cambios en la tabla `Agency`, incluyendo la adición de un campo `PostalAddress` y la modificación de `PostalCode` a `ZipCode`.
  - Se creó una agencia central por defecto para AESAN.

- **Interfaces:**

  - Se actualizaron las interfaces para incluir nuevos métodos relacionados con la gestión de contraseñas temporales y la actualización del estado de las agencias.

- **Configuración:**

  - Se añadieron configuraciones para el envío de correos electrónicos a través de Gmail en `appsettings.json` y `appsettings.Development.json`.

- **Mejoras Generales:**
  - Se realizaron mejoras en el manejo de errores y en la estructura del código para una mejor legibilidad y mantenimiento.

Estos cambios mejoran la seguridad, la gestión de usuarios y la funcionalidad del sistema, facilitando la comunicación a través de correos electrónicos y optimizando la gestión de agencias y programas.

## [2024-11-02]

### Descripción de los cambios:

- **Controladores:**

  - Se agregó el método `UpdateAgency` en `AgencyController` para actualizar los datos de una agencia.
  - Se agregó el método `UpdateAgencyLogo` en `AgencyController` para actualizar el logo de una agencia.
  - Se modificó el método `UpdateAgencyStatus` para incluir justificación de rechazo.

- **Procedimientos Almacenados:**

  - Se crearon o modificaron los procedimientos almacenados:
    - `100_UpdateAgency` para actualizar los datos de la agencia, incluyendo justificación de rechazo y logo.
    - `100_UpdateAgencyLogo` para actualizar el logo de la agencia.
    - `100_UpdateAgencyStatus` para actualizar el estado de la agencia con justificación de rechazo.
    - Se añadieron campos `ImageURL` y `RejectionJustification` en la tabla `Agency`.

- **Interfaces:**

  - Se actualizaron los métodos en `IAgencyRepository` para incluir `UpdateAgency` y `UpdateAgencyLogo`.

- **Modelos:**

  - Se añadieron propiedades `ImageUrl` y `RejectionJustification` en `AgencyRequest` y `QueryParameters`.
  - Se actualizó el modelo `DTOAgency` para incluir `ImageURL`.

- **Repositorios:**

  - Se implementaron los métodos `UpdateAgency` y `UpdateAgencyLogo` en `AgencyRepository`.

- **Validaciones:**

  - Se mejoró la validación de modelos en los métodos de actualización de agencias.

- **Manejo de Errores:**
  - Se implementó un mejor manejo de errores en los métodos de actualización.

Estos cambios mejoran la funcionalidad de gestión de agencias, permitiendo actualizaciones más completas y un mejor manejo de errores.

## [2024-11-04]

### Cambios Realizados:

- **Procedimientos Almacenados:**

  - Se modificó el procedimiento almacenado `100_InsertAgency` para permitir la inserción de agencias con nuevos campos:

    - Se añadieron los parámetros `@Address`, `@ZipCode`, `@PostalAddress`, `@PostalZipCode`, `@PostalCityId`, y `@PostalRegionId`.
    - Se actualizó la inserción en la tabla `Agency` para incluir los nuevos campos de dirección física y postal.

  - Se creó el procedimiento almacenado `100_InsertAgencyProgram` para insertar programas asociados a una agencia.

  - Se modificó el procedimiento almacenado `100_GetAgencies` para incluir la obtención de programas asociados a las agencias.

  - Se creó el procedimiento almacenado `100_GetAgencyById` para obtener detalles de una agencia por su ID, incluyendo información de programas asociados.

  - Se creó el procedimiento almacenado `100_GetAllAgencyStatus` para obtener todos los estados de agencia.

  - Se actualizó el procedimiento almacenado `100_UpdateAgency` para incluir nuevos campos de dirección postal.

- **Modelos:**

  - Se actualizaron los modelos `DTOAgency` y `AgencyRequest` para incluir propiedades relacionadas con la dirección postal y programas asociados.

- **Interfaces:**

  - Se actualizaron las interfaces en `IAgencyRepository` para incluir métodos para insertar programas de agencia y obtener detalles de agencias.

- **Repositorios:**

  - Se implementaron los métodos en `AgencyRepository` para manejar la inserción de programas asociados a agencias y la obtención de detalles de agencias con sus programas.

Estos cambios mejoran la funcionalidad del sistema al permitir la gestión de múltiples programas asociados a las agencias, así como una mejor organización de la información de dirección.
