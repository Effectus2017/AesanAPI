# techContext.md

## Tecnologías utilizadas

- .NET (C#)
- Dapper para acceso a datos
- SQL Server como base de datos principal
- Inyección de dependencias y configuración vía appsettings.json
- Uso de caché en memoria (IMemoryCache)

## Setup de desarrollo

- Repositorio estructurado por capas: Controllers, Repositories, Models, Services, etc.
- Uso de procedimientos almacenados para todas las operaciones de datos.
- Scripts de migración y control de versiones en carpeta Database.

## Restricciones técnicas

- Todos los parámetros enviados a procedimientos almacenados deben usar DynamicParameters con DbType y ParameterDirection explícitos.
- El código debe ser fácilmente auditable y extensible.
- Se prioriza la robustez y la claridad en la capa de acceso a datos.
