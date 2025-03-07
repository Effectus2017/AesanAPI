-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:16
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/School/100_AllStoredProceduresForSchool-2.sql
-- =============================================

-- Procedimiento para obtener todas las escuelas
CREATE OR ALTER PROCEDURE [100_GetSchools]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @educationLevelId INT = NULL,
    @operatingPeriodId INT = NULL,
    @organizationTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.Name,
        -- Nivel Educativo
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        -- Período Operativo
        s.OperatingPeriodId,
        op.Name AS OperatingPeriodName,
        -- Dirección Física
        s.Address,
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        -- Tipo de Organización
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName,
        -- Auditoría
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
        LEFT JOIN OperatingPeriod op ON s.OperatingPeriodId = op.Id
        LEFT JOIN CityRegion cr ON s.CityRegionId = cr.Id
        LEFT JOIN City c ON cr.CityId = c.Id
        LEFT JOIN Region r ON cr.RegionId = r.Id
        LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
    WHERE (@alls = 1 OR s.IsActive = 1)
        AND (@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR cr.RegionId = @regionId)
        AND (@cityId IS NULL OR cr.CityId = @cityId)
        AND (@educationLevelId IS NULL OR s.EducationLevelId = @educationLevelId)
        AND (@operatingPeriodId IS NULL OR s.OperatingPeriodId = @operatingPeriodId)
        AND (@organizationTypeId IS NULL OR s.OrganizationTypeId = @organizationTypeId)
    ORDER BY s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM School s
        LEFT JOIN CityRegion cr ON s.CityRegionId = cr.Id
    WHERE (@alls = 1 OR s.IsActive = 1)
        AND (@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR cr.RegionId = @regionId)
        AND (@cityId IS NULL OR cr.CityId = @cityId)
        AND (@educationLevelId IS NULL OR s.EducationLevelId = @educationLevelId)
        AND (@operatingPeriodId IS NULL OR s.OperatingPeriodId = @operatingPeriodId)
        AND (@organizationTypeId IS NULL OR s.OrganizationTypeId = @organizationTypeId);
END;
GO

-- Procedimiento para obtener una escuela por ID
CREATE OR ALTER PROCEDURE [100_GetSchoolById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.Name,
        -- Nivel Educativo
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        -- Período Operativo
        s.OperatingPeriodId,
        op.Name AS OperatingPeriodName,
        -- Dirección Física
        s.Address,
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        -- Tipo de Organización
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName,
        -- Auditoría
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
        LEFT JOIN OperatingPeriod op ON s.OperatingPeriodId = op.Id
        LEFT JOIN CityRegion cr ON s.CityRegionId = cr.Id
        LEFT JOIN City c ON cr.CityId = c.Id
        LEFT JOIN Region r ON cr.RegionId = r.Id
        LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
    WHERE s.Id = @id AND s.IsActive = 1;
END;
GO

-- Procedimiento para insertar una escuela
CREATE OR ALTER PROCEDURE [100_InsertSchool]
    @name NVARCHAR(255),
    @educationLevelId INT,
    @operatingPeriodId INT,
    @address NVARCHAR(255),
    @cityId INT,
    @regionId INT,
    @zipCode NVARCHAR(10),
    @organizationTypeId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que exista la relación CityRegion
    DECLARE @cityRegionId INT;
    SELECT @cityRegionId = Id 
    FROM CityRegion 
    WHERE CityId = @cityId AND RegionId = @regionId AND IsActive = 1;

    IF @cityRegionId IS NULL
    BEGIN
        RAISERROR ('La relación entre la ciudad y la región especificada no existe o no está activa', 16, 1);
        RETURN -1;
    END

    INSERT INTO School (
        Name,
        EducationLevelId,
        OperatingPeriodId,
        Address,
        CityRegionId,
        ZipCode,
        OrganizationTypeId,
        IsActive,
        CreatedAt
    )
    VALUES (
        @name,
        @educationLevelId,
        @operatingPeriodId,
        @address,
        @cityRegionId,
        @zipCode,
        @organizationTypeId,
        1,
        GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

-- Procedimiento para actualizar una escuela
CREATE OR ALTER PROCEDURE [100_UpdateSchool]
    @id INT,
    @name NVARCHAR(255),
    @educationLevelId INT,
    @operatingPeriodId INT,
    @address NVARCHAR(255),
    @cityId INT,
    @regionId INT,
    @zipCode NVARCHAR(10),
    @organizationTypeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Validar que exista la relación CityRegion
    DECLARE @cityRegionId INT;
    SELECT @cityRegionId = Id 
    FROM CityRegion 
    WHERE CityId = @cityId AND RegionId = @regionId AND IsActive = 1;

    IF @cityRegionId IS NULL
    BEGIN
        RAISERROR ('La relación entre la ciudad y la región especificada no existe o no está activa', 16, 1);
        RETURN -1;
    END

    UPDATE School
    SET Name = @name,
        EducationLevelId = @educationLevelId,
        OperatingPeriodId = @operatingPeriodId,
        Address = @address,
        CityRegionId = @cityRegionId,
        ZipCode = @zipCode,
        OrganizationTypeId = @organizationTypeId,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

-- Procedimiento para eliminar una escuela
CREATE OR ALTER PROCEDURE [100_DeleteSchool]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE School
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 
GO
