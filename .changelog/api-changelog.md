# Changelog

# Lista de Tareas Realizadas

## [2024-09-01] hasta [2024-10-28]

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

## [2024-11-01]

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

## [2024-11-05]

### Base de Datos

#### Modificaciones en Stored Procedures

- Se eliminó el campo `ProgramId` del procedimiento `100_InsertAgency`
- Se actualizaron los parámetros opcionales en `100_InsertAgency`:
  - `@RejectionJustification` ahora es NULL por defecto
  - `@ImageURL` ahora es NULL por defecto
- Se corrigió el nombre de la tabla en `100_InsertAgencyProgram` de `AgencyPrograms` a `AgencyProgram`
- Se añadió filtro `IsListable = 1` en `100_GetAgencies`
- Se agregó el campo `AgencyId` en la consulta de programas
- Se eliminó la join con la tabla `Program` en `100_GetAgencyById`
- Se implementó nuevo SP `100_DeleteAgency` para eliminar agencias y sus programas asociados

#### Modificaciones en Tablas

- Se añadieron nuevos campos a la tabla `Agency`:
  - `IsActive` (BIT, NULL, DEFAULT 1)
  - `IsListable` (BIT, NULL, DEFAULT 1)
- Se actualizó el script de inserción de AESAN para incluir los nuevos campos

### Código

#### Interfaces

- Se añadió método `DeleteAgency` en `IAgencyRepository`

#### Modelos

- Se agregó propiedad `AgencyId` en `DTOProgram`
- Se añadieron nuevos campos en `AgencyRequest`:
  - `IsActive` (default: true)
  - `IsListable` (default: true)

#### Repositorios

- Se actualizó `AgencyRepository`:
  - Mejora en el manejo de campos nulos para `PostalCity` y `PostalRegion`
  - Optimización en la asignación de programas a agencias
  - Implementación del método `DeleteAgency`
- Se actualizó `UserRepository`:
  - Se añadió contraseña temporal fija "9c272156" en modo DEBUG
  - Se renombró `DeleteUserByEmail` a `RemoveUserAndAgencyRelatedDataByEmail`
  - Se mejoró el proceso de eliminación de usuarios incluyendo datos relacionados

### Mejoras Generales

- Optimización en el manejo de programas asociados a agencias
- Mejor manejo de campos nulos en consultas
- Implementación de logging más detallado
- Mejoras en el manejo de errores y eliminación de datos relacionados

## [2024-11-06]

### Cambios en Envío de Correos

### Interfaces

- Se añadió el uso de `Api.Models` en `IEmailService.cs`.
- Se agregó el método `SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword)` en `IEmailService`.

### Modelos

- Se añadió la propiedad `EmailToDev` en `GmailSettings` en `ApplicationSettings.cs`.

### Repositorios

- Se actualizó `UserRepository.cs`:
  - Se corrigió el mensaje de log para la inserción de la contraseña temporal.
  - Se modificó el envío de correo para utilizar `SendWelcomeAgencyEmail` en lugar de `SendTemporaryPasswordEmail`.

### Servicios

- Se actualizó `EmailService.cs`:
  - Se añadió el logger para registrar el envío de correos.
  - Se implementó el método `SendWelcomeAgencyEmail` para enviar un correo de bienvenida a la agencia.
  - Se implementaron métodos adicionales para enviar correos de confirmación de aprobación y denegación de auspiciador.

### Configuración

- Se actualizaron los archivos `appsettings.Development.json` y `appsettings.json` para incluir configuraciones de Gmail, como `EmailFrom`, `SmtpServer`, `SmtpServerPort`, `SmtpUser`, `SmtpPass`, y `EmailToDev`.

Estos cambios mejoran la funcionalidad del sistema al optimizar el envío de correos electrónicos, incluyendo correos de bienvenida y confirmación para los usuarios de la agencia.

## [2024-19-11]

### Cambios en Edición, Rechazo y Aprobación

### Modificaciones en DapperContext

- Se mejoró la gestión de excepciones en el constructor:
  - Se lanzó `ArgumentNullException` si `configuration` es nulo.
  - Se lanzó `InvalidOperationException` si la cadena de conexión "DefaultConnection" no se encuentra en la configuración.

