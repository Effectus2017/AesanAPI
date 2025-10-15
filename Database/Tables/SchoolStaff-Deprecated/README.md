# SchoolStaff - Asignación de Empleados a Sitios

## Descripción

Este módulo implementa la funcionalidad para asignar empleados del staff a sitios/escuelas del sistema. Permite gestionar las relaciones entre empleados y sitios de manera flexible y controlada.

## Características

- **Asignación múltiple**: Un sitio puede tener múltiples empleados asignados
- **Tipos de asignación**: Principal, Secundario, Temporal, Apoyo
- **Empleado principal**: Solo un empleado puede ser principal por sitio
- **Validaciones**: Solo permite asignar empleados (StaffTypeId = 1), no miembros de junta
- **Trazabilidad**: Registra fechas de asignación, comentarios y auditoría completa
- **Baja lógica**: Las desasignaciones se marcan como inactivas, no se eliminan

## Estructura de la Base de Datos

### Tabla Principal: SchoolStaff

```sql
CREATE TABLE SchoolStaff
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,           -- Referencia al sitio
    StaffId INT NOT NULL,            -- Referencia al empleado
    AssignmentDate DATE NOT NULL,    -- Fecha de asignación
    AssignmentTypeId INT NOT NULL,   -- Tipo de asignación
    IsPrimary BIT NOT NULL,          -- Es empleado principal
    StartDate DATE NULL,             -- Fecha de inicio
    EndDate DATE NULL,               -- Fecha de finalización
    Comments NVARCHAR(500) NULL,     -- Comentarios
    IsActive BIT NOT NULL,           -- Estado activo
    CreatedAt DATETIME NOT NULL,     -- Fecha de creación
    UpdatedAt DATETIME NULL          -- Fecha de actualización
);
```

### Tipos de Asignación

Los tipos se almacenan en `OptionSelection` con `optionKey = 'staffAssignmentType'`:

1. **Principal** (Primary) - Empleado principal del sitio
2. **Secundario** (Secondary) - Empleado de apoyo
3. **Temporal** (Temporary) - Asignación temporal
4. **Apoyo** (Support) - Empleado de soporte

## Procedimientos Almacenados

### 1. `100_GetStaffBySchool`

Obtiene todos los empleados asignados a un sitio específico.

**Parámetros:**

- `@schoolId` - ID del sitio

**Retorna:** Lista completa de empleados con información del staff, sitio y asignación.

### 2. `100_GetSchoolsByStaff`

Obtiene todos los sitios asignados a un empleado específico.

**Parámetros:**

- `@staffId` - ID del empleado

**Retorna:** Lista completa de sitios con información del sitio y asignación.

### 3. `100_AssignStaffToSchool`

Asigna un empleado a un sitio.

**Parámetros:**

- `@schoolId` - ID del sitio
- `@staffId` - ID del empleado
- `@assignmentTypeId` - ID del tipo de asignación
- `@isPrimary` - Es empleado principal (opcional, default: 0)
- `@startDate` - Fecha de inicio (opcional)
- `@endDate` - Fecha de finalización (opcional)
- `@comments` - Comentarios (opcional)

**Validaciones:**

- El sitio debe existir y estar activo
- El empleado debe existir y estar activo
- Solo se permiten empleados (StaffTypeId = 1)
- No puede haber asignaciones duplicadas activas
- Si es principal, desactiva otros empleados principales del sitio

**Retorna:** ID de la nueva asignación.

### 4. `100_UnassignStaffFromSchool`

Desasigna un empleado de un sitio (baja lógica).

**Parámetros:**

- `@schoolId` - ID del sitio
- `@staffId` - ID del empleado

**Retorna:** 1 si se desasignó exitosamente.

### 5. `100_UpdateSchoolStaff`

Actualiza una asignación existente.

**Parámetros:**

- `@id` - ID de la asignación
- `@assignmentTypeId` - Nuevo tipo de asignación (opcional)
- `@isPrimary` - Nuevo estado de empleado principal (opcional)
- `@startDate` - Nueva fecha de inicio (opcional)
- `@endDate` - Nueva fecha de finalización (opcional)
- `@comments` - Nuevos comentarios (opcional)

**Retorna:** 1 si se actualizó exitosamente.

### 6. `100_GetSchoolStaffById`

Obtiene una asignación específica por su ID.

**Parámetros:**

- `@id` - ID de la asignación

**Retorna:** Información completa de la asignación.

## Implementación en el Backend

### Modelos

- `SchoolStaff.cs` - Modelo principal
- `DTOSchoolStaff.cs` - DTO con información completa
- `SchoolStaffRequest.cs` - Request para asignaciones
- `UpdateSchoolStaffRequest.cs` - Request para actualizaciones

### Interfaces

- `ISchoolStaffRepository.cs` - Interfaz del repositorio

