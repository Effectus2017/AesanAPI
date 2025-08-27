-- =============================================
-- Script de Migración: SchoolStaff - Completo
-- =============================================
-- Este script implementa completamente la funcionalidad de asignación
-- de empleados a sitios (SchoolStaff)
-- 
-- Ejecutar en el siguiente orden:
-- 1. Crear tabla SchoolStaff
-- 2. Insertar tipos de asignación
-- 3. Crear procedimientos almacenados
-- =============================================

PRINT 'Iniciando migración de SchoolStaff...';

-- =============================================
-- PASO 1: Crear tabla SchoolStaff
-- =============================================
IF NOT EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'SchoolStaff')
BEGIN
    PRINT 'Creando tabla SchoolStaff...';

    CREATE TABLE SchoolStaff
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        SchoolId INT NOT NULL,
        StaffId INT NOT NULL,
        AssignmentDate DATE NOT NULL DEFAULT GETDATE(),
        AssignmentTypeId INT NOT NULL DEFAULT 1,
        IsPrimary BIT NOT NULL DEFAULT 0,
        StartDate DATE NULL,
        EndDate DATE NULL,
        Comments NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL
    );

    -- Restricciones de integridad referencial
    ALTER TABLE SchoolStaff
        ADD CONSTRAINT FK_SchoolStaff_School FOREIGN KEY (SchoolId) REFERENCES School(Id);

    ALTER TABLE SchoolStaff
        ADD CONSTRAINT FK_SchoolStaff_Staff FOREIGN KEY (StaffId) REFERENCES Staff(Id);

    ALTER TABLE SchoolStaff
        ADD CONSTRAINT FK_SchoolStaff_AssignmentType FOREIGN KEY (AssignmentTypeId) REFERENCES OptionSelection(Id);

    -- Restricción única: una persona no puede estar asignada al mismo sitio más de una vez activamente
    ALTER TABLE SchoolStaff
        ADD CONSTRAINT UQ_SchoolStaff_Active UNIQUE (SchoolId, StaffId, IsActive);

    -- Índices para mejorar el rendimiento
    CREATE INDEX IX_SchoolStaff_SchoolId ON SchoolStaff(SchoolId);
    CREATE INDEX IX_SchoolStaff_StaffId ON SchoolStaff(StaffId);
    CREATE INDEX IX_SchoolStaff_AssignmentTypeId ON SchoolStaff(AssignmentTypeId);
    CREATE INDEX IX_SchoolStaff_IsActive ON SchoolStaff(IsActive);
    CREATE INDEX IX_SchoolStaff_IsPrimary ON SchoolStaff(IsPrimary);
    CREATE INDEX IX_SchoolStaff_AssignmentDate ON SchoolStaff(AssignmentDate);
    CREATE INDEX IX_SchoolStaff_StartDate ON SchoolStaff(StartDate);
    CREATE INDEX IX_SchoolStaff_EndDate ON SchoolStaff(EndDate);

    PRINT 'Tabla SchoolStaff creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla SchoolStaff ya existe.';
END

-- =============================================
-- PASO 2: Insertar tipos de asignación
-- =============================================
PRINT 'Insertando tipos de asignación...';

IF NOT EXISTS (SELECT 1
FROM OptionSelection
WHERE OptionKey = 'staffAssignmentType')
BEGIN
    INSERT INTO OptionSelection
        (OptionKey, Name, NameEN, DisplayOrder, IsActive, CreatedAt)
    VALUES
        ('staffAssignmentType', 'Principal', 'Primary', 1, 1, GETDATE()),
        ('staffAssignmentType', 'Secundario', 'Secondary', 2, 1, GETDATE()),
        ('staffAssignmentType', 'Temporal', 'Temporary', 3, 1, GETDATE()),
        ('staffAssignmentType', 'Apoyo', 'Support', 4, 1, GETDATE());

    PRINT 'Tipos de asignación insertados exitosamente.';
END
ELSE
BEGIN
    PRINT 'Los tipos de asignación ya existen.';
END

-- =============================================
-- PASO 3: Crear procedimientos almacenados
-- =============================================
PRINT 'Creando procedimientos almacenados...';

-- Procedimiento: Obtener empleados por sitio
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_GetStaffBySchool')
    DROP PROCEDURE [dbo].[100_GetStaffBySchool];
GO

CREATE PROCEDURE [dbo].[100_GetStaffBySchool]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SchoolId,
        ss.StaffId,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt,

        -- Información del Staff
        s.FirstName AS StaffFirstName,
        s.MiddleName AS StaffMiddleName,
        s.FatherLastName AS StaffFatherLastName,
        s.MotherLastName AS StaffMotherLastName,
        s.Email AS StaffEmail,
        s.PositionId AS StaffPositionId,
        s.StaffTypeId,
        s.ContractStartDate,
        s.ContractEndDate,
        s.BirthDate,
        s.PostalAddress,
        s.CityId AS StaffCityId,
        s.RegionId AS StaffRegionId,
        s.AreaCode,
        s.Comments AS StaffComments,
        s.UserId AS StaffUserId,
        s.IsActive AS StaffIsActive,

        -- Información del Sitio
        sch.Name AS SchoolName,
        sch.Address AS SchoolAddress,
        sch.CityId AS SchoolCityId,
        sch.RegionId AS SchoolRegionId,
        sch.ZipCode AS SchoolZipCode,

        -- Información de la Asignación
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEn,

        -- Información de la posición del staff
        pos.Name AS StaffPositionName,
        pos.NameEN AS StaffPositionNameEn,

        -- Información del tipo de staff
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,

        -- Información de la ciudad del staff
        c.Name AS StaffCityName,
        c.Name AS StaffCityNameEn,

        -- Información de la región del staff
        r.Name AS StaffRegionName,
        r.Name AS StaffRegionNameEn

    FROM SchoolStaff ss
        INNER JOIN Staff s ON ss.StaffId = s.Id
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN OptionSelection pos ON s.PositionId = pos.Id
        INNER JOIN StaffType st ON s.StaffTypeId = st.Id
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id

    WHERE ss.SchoolId = @schoolId
        AND ss.IsActive = 1
        AND s.IsActive = 1
        AND sch.IsActive = 1

    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END
GO

-- Procedimiento: Obtener sitios por empleado
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_GetSchoolsByStaff')
    DROP PROCEDURE [dbo].[100_GetSchoolsByStaff];
GO

CREATE PROCEDURE [dbo].[100_GetSchoolsByStaff]
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SchoolId,
        ss.StaffId,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt,

        -- Información del Sitio
        sch.Name AS SchoolName,
        sch.Address AS SchoolAddress,
        sch.CityId AS SchoolCityId,
        sch.RegionId AS SchoolRegionId,
        sch.ZipCode AS SchoolZipCode,
        sch.Latitude,
        sch.Longitude,
        sch.PostalAddress AS SchoolPostalAddress,
        sch.PostalCityId AS SchoolPostalCityId,
        sch.PostalRegionId AS SchoolPostalRegionId,
        sch.PostalZipCode AS SchoolPostalZipCode,
        sch.SameAsPhysicalAddress,
        sch.OrganizationTypeId,
        sch.CenterTypeId,
        sch.NonProfit,
        sch.BaseYear,
        sch.RenewalYear,
        sch.OperatingFromDate,
        sch.OperatingToDate,
        sch.OperatingDaysCalculated,
        sch.KitchenTypeId,
        sch.GroupTypeId,
        sch.DeliveryTypeId,
        sch.SponsorTypeId,
        sch.ApplicantTypeId,
        sch.ResidentialTypeId,
        sch.OperatingPolicyId,
        sch.AreaTypeId,
        sch.HasWarehouse,
        sch.HasDiningRoom,
        sch.AdministratorAuthorizedName,
        sch.SitePhone,
        sch.Extension,
        sch.MobilePhone,
        sch.Breakfast,
        sch.BreakfastFrom,
        sch.BreakfastTo,
        sch.Lunch,
        sch.LunchFrom,
        sch.LunchTo,
        sch.Snack,
        sch.SnackFrom,
        sch.SnackTo,
        sch.Dinner,
        sch.DinnerFrom,
        sch.DinnerTo,
        sch.SnackNight,
        sch.SnackNightFrom,
        sch.SnackNightTo,
        sch.CommunityId,
        sch.WalkersId,
        sch.SiteTypeId,
        sch.ExperienceId,
        sch.ReviewResultId,
        sch.ReviewDate,
        sch.ReviewJustification,
        sch.IsActive AS SchoolIsActive,
        sch.InactiveJustification,
        sch.InactiveDate,
        sch.CreatedAt AS SchoolCreatedAt,
        sch.UpdatedAt AS SchoolUpdatedAt,

        -- Información de la Asignación
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEn,

        -- Información de la agencia del sitio
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive

    FROM SchoolStaff ss
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN Agency a ON sch.AgencyId = a.Id

    WHERE ss.StaffId = @staffId
        AND ss.IsActive = 1
        AND sch.IsActive = 1
        AND a.IsActive = 1

    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END
GO

-- Procedimiento: Asignar empleado a sitio
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_AssignStaffToSchool')
    DROP PROCEDURE [dbo].[100_AssignStaffToSchool];
GO