### Modificaciones en Procedimientos Almacenados

- Se eliminó el parámetro `@ProgramId` del procedimiento `100_UpdateAgency`.
- Se actualizó el procedimiento `100_UpdateAgency` para retornar directamente el número de filas afectadas.
- Se creó el procedimiento almacenado `100_GetTemporaryPassword` para obtener una contraseña temporal por `UserId`.

### Interfaces

- Se añadió el método `SendApprovalSponsorEmail(User user, string temporaryPassword)` en `IEmailService`.
- Se añadió el método `SendDenialSponsorEmail(User user, string rejectionReason)` en `IEmailService`.
- Se creó la interfaz `IPasswordService` con el método `GetTemporaryPassword(string userId)`.

### Repositorios

- Se actualizó `AgencyRepository` para incluir inyecciones de `IEmailService` y `IPasswordService`.
- Se modificó el método `UpdateAgencyStatus` para enviar correos de aprobación o rechazo según el estado de la agencia.
- Se implementó el método `GetTemporaryPassword` en `UserRepository` para obtener la contraseña temporal de un usuario.

### Servicios

- Se creó `PasswordService` para manejar la obtención de contraseñas temporales.
- Se actualizó `EmailService` para enviar correos de aprobación y rechazo a los auspiciadores.

### Mejoras Generales

- Se mejoró el manejo de errores y la legibilidad del código en varios métodos.
- Se implementaron registros de log para el envío de correos electrónicos y la obtención de contraseñas temporales.

Estos cambios mejoran la funcionalidad del sistema al optimizar el manejo de correos electrónicos y la gestión de contraseñas temporales, así como la lógica de aprobación y rechazo de agencias.

## [2024-19-12] hasta [2024-12-07]

### Implementación de Etapas 2, 3 y 4

### Controladores

- **AgencyController**:

  - Renombrado `GetAllAgencies` a `GetAgencyById` para obtener una agencia específica.
  - Añadido `GetAgencyByIdAndUserId` para obtener agencia por ID y usuario.
  - Restaurado `GetAllAgencies` con parámetros actualizados.
  - Implementado `GetAgencyProgramsByUserId` para obtener programas por usuario.
  - Añadido `UpdateAgencyProgram` para actualizar programas de agencias.

- **AuthController**:

  - Añadido logger para registro de operaciones.
  - Implementado `ResetPassword` para restablecer contraseñas con documentación Swagger.

- **ProgramController**:

  - Modificada autorización para `InsertProgram` e `InsertProgramInscription`.
  - Añadidos endpoints para gestión de programas y autoridades alimentarias.

- **SchoolController (Nuevo)**:
  - Implementados endpoints CRUD para escuelas.
  - Añadida gestión de tipos de comidas.

### Base de Datos

#### Nuevas Tablas

- Creadas tablas para:
  - `FoodAuthority`, `OperatingPolicy`, `AlternativeCommunication`
  - `OptionSelection`, `EducationLevel`, `OperatingPeriod`
  - `MealType`, `OrganizationType`, `Facility`
  - `School`, `SatelliteSchool`, `SchoolMealRequest`
  - `ProgramInscription` y tablas relacionadas

#### Modificaciones en Tablas Existentes

- **Agency**: Añadidos campos `AppointmentCoordinated` y `AppointmentDate`
- **AspNetUsers**: Añadido campo `IsTemporalPasswordActived`
- **AgencyProgram**: Nueva estructura con campos adicionales

#### Procedimientos Almacenados

- Creados nuevos procedimientos:
  - `101_GetAgencies`
  - `101_GetAgencyByIdAndUserId`
  - `100_UpdateAgencyProgram`
  - `100_InsertProgram`
  - `100_InsertProgramInscription`
  - `100_GetAllProgramInscriptions`

### Modelos y DTOs

- Añadidos nuevos DTOs:
  - `DTOAlternativeCommunication`, `DTODocumentsRequired`
  - `DTOEducationLevel`, `DTOFacility`
  - `DTOFederalFundingSource`, `DTOFoodAuthority`
  - `DTOMealType`, `DTOOperatingPolicy`
  - `DTOProgramInscription`, `DTOSchool`

### Interfaces y Repositorios

- Implementado `ISchoolRepository` y `SchoolRepository`
- Actualizado `IAgencyRepository` con nuevos métodos
- Modificado `IProgramRepository` con funcionalidades adicionales
- Actualizado `IUserRepository` con gestión de contraseñas temporales

### Servicios

- Mejorado sistema de envío de correos
- Implementada gestión de contraseñas temporales
- Añadido servicio de escuelas en `Startup.cs`

### Seguridad y Validación

- Mejorado manejo de errores en DapperContext
- Implementada validación de contraseñas temporales
- Actualizada gestión de claims para roles específicos

### Datos Iniciales

- Añadidos scripts de inserción para:
  - Estados de agencia
  - Agencia por defecto (AESAN)
  - Programas base
  - Ciudades y regiones
  - Roles y usuarios iniciales

## [2024-12-07] hasta [2025-01-01]

### Implementación de Reglas de Desarrollo y Nuevas Funcionalidades

### Documentación y Estándares

- Se creó archivo `.cursorrules` con reglas detalladas de desarrollo para:
  - Convenciones de código C# y .NET
  - Estándares de nomenclatura
  - Patrones de diseño y arquitectura
  - Ejemplos de implementación para interfaces, controladores y repositorios
  - Reglas para procedimientos almacenados

### Controladores

- **MealTypeController (Nuevo)**:

  - Implementación completa de operaciones CRUD para tipos de comida
  - Documentación Swagger para cada endpoint
  - Manejo de autorizaciones y validaciones
  - Logging de operaciones

- **SchoolController (Actualizado)**:
  - Refactorización usando constructor con inyección de dependencias
  - Actualización de rutas siguiendo convenciones REST
  - Eliminación de endpoints redundantes
  - Mejora en el manejo de errores

### Base de Datos

#### Procedimientos Almacenados

- **Nuevos Procedimientos para MealType**:

  - `100_DeleteMealType`
  - `100_GetAllMealTypes`
  - `100_GetMealTypeById`
  - `100_InsertMealType`
  - `100_UpdateMealType`

- **Actualizaciones en Procedimientos Existentes**:
  - `100_InsertAgency`: Añadido campo `OrganizedAthleticPrograms`
  - `101_GetAgencies`: Mejorada la lógica de filtrado

#### Modificaciones en Tablas

- **Agency**: Añadidos nuevos campos:
  - `NonProfit`
  - `FederalFundsDenied`
  - `StateFundsDenied`
  - `OrganizedAthleticPrograms`
- **SchoolMealRequest**: Renombrada a `SchoolMeal`

### Interfaces

- **IMealTypeRepository (Nuevo)**:

  - Definición completa de operaciones CRUD
  - Documentación XML para cada método

- **ISchoolRepository (Actualizado)**:

  - Simplificación de métodos
  - Eliminación de métodos redundantes
  - Actualización de firmas de métodos

- **IAgencyRepository (Actualizado)**:
  - Modificación de parámetros en `GetAllAgenciesFromDb`

### Modelos

- **Nuevos DTOs**:
  - `DTOAlternativeCommunication`
  - `DTODocumentsRequired`
  - `DTOEducationLevel`
- Actualización de propiedades para usar `required`
- Añadidos namespaces faltantes

### Mejoras Generales

- Implementación consistente de manejo de errores
- Mejora en la documentación de API con Swagger
- Estandarización de convenciones de código
- Optimización de consultas a base de datos

## [2025-01-07]

### Implementación de Nuevos Controladores y Procedimientos Almacenados

### Documentación y Estándares

- Se actualizó `.cursorrules` con nueva convención de nombres para archivos consolidados de procedimientos almacenados:
  - Nuevo formato: `100_AllStoredProceduresForEntityName.sql`

### Controladores

- **AgencyController**:

  - Eliminado método `GetAllAgencyStatus` para mover funcionalidad a nuevo controlador.

