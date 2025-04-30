# Registro de Cambios SQL - 21/03/2024

## Stored Procedures Creados

### Tabla: Program

Se han creado los siguientes stored procedures versión 100:

1. **100_GetPrograms.sql**

   - `[100_GetAllPrograms]`: Obtiene lista de programas con paginación y filtros
   - `[100_GetProgramById]`: Obtiene un programa específico por su Id

2. **100_InsertProgram.sql**

   - `[100_InsertProgram]`: Inserta un nuevo programa

3. **100_UpdateProgram.sql**

   - `[100_UpdateProgram]`: Actualiza un programa existente

4. **100_DeleteProgram.sql**
   - `[100_DeleteProgram]`: Eliminación lógica de un programa (soft delete)

### Características principales:

- Implementación de soft delete para eliminación de registros
- Manejo automático de fechas (CreatedAt, UpdatedAt)
- Paginación y filtros en consultas
- Manejo de programas activos/inactivos
- Retorno de registros afectados en operaciones de modificación