CREATE PROCEDURE [dbo].[100_AssignStaffToSchool]
    @schoolId INT,
    @staffId INT,
    @assignmentTypeId INT,
    @isPrimary BIT = 0,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que el sitio existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @schoolId AND IsActive = 1)
        BEGIN
        RAISERROR ('El sitio especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que el empleado existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que el empleado es de tipo "Empleado" (StaffTypeId = 1)
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId AND StaffTypeId = 1)
        BEGIN
        RAISERROR ('Solo se pueden asignar empleados (StaffTypeId = 1) a sitios', 16, 1);
        RETURN;
    END
        
        -- Verificar que el tipo de asignación existe
        IF NOT EXISTS (SELECT 1
    FROM OptionSelection
    WHERE Id = @assignmentTypeId AND OptionKey = 'staffAssignmentType' AND IsActive = 1)
        BEGIN
        RAISERROR ('El tipo de asignación especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que no existe una asignación activa del mismo empleado al mismo sitio
        IF EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado ya está asignado activamente a este sitio', 16, 1);
        RETURN;
    END
        
        -- Si es asignación principal, desactivar otras asignaciones principales del sitio
        IF @isPrimary = 1
        BEGIN
        UPDATE SchoolStaff 
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND IsActive = 1;
    END
        
        -- Insertar la nueva asignación
        INSERT INTO SchoolStaff
        (
        SchoolId, StaffId, AssignmentDate, AssignmentTypeId,
        IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt
        )
    VALUES
        (
            @schoolId, @staffId, GETDATE(), @assignmentTypeId,
            @isPrimary, @startDate, @endDate, @comments, 1, GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Retornar el ID de la nueva asignación
        SELECT SCOPE_IDENTITY() AS Id;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- Procedimiento: Desasignar empleado de sitio
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_UnassignStaffFromSchool')
    DROP PROCEDURE [dbo].[100_UnassignStaffFromSchool];
GO

CREATE PROCEDURE [dbo].[100_UnassignStaffFromSchool]
    @schoolId INT,
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la asignación existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('La asignación especificada no existe o no está activa', 16, 1);
        RETURN;
    END
        
        -- Realizar baja lógica
        UPDATE SchoolStaff 
        SET IsActive = 0, UpdatedAt = GETDATE()
        WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1;
        
        COMMIT TRANSACTION;
        
        -- Retornar éxito
        SELECT 1 AS Success;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- Procedimiento: Actualizar asignación
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_UpdateSchoolStaff')
    DROP PROCEDURE [dbo].[100_UpdateSchoolStaff];
GO

CREATE PROCEDURE [dbo].[100_UpdateSchoolStaff]
    @id INT,
    @assignmentTypeId INT = NULL,
    @isPrimary BIT = NULL,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la asignación existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE Id = @id AND IsActive = 1)
        BEGIN
        RAISERROR ('La asignación especificada no existe o no está activa', 16, 1);
        RETURN;
    END
        
        -- Si se va a cambiar a asignación principal, desactivar otras asignaciones principales del mismo sitio
        IF @isPrimary = 1
        BEGIN
        DECLARE @schoolId INT;
        SELECT @schoolId = SchoolId
        FROM SchoolStaff
        WHERE Id = @id;

        UPDATE SchoolStaff 
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND Id != @id AND IsActive = 1;
    END
        
        -- Verificar que el tipo de asignación existe (si se va a cambiar)
        IF @assignmentTypeId IS NOT NULL
        BEGIN
        IF NOT EXISTS (SELECT 1
        FROM OptionSelection
        WHERE Id = @assignmentTypeId AND OptionKey = 'staffAssignmentType' AND IsActive = 1)
            BEGIN
            RAISERROR ('El tipo de asignación especificado no existe o no está activo', 16, 1);
            RETURN;
        END
    END
        
        -- Actualizar la asignación
        UPDATE SchoolStaff 
        SET 
            AssignmentTypeId = ISNULL(@assignmentTypeId, AssignmentTypeId),
            IsPrimary = ISNULL(@isPrimary, IsPrimary),
            StartDate = ISNULL(@startDate, StartDate),
            EndDate = ISNULL(@endDate, EndDate),
            Comments = ISNULL(@comments, Comments),
            UpdatedAt = GETDATE()
        WHERE Id = @id AND IsActive = 1;
        
        COMMIT TRANSACTION;
        
        -- Retornar éxito
        SELECT 1 AS Success;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- Procedimiento: Obtener asignación por ID
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME = '100_GetSchoolStaffById')
    DROP PROCEDURE [dbo].[100_GetSchoolStaffById];
GO