- **Nuevos Controladores Implementados**:
  - **AgencyStatusController**: Gestión de estados de agencias.
  - **EducationLevelController**: Gestión de niveles educativos.
  - **FacilityController**: Gestión de instalaciones.
  - **FederalFundingCertificationController**: Gestión de certificaciones de fondos federales.
  - **FoodAuthorityController**: Gestión de autoridades alimentarias.
  - **OperatingPeriodController**: Gestión de períodos operativos.
  - **OperatingPolicyController**: Gestión de políticas operativas.
  - **OrganizationTypeController**: Gestión de tipos de organizaciones.

### Base de Datos

#### Nuevos Procedimientos Almacenados

- **AgencyStatus**:

  - `100_GetAllAgencyStatuses`
  - `100_InsertAgencyStatus`
  - `100_DeleteAgencyStatus`
  - `100_GetAgencyStatusById`
  - `100_UpdateAgencyStatus`

- **EducationLevel**:

  - `100_GetAllEducationLevels`
  - `100_InsertEducationLevel`
  - `100_DeleteEducationLevel`
  - `100_GetEducationLevelById`
  - `100_UpdateEducationLevel`

- **Facility**:

  - `100_GetAllFacilities`
  - `100_GetFacilityById`
  - `100_InsertFacility`
  - `100_UpdateFacility`
  - `100_DeleteFacility`

- **FederalFundingCertification**:

  - `100_GetAllFederalFundingCertifications`
  - `100_InsertFederalFundingCertification`
  - `100_DeleteFederalFundingCertification`
  - `100_GetFederalFundingCertificationById`
  - `100_UpdateFederalFundingCertification`

- **FoodAuthority**:

  - `100_GetAllFoodAuthorities`
  - `100_GetFoodAuthorityById`
  - `100_InsertFoodAuthority`
  - `100_UpdateFoodAuthority`
  - `100_DeleteFoodAuthority`

- **OperatingPeriod**:
  - `100_GetAllOperatingPeriods`
  - `100_GetOperatingPeriodById`
  - `100_InsertOperatingPeriod`
  - `100_UpdateOperatingPeriod`
  - `100_DeleteOperatingPeriod`

### Características Comunes en Todos los Nuevos Controladores

- Implementación de operaciones CRUD completas
- Manejo de errores con try-catch y logging
- Autorización JWT (condicional en modo DEBUG)
- Documentación Swagger para cada endpoint
- Uso de inyección de dependencias
- Respuestas HTTP apropiadas (200, 201, 400, 404, 500)

### Mejoras Generales

- Reorganización de funcionalidades en controladores específicos
- Estandarización de nombres y estructuras de procedimientos almacenados
- Mejora en la organización del código y separación de responsabilidades
- Implementación consistente de patrones de diseño

## [2025-01-08]

### Implementación de Actualización de Programas y Mejoras en Agencias

### Controladores

- **AgencyController**:
  - Implementado endpoint `UpdateAgencyProgram` con validación de modelo
  - Añadida documentación Swagger para el nuevo endpoint
  - Mejorada la gestión de autorizaciones

### Modelos

- **Nuevos Modelos**:
  - Creado `UpdateAgencyProgramRequest` con propiedades:
    - AgencyId
    - ProgramId
    - StatusId
    - UserId
    - Comment
    - AppointmentCoordinated
    - AppointmentDate

### Interfaces

- **IAgencyRepository**:
  - Añadido método `UpdateAgencyProgram` con parámetros completos
  - Mejorada la documentación XML
  - Reorganizada la estructura de la interfaz por secciones (Actualizar, Eliminar)

### Repositorios

- **AgencyRepository**:
  - Implementada lógica de actualización de programas de agencia
  - Mejorado sistema de logging para seguimiento de operaciones
  - Implementada validación de estados y envío de correos según el estado
  - Añadida gestión de contraseñas temporales para estados específicos
  - Mejorado manejo de errores con try-catch

### Base de Datos

- **Procedimientos Almacenados**:
  - Creado `100_UpdateAgencyProgram` con parámetros:
    - @AgencyId
    - @ProgramId
    - @StatusId
    - @UserId
    - @Comment
    - @AppointmentCoordinated
    - @AppointmentDate

### Mejoras Generales

- Implementación de logging detallado para seguimiento de operaciones
- Mejora en el manejo de errores y excepciones
- Optimización en la gestión de estados de agencias
- Implementación de validaciones de modelo
- Mejora en la documentación de API

## [2025-01-09] hasta [2025-02-07]

