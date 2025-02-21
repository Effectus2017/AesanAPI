# SQL Change Log

## 2024-03-21

### Stored Procedures

- [102_GetAgencies] Modificado el SP para cambiar INNER JOIN a LEFT JOIN en la unión con UserProgram
  - Se cambió de INNER JOIN a LEFT JOIN para permitir obtener todas las agencias incluso si no tienen un usuario asignado a su programa
  - Se crearon las migraciones correspondientes en Flyway para local y Azure
  - Versión: V1.0.2
