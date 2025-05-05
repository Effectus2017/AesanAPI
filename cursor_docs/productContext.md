# productContext.md

## Por qué existe este proyecto

El proyecto busca gestionar y automatizar procesos administrativos y operativos para agencias, escuelas y entidades relacionadas, centralizando la información y facilitando la trazabilidad y control de datos.

## Qué problema resuelve

- Evita la dispersión de información en sistemas aislados.
- Mejora la integridad y consistencia de los datos.
- Permite la gestión eficiente de entidades complejas (agencias, escuelas, usuarios, etc.)
- Facilita la integración y el reporte de información para la toma de decisiones.

## Cómo debe funcionar

- Debe proveer una API robusta y segura para la gestión de entidades.
- La capa de acceso a datos debe ser mantenible y robusta, usando patrones claros como el uso explícito de DynamicParameters con DbType y ParameterDirection en todos los procedimientos almacenados.
- El sistema debe ser fácilmente auditable y extensible.