### Reestructuración de Base de Datos y Mejoras en el Sistema

### Reglas y Documentación

- Se añadió archivo `.SQLRules.mdc` con reglas para:
  - Uso de procedimientos almacenados
  - Ubicación de archivos
  - Convenciones de nombres
  - Versionamiento

### Base de Datos

#### Reestructuración de Tablas

- **Nuevas Tablas**:

  - Creación de nueva estructura para `Agency` con campos adicionales como `AgencyCode`
  - Reorganización de tablas `City`, `Region` y `CityRegion`

- **Eliminaciones**:
  - Eliminado `Tables-2.sql` y renombrado `Tables.sql` a `Tables-Deprecado.sql`
  - Removidas tablas obsoletas relacionadas con usuarios y gestión de agencias

#### Procedimientos Almacenados

- **Nuevos Archivos Consolidados**:

  - `101_AllStoredProceduresForAgency.sql`
  - `101_AllStoredProceduresForCityRegion.sql`

- **Actualizaciones**:
  - Versión 101 de procedimientos para gestión de agencias
  - Nuevos procedimientos para manejo de ciudades y regiones
  - Mejoras en la gestión de relaciones ciudad-región

### Modelos y DTOs

- **DTOAgency**:
  - Añadida propiedad `AgencyCode`
  - Actualización de propiedades relacionadas con ubicación

### Repositorios

- **AgencyRepository**:
  - Actualización a versión 101 de procedimientos almacenados
  - Implementación de generación de códigos únicos para agencias
  - Mejoras en el manejo de parámetros

### Utilidades

- **Nuevo Sistema de Códigos**:
  - Implementado `GenerateAgencyCode` para crear identificadores únicos
  - Métodos auxiliares para generación de iniciales
  - Sistema de secuencia numérica por año

### Configuración

- **Actualizaciones de Base de Datos**:
  - Desarrollo: Cambio de `AESAN-2` a `NUTRE`
  - Producción: Cambio de `aesandb` a `aesan2`

### Datos Iniciales

- **Carga de Datos**:
  - Inserción de 7 regiones principales
  - Carga de 78 ciudades
  - Establecimiento de relaciones ciudad-región
  - Datos iniciales para agencia AESAN

### Mejoras Generales

- Implementación de versionamiento para procedimientos almacenados
- Mejor organización de archivos de base de datos
- Optimización de consultas y relaciones
- Implementación de sistema de códigos únicos para agencias

Estos cambios representan una significativa reestructuración del sistema, mejorando la organización de la base de datos y añadiendo nuevas funcionalidades para la gestión de agencias.

## [2025-02-12] hasta [2025-02-13]

### Mejoras en Gestión de Programas y Validaciones

### Controladores

- **ProgramController**:
  - Implementado nuevo endpoint `GetProgramSuggestions` para obtener sugerencias de programas
  - Mejorada la validación de parámetros de consulta
  - Optimizado el manejo de respuestas para sugerencias

### Base de Datos

#### Procedimientos Almacenados

- **Nuevos Procedimientos**:

  - `101_GetProgramSuggestions`: Obtiene sugerencias de programas basadas en criterios específicos
  - `101_GetProgramById`: Versión actualizada para incluir información detallada

- **Actualizaciones**:
  - Optimización de consultas existentes para mejor rendimiento
  - Mejora en la gestión de parámetros nulos

### Interfaces

- **IProgramRepository**:
  - Añadido método `GetProgramSuggestions`
  - Actualizada documentación XML
  - Mejorada la estructura de la interfaz

### Repositorios

- **ProgramRepository**:
  - Implementada lógica para obtener sugerencias de programas
  - Mejorado sistema de logging
  - Optimizado manejo de errores

### Mejoras Generales

- Implementación de validaciones más robustas
- Optimización de consultas a base de datos
- Mejora en el manejo de respuestas HTTP
- Actualización de documentación Swagger

## [2025-02-18]

### Mejoras en Gestión de Usuarios y Configuración del Sistema

### Controladores

- **UserController**:
  - Implementado nuevo endpoint `AddUserToDb` para registro de usuarios
  - Mejorada validación de datos de usuario
  - Optimizado manejo de roles