### Repositorios

- `SchoolStaffRepository.cs` - Implementación del repositorio

### Controladores

- `SchoolStaffController.cs` - API endpoints para gestionar asignaciones

### Endpoints Disponibles

- `GET /school-staff/get-staff-by-school?schoolId={id}` - Obtener empleados de un sitio
- `GET /school-staff/get-schools-by-staff?staffId={id}` - Obtener sitios de un empleado
- `GET /school-staff/get-assignment-by-id?id={id}` - Obtener asignación por ID
- `POST /school-staff/assign-staff` - Asignar empleado a sitio
- `PUT /school-staff/update-assignment/{id}` - Actualizar asignación
- `DELETE /school-staff/unassign-staff?schoolId={id}&staffId={id}` - Desasignar empleado

## Instalación

### 1. Ejecutar Script de Migración

```sql
-- Ejecutar el script completo
EXEC 000_Migration_SchoolStaff_Complete.sql
```

### 2. Verificar la Instalación

El script de migración incluye verificaciones automáticas que confirman:

- ✅ Tabla SchoolStaff creada
- ✅ Tipos de asignación insertados
- ✅ Todos los procedimientos almacenados creados

### 3. Actualizar IUnitOfWork

Asegurarse de que `IUnitOfWork` incluya:

```csharp
ISchoolStaffRepository SchoolStaffRepository { get; }
```

## Uso

### Asignar un Empleado a un Sitio

```csharp
var request = new SchoolStaffRequest
{
    SchoolId = 1,
    StaffId = 5,
    AssignmentTypeId = 1, // Principal
    IsPrimary = true,
    StartDate = DateTime.Today,
    Comments = "Empleado principal del sitio"
};

var assignmentId = await _unitOfWork.SchoolStaffRepository.AssignStaffToSchool(request);
```

### Obtener Empleados de un Sitio

```csharp
var staff = await _unitOfWork.SchoolStaffRepository.GetStaffBySchool(schoolId);
foreach (var employee in staff)
{
    Console.WriteLine($"Empleado: {employee.StaffFirstName} {employee.StaffFatherLastName}");
    Console.WriteLine($"Tipo: {employee.AssignmentTypeName}");
    Console.WriteLine($"Principal: {employee.IsPrimary}");
}
```

### Obtener Sitios de un Empleado

```csharp
var sites = await _unitOfWork.SchoolStaffRepository.GetSchoolsByStaff(staffId);
foreach (var site in sites)
{
    Console.WriteLine($"Sitio: {site.SchoolName}");
    Console.WriteLine($"Dirección: {site.Address}");
    Console.WriteLine($"Tipo de Asignación: {site.AssignmentTypeName}");
}
```

## Reglas de Negocio

1. **Solo empleados**: Solo se pueden asignar empleados (StaffTypeId = 1), no miembros de junta
2. **Un empleado principal por sitio**: Solo un empleado puede ser principal por sitio
3. **Sin duplicados**: Un empleado no puede estar asignado al mismo sitio más de una vez activamente
4. **Validaciones de integridad**: Todas las referencias deben existir y estar activas
5. **Baja lógica**: Las desasignaciones se marcan como inactivas, no se eliminan físicamente

## Consideraciones de Rendimiento

- **Índices**: Se han creado índices en las columnas más consultadas
- **JOINs optimizados**: Los procedimientos usan JOINs eficientes
- **Filtros**: Se aplican filtros de estado activo para evitar datos obsoletos
- **Ordenamiento**: Los resultados se ordenan por empleado principal y fecha de asignación

## Mantenimiento

### Limpieza de Datos

Para mantener la base de datos limpia, considerar:

```sql
-- Verificar asignaciones inactivas antiguas
SELECT * FROM SchoolStaff
WHERE IsActive = 0
AND UpdatedAt < DATEADD(YEAR, -2, GETDATE());

-- Verificar sitios sin empleados asignados
SELECT s.* FROM School s
LEFT JOIN SchoolStaff ss ON s.Id = ss.SchoolId AND ss.IsActive = 1
WHERE ss.Id IS NULL AND s.IsActive = 1;
```

### Monitoreo

- Revisar logs de errores en los procedimientos almacenados
- Monitorear el rendimiento de las consultas
- Verificar la integridad referencial periódicamente

## Soporte

Para problemas o preguntas sobre este módulo:

1. Revisar los logs de la aplicación
2. Verificar que todos los procedimientos almacenados estén creados
3. Confirmar que las dependencias (Staff, School, OptionSelection) estén activas
4. Ejecutar las verificaciones del script de migración

---

**Nota**: Este módulo está diseñado para ser extensible. Se pueden agregar nuevos tipos de asignación, campos adicionales o funcionalidades específicas según las necesidades del negocio.