CREATE PROCEDURE [dbo].[100_GetSchoolStaffById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SchoolId,
        ss.StaffId,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt,

        -- Información del Staff
        s.FirstName AS StaffFirstName,
        s.MiddleName AS StaffMiddleName,
        s.FatherLastName AS StaffFatherLastName,
        s.MotherLastName AS StaffMotherLastName,
        s.Email AS StaffEmail,
        s.PositionId AS StaffPositionId,
        s.StaffTypeId,
        s.ContractStartDate,
        s.ContractEndDate,
        s.BirthDate,
        s.PostalAddress,
        s.CityId AS StaffCityId,
        s.RegionId AS StaffRegionId,
        s.AreaCode,
        s.Comments AS StaffComments,
        s.UserId AS StaffUserId,
        s.IsActive AS StaffIsActive,

        -- Información del Sitio
        sch.Name AS SchoolName,
        sch.Address AS SchoolAddress,
        sch.CityId AS SchoolCityId,
        sch.RegionId AS SchoolRegionId,
        sch.ZipCode AS SchoolZipCode,
        sch.Latitude,
        sch.Longitude,
        sch.PostalAddress AS SchoolPostalAddress,
        sch.PostalCityId AS SchoolPostalCityId,
        sch.PostalRegionId AS SchoolPostalRegionId,
        sch.PostalZipCode AS SchoolPostalZipCode,
        sch.SameAsPhysicalAddress,
        sch.OrganizationTypeId,
        sch.CenterTypeId,
        sch.NonProfit,
        sch.BaseYear,
        sch.RenewalYear,
        sch.OperatingFromDate,
        sch.OperatingToDate,
        sch.OperatingDaysCalculated,
        sch.KitchenTypeId,
        sch.GroupTypeId,
        sch.DeliveryTypeId,
        sch.SponsorTypeId,
        sch.ApplicantTypeId,
        sch.ResidentialTypeId,
        sch.OperatingPolicyId,
        sch.AreaTypeId,
        sch.HasWarehouse,
        sch.HasDiningRoom,
        sch.AdministratorAuthorizedName,
        sch.SitePhone,
        sch.Extension,
        sch.MobilePhone,
        sch.Breakfast,
        sch.BreakfastFrom,
        sch.BreakfastTo,
        sch.Lunch,
        sch.LunchFrom,
        sch.LunchTo,
        sch.Snack,
        sch.SnackFrom,
        sch.SnackTo,
        sch.Dinner,
        sch.DinnerFrom,
        sch.DinnerTo,
        sch.SnackNight,
        sch.SnackNightFrom,
        sch.SnackNightTo,
        sch.CommunityId,
        sch.WalkersId,
        sch.SiteTypeId,
        sch.ExperienceId,
        sch.ReviewResultId,
        sch.ReviewDate,
        sch.ReviewJustification,
        sch.IsActive AS SchoolIsActive,
        sch.InactiveJustification,
        sch.InactiveDate,
        sch.CreatedAt AS SchoolCreatedAt,
        sch.UpdatedAt AS SchoolUpdatedAt,

        -- Información de la Asignación
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEn,

        -- Información de la posición del staff
        pos.Name AS StaffPositionName,
        pos.NameEN AS StaffPositionNameEn,

        -- Información del tipo de staff
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,

        -- Información de la ciudad del staff
        c.Name AS StaffCityName,
        c.Name AS StaffCityNameEn,

        -- Información de la región del staff
        r.Name AS StaffRegionName,
        r.Name AS StaffRegionNameEn,

        -- Información de la agencia del sitio
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive

    FROM SchoolStaff ss
        INNER JOIN Staff s ON ss.StaffId = s.Id
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN OptionSelection pos ON s.PositionId = pos.Id
        INNER JOIN StaffType st ON s.StaffTypeId = st.Id
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        INNER JOIN Agency a ON sch.AgencyId = a.Id

    WHERE ss.Id = @id;
END
GO

PRINT 'Procedimientos almacenados creados exitosamente.';

-- =============================================
-- VERIFICACIÓN FINAL
-- =============================================
PRINT 'Verificando la migración...';

-- Verificar que la tabla existe
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'SchoolStaff')
    PRINT '✓ Tabla SchoolStaff creada correctamente';
ELSE
    PRINT '✗ ERROR: Tabla SchoolStaff no se creó';

-- Verificar que los tipos de asignación existen
IF EXISTS (SELECT 1
FROM OptionSelection
WHERE OptionKey = 'staffAssignmentType')
    PRINT '✓ Tipos de asignación insertados correctamente';
ELSE
    PRINT '✗ ERROR: Tipos de asignación no se insertaron';

-- Verificar que los procedimientos existen
DECLARE @procCount INT = 0;
SELECT @procCount = COUNT(*)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME IN ('100_GetStaffBySchool', '100_GetSchoolsByStaff', '100_AssignStaffToSchool', '100_UnassignStaffFromSchool', '100_UpdateSchoolStaff', '100_GetSchoolStaffById');

IF @procCount = 6
    PRINT '✓ Todos los procedimientos almacenados creados correctamente';
ELSE
    PRINT '✗ ERROR: Solo se crearon ' + CAST(@procCount AS VARCHAR) + ' de 6 procedimientos';

PRINT 'Migración de SchoolStaff completada.';
PRINT 'La funcionalidad está lista para usar.';