### Interfaces y Repositorios

- **IUserRepository/UserRepository**:
  - Actualizado método `RegisterUser` para usar DTOUser
  - Mejorada gestión de contraseñas temporales
  - Optimizado manejo de datos de usuario

### Configuración del Sistema

- **Startup.cs**:
  - Actualizada configuración CORS
  - Añadido soporte para nuevos dominios
  - Mejorada configuración de Swagger

### Mejoras Generales

- Implementación de validaciones más robustas
- Optimización de registro de usuarios
- Mejora en el manejo de respuestas HTTP
- Actualización de documentación Swagger

## [2025-02-19] hasta [2025-03-28]

### Mejoras en Gestión de Usuarios y Sistema de Agencias

### Controladores y Endpoints

- **UserController**:

  - Implementado nuevo endpoint `AddUserToDb` para agregar usuarios a la base de datos
  - Mejoradas las validaciones de datos de usuario
  - Optimizado el manejo de roles y permisos

- **UploadController**:

  - Mejoras en el manejo de subida de archivos
  - Implementación de validaciones más robustas

- **AgencyController**:

  - Optimización en la obtención de datos de agencias
  - Mejoras en la asignación de usuarios a agencias

- **AgencyUserAssignmentController**:
  - Nuevo controlador para la gestión de asignaciones de usuarios a agencias
  - Implementación de endpoints para asignar y desasignar agencias a usuarios

### Base de Datos

#### Procedimientos Almacenados

- **Nuevos Procedimientos**:

  - `109_GeneratePasswordResetToken`: Generación de tokens para restablecimiento de contraseñas
  - `110_ValidatePasswordResetToken`: Validación de tokens para restablecimiento de contraseñas
  - `101_GetAllProgramInscriptions`: Obtiene todas las inscripciones a programas
  - `103_GetAgencyByIdAndUserId`: Obtiene una agencia por ID y usuario
  - `104_GetAgencyByIdAndUserId`: Versión mejorada del procedimiento anterior

- **Actualizaciones**:
  - `108_GetAllUsersFromDb`: Mejora en la obtención de usuarios de la base de datos
  - `101_AssignAgencyToUser`: Optimización en la asignación de agencias a usuarios
  - Renombrados varios procedimientos almacenados para mejorar la organización

### Interfaces y Repositorios

- **IUserRepository**:

  - Actualizado el método `RegisterUser` para usar DTO en lugar de entidad User
  - Añadidos métodos relacionados con la gestión de contraseñas

- **IEmailService**:

  - Añadidas funciones para envío de correos de recuperación de contraseña

- **IAgencyUsersRepository**:

  - Mejorada la interfaz para la gestión de usuarios de agencias

- **UserRepository**:

  - Implementación de la lógica para registro de usuarios con DTOs
  - Mejoras en la gestión de contraseñas temporales
  - Soporte para avatares de usuario

- **AgencyRepository**:

  - Implementación de caché para mejorar rendimiento
  - Mejoras en la actualización de datos de agencias

- **AgencyUserAssignmentRepository**:
  - Nuevo repositorio para gestionar asignaciones de usuarios a agencias

### Modelos

- **DTOUser** y **DTOUserDB**:

  - Actualizados para incluir más campos relevantes
  - Mejoras en la validación de datos

- **UserAvatarRequest**:

  - Nuevo modelo para gestionar la subida de avatares de usuario

- **DTOChangePasword**:
  - Nuevo modelo para el cambio de contraseñas

### Mejoras Generales

- **Configuración CORS**:

  - Actualización de políticas para mejor control de acceso
  - Soporte para nuevos dominios

- **Caché y Rendimiento**:

  - Implementación de sistema de caché para mejorar tiempos de respuesta
  - Optimización de consultas a base de datos

- **Seguridad**:

  - Mejoras en la gestión de tokens JWT
  - Implementación de sistema de recuperación de contraseñas
  - Validaciones más robustas para datos sensibles

- **Telemetría y Logging**:

  - Implementación de servicio de logging mejorado
  - Integración con Application Insights para telemetría

- **Gestión de Archivos**:
  - Mejoras en el sistema de subida y gestión de archivos
  - Soporte para avatares de usuario
