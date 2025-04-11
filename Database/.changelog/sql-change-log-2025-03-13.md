# Cambios SQL - 13 de marzo de 2025

## Tablas

- **Agency**: Se modificó el tipo de datos de las columnas `ZipCode` y `PostalZipCode` de INT a NVARCHAR(20) para permitir códigos postales alfanuméricos y con ceros a la izquierda.

## Stored Procedures

- **104_InsertAgency**: Actualizado para manejar `ZipCode` y `PostalZipCode` como NVARCHAR(20) en lugar de INT.
- **104_UpdateAgency**: Actualizado para manejar `ZipCode` y `PostalZipCode` como NVARCHAR(20) en lugar de INT.

## Migraciones

- Se creó el script de migración `103-cambio-a104.sql` para convertir los datos existentes de INT a NVARCHAR(20).
- Se actualizaron los archivos de migración para Flyway en los directorios `/.flyway/migrations-local-db/` y `/.flyway/migrations-azure-db/`.
