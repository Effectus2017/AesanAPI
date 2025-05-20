# Contexto activo

## Qué se está trabajando ahora

- Migración y consolidación de la lógica de HouseholdMemberIncome en su propio Controller y Repository.
- Limpieza de duplicidad en HouseholdController y HouseholdRepository.
- Actualización de las reglas CRUD para reflejar el patrón AgencyStatusController.

## Cambios recientes

- Eliminada la lógica de HouseholdMemberIncome de HouseholdController y HouseholdRepository.
- Implementado HouseholdMemberIncomeController y HouseholdMemberIncomeRepository siguiendo el patrón modular.
- Actualizado systemPattern-crud-model.md con la estructura y buenas prácticas de AgencyStatusController.

## Última tarea completada

- Implementada función en compare_databases.py para eliminar versiones viejas de SPs versionados (NNN_NombreSP) en ambas bases de datos.
- Mejorado --dry-run: ahora muestra explícitamente cuál es el SP más reciente que se conservará para cada grupo de versiones.
- Ahora la limpieza de SPs versionados genera un archivo de log con los SPs más recientes que se conservarán, para trazabilidad y seguridad.
- Documentado el uso de los comandos:
  - Simulación: `python compare_databases.py --clean-old-sp-versions --dry-run`
  - Borrado real: `python compare_databases.py --clean-old-sp-versions`
- Implementada opción --sync-missing-sps en compare_databases.py para sincronizar SPs faltantes desde archivos SQL hacia db2, deteniéndose ante el primer error.
- Documentado el uso, limitaciones y ejemplo en el propio script.
- Implementada opción --move-old-sp-files-local en compare_databases.py para mover versiones viejas de SPs locales a una subcarpeta Deprecated y renombrarlas como -Deprecated.sql.
- Documentado el uso, funcionamiento y limitaciones en el propio script.

## Próximos pasos

- Verificar y adaptar el frontend para consumir los nuevos endpoints.
- Actualizar la documentación técnica y el Memory Bank.
- Actualizar y/o crear pruebas unitarias e integración para el nuevo flujo.
- Validar en entorno de pruebas antes de ejecutar en producción.
- Actualizar changelog si se usa en entorno real.
- Validar la sincronización en entorno de pruebas.
- Mejorar la robustez para casos de dependencias entre SPs si es necesario.
- Validar la organización de archivos en entorno local.
- Considerar automatizar la limpieza de dependencias si es necesario.
