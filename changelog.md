# Changelog

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
