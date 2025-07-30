# Migración de Employee a Staff + StaffType

## Resumen

Esta migración convierte el sistema de "Empleados" a "Staff" y agrega un nuevo catálogo "StaffType" para categorizar el personal.

## Archivos Creados

### Base de Datos - StaffType

- `StaffType-Table.sql` - Tabla principal de tipos de staff
- `100_GetStaffTypeById.sql` - Obtener tipo de staff por ID
- `100_GetAllStaffTypes.sql` - Obtener todos los tipos de staff
- `100_InsertStaffType.sql` - Insertar nuevo tipo de staff
- `100_UpdateStaffType.sql` - Actualizar tipo de staff
- `100_DeleteStaffType.sql` - Eliminar tipo de staff (baja lógica)
- `100_InsertStaffTypeTitles.sql` - Datos iniciales de tipos de staff

### Base de Datos - Staff

- `Staff-Table.sql` - Tabla principal de staff (migración de Employee)
- `100_GetStaffById.sql` - Obtener staff por ID
- `100_GetAllStaff.sql` - Obtener todo el staff
- `100_InsertStaff.sql` - Insertar nuevo staff
- `100_UpdateStaff.sql` - Actualizar staff
- `100_DeleteStaff.sql` - Eliminar staff (baja lógica)
- `100_ConvertStaffToUser.sql` - Convertir staff en usuario
- `100_UpdateStaffActiveStatus.sql` - Actualizar estado activo
- `100_HasMainStaff.sql` - Verificar si existe staff principal

### Backend - Modelos

- `StaffType.cs` - Modelo de tipo de staff
- `DTOStaffType.cs` - DTO de tipo de staff
- `StaffTypeRequest.cs` - Request de tipo de staff
- `Staff.cs` - Modelo de staff (migración de Employee)
- `DTOStaff.cs` - DTO de staff (migración de DTOEmployee)
- `StaffRequest.cs` - Request de staff (migración de EmployeeRequest)

### Backend - Interfaces

- `IStaffTypeRepository.cs` - Interfaz para StaffType
- `IStaffRepository.cs` - Interfaz para Staff (migración de IEmployeeRepository)

### Backend - Repositorios

- `StaffTypeRepository.cs` - Repositorio de StaffType
- `StaffRepository.cs` - Repositorio de Staff (migración de EmployeeRepository)

### Backend - Controladores

- `StaffTypeController.cs` - Controlador de StaffType
- `StaffController.cs` - Controlador de Staff (migración de EmployeeController)

### Scripts de Migración

- `110_MigrateEmployeeToStaff.sql` - Migración de datos de Employee a Staff
- `120_CleanupEmployeeTables.sql` - Limpieza de tablas Employee (opcional)

## Archivos Modificados

### Backend - Configuración

- `IUnitOfWork.cs` - Agregado StaffRepository y StaffTypeRepository
- `UnitOfWork.cs` - Implementación de nuevos repositorios
- `Program.cs` - Registro de nuevos servicios
- `ApplicationSettings.cs` - Nuevas claves de caché para Staff y StaffType

### Base de Datos - Permisos

- `Permission-InsertPermissions.sql` - Nuevos permisos para Staff y StaffType

## Orden de Ejecución

### 1. Crear StaffType

```sql
-- Ejecutar en orden:
1. StaffType-Table.sql
2. 100_GetStaffTypeById.sql
3. 100_GetAllStaffTypes.sql
4. 100_InsertStaffType.sql
5. 100_UpdateStaffType.sql
6. 100_DeleteStaffType.sql
7. 100_InsertStaffTypeTitles.sql
```

### 2. Crear Staff

```sql
-- Ejecutar en orden:
1. Staff-Table.sql
2. 100_GetStaffById.sql
3. 100_GetAllStaff.sql
4. 100_InsertStaff.sql
5. 100_UpdateStaff.sql
6. 100_DeleteStaff.sql
7. 100_ConvertStaffToUser.sql
8. 100_UpdateStaffActiveStatus.sql
9. 100_HasMainStaff.sql
```

### 3. Migrar Datos

```sql
-- Ejecutar después de crear las tablas:
110_MigrateEmployeeToStaff.sql
```

### 4. Actualizar Permisos

```sql
-- Ejecutar para agregar nuevos permisos:
-- Los nuevos permisos ya están incluidos en Permission-InsertPermissions.sql
```

### 5. Limpiar (Opcional)

```sql
-- Solo ejecutar después de verificar que la migración fue exitosa:
120_CleanupEmployeeTables.sql
```

## Cambios en el Sistema

### Nuevas Funcionalidades

1. **StaffType**: Catálogo para categorizar el personal
2. **Staff con StaffType**: Relación entre staff y su tipo
3. **Nuevos Permisos**: Permisos específicos para Staff y StaffType

### Migraciones

1. **Employee → Staff**: Migración completa de datos
2. **employeePosition → staffPosition**: Actualización de OptionSelection
3. **Permisos Employee → Staff**: Nuevos permisos para staff

### Configuraciones

1. **Caché**: Nuevas claves para Staff y StaffType
2. **Servicios**: Registro de nuevos repositorios y controladores
3. **UnitOfWork**: Actualización para incluir nuevos repositorios

## Verificación

### Después de la Migración

1. Verificar que los datos se migraron correctamente
2. Probar los nuevos endpoints de Staff y StaffType
3. Verificar que los permisos funcionan correctamente
4. Comprobar que el caché funciona para las nuevas entidades

### Endpoints Nuevos

- `GET /staff-type/get-staff-type-by-id`
- `GET /staff-type/get-all-staff-types-from-db`
- `POST /staff-type/insert-staff-type`
- `PUT /staff-type/update-staff-type`
- `DELETE /staff-type/delete-staff-type`

- `GET /staff/get-staff-by-id`
- `GET /staff/get-all-staff-from-db`
- `POST /staff/insert-staff`
- `PUT /staff/update-staff`
- `DELETE /staff/delete-staff`
- `POST /staff/convert-staff-to-user`
- `GET /staff/has-main-staff`
- `PUT /staff/update-staff-active-status`

## Notas Importantes

1. **Backup**: Siempre hacer backup antes de ejecutar la migración
2. **Verificación**: Verificar que la migración fue exitosa antes de limpiar
3. **Permisos**: Los nuevos permisos deben asignarse a los usuarios correspondientes
4. **Frontend**: El frontend debe actualizarse para usar los nuevos endpoints
5. **Documentación**: Actualizar la documentación de la API

## Rollback

Si es necesario hacer rollback:

1. Restaurar backup de la base de datos
2. Revertir cambios en el código
3. Restaurar configuraciones originales
