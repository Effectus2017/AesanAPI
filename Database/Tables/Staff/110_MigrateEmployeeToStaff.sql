-- =============================================
-- Script: 110_MigrateEmployeeToStaff
-- =============================================
-- Migra los datos de la tabla Employee a Staff
-- Este script debe ejecutarse después de crear la tabla Staff y StaffType

-- Paso 1: Verificar que la tabla Staff existe
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Staff')
BEGIN
    PRINT 'Error: La tabla Staff no existe. Ejecute primero el script Staff-Table.sql';
    RETURN;
END

-- Paso 2: Verificar que la tabla StaffType existe
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'StaffType')
BEGIN
    PRINT 'Error: La tabla StaffType no existe. Ejecute primero el script StaffType-Table.sql';
    RETURN;
END

-- Paso 3: Verificar que hay datos en StaffType
IF NOT EXISTS (SELECT *
FROM StaffType
WHERE IsActive = 1)
BEGIN
    PRINT 'Error: No hay tipos de staff disponibles. Ejecute primero el script 100_InsertStaffTypeTitles.sql';
    RETURN;
END

-- Paso 4: Obtener el ID del tipo de staff por defecto (Administrativo)
DECLARE @defaultStaffTypeId INT;
SELECT @defaultStaffTypeId = Id
FROM StaffType
WHERE Name = 'Administrativo' AND IsActive = 1;

IF @defaultStaffTypeId IS NULL
BEGIN
    -- Si no existe Administrativo, tomar el primero disponible
    SELECT @defaultStaffTypeId = Id
    FROM StaffType
    WHERE IsActive = 1
    ORDER BY Id;
END

PRINT 'Tipo de staff por defecto: ' + CAST(@defaultStaffTypeId AS VARCHAR(10));

-- Paso 5: Migrar datos de Employee a Staff
IF EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Employee')
BEGIN
    PRINT 'Iniciando migración de datos de Employee a Staff...';

    -- Insertar datos de Employee en Staff
    INSERT INTO Staff
        (
        FirstName,
        MiddleName,
        FatherLastName,
        MotherLastName,
        StatusId,
        PositionId,
        StaffTypeId,
        BirthDate,
        Email,
        PostalAddress,
        CityId,
        RegionId,
        AreaCode,
        Comments,
        UserId,
        CreatedAt,
        UpdatedAt,
        IsActive
        )
    SELECT
        FirstName,
        MiddleName,
        FatherLastName,
        MotherLastName,
        StatusId,
        PositionId,
        @defaultStaffTypeId, -- StaffTypeId por defecto
        BirthDate,
        Email,
        PostalAddress,
        CityId,
        RegionId,
        AreaCode,
        Comments,
        UserId,
        CreatedAt,
        UpdatedAt,
        IsActive
    FROM Employee
    WHERE IsActive = 1;

    PRINT 'Migración completada. ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros migrados.';

    -- Mostrar estadísticas de la migración
            SELECT
            'Registros migrados' AS Descripcion,
            COUNT(*) AS Cantidad
        FROM Staff
    UNION ALL
        SELECT
            'Registros originales en Employee' AS Descripcion,
            COUNT(*) AS Cantidad
        FROM Employee
        WHERE IsActive = 1;

END
ELSE
BEGIN
    PRINT 'La tabla Employee no existe. No hay datos para migrar.';
END

-- Paso 6: Actualizar OptionSelection para cambiar employeePosition por staffPosition
UPDATE OptionSelection 
SET OptionKey = 'staffPosition'
WHERE OptionKey = 'employeePosition';

PRINT 'OptionKey actualizado de employeePosition a staffPosition';

-- Paso 7: Verificar la migración
SELECT
    'Verificación de migración' AS Tipo,
    COUNT(*) AS TotalStaff,
    COUNT(CASE WHEN StaffTypeId = @defaultStaffTypeId THEN 1 END) AS ConTipoPorDefecto,
    COUNT(CASE WHEN StaffTypeId IS NULL THEN 1 END) AS SinTipo
FROM Staff
WHERE IsActive = 1;

PRINT 'Migración de Employee a Staff completada exitosamente.';