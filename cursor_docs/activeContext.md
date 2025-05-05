# activeContext.md

## En qué se está trabajando ahora

- Refactorización de todos los repositorios para que usen `DynamicParameters` especificando explícitamente `DbType` y `ParameterDirection` en todos los parámetros de inserción, actualización y borrado.
- Ya se aplicó el cambio en: HouseholdRepository.cs y SchoolRepository.cs.
- El patrón está documentado en `systemPatterns.md`.

## Cambios recientes

- Refactorización completa de HouseholdRepository.cs y SchoolRepository.cs.
- Creación/actualización de la documentación de patrones de sistema.

## Próximos pasos

- Continuar aplicando el mismo patrón en el resto de los repositorios de la carpeta Api/Repositories.
- Actualizar la documentación de progreso y tecnologías si es necesario.
