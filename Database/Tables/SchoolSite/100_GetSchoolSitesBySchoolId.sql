-- =============================================
-- Stored Procedure: 100_GetSchoolSitesBySchoolId
-- Descripción: Obtiene todos los Sites asignados a una School específica con paginación
-- Fecha: 2025-10-15
-- Versión: 1.4
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolSitesBySchoolId]
    @schoolId INT,
    @take INT = 50,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Limpiar el parámetro de búsqueda (eliminar espacios al inicio y final)
    SET @name = LTRIM(RTRIM(@name));
    
    -- Si después de limpiar está vacío, establecer como NULL
    IF @name = ''
        SET @name = NULL;

    -- Retorna los datos con paginación
    SELECT
        ss.[Id],
        ss.[SchoolId],
        ss.[SiteId],
        ss.[AssignmentDate],
        ss.[Comment],
        ss.[IsActive],
        ss.[CreatedAt],
        ss.[UpdatedAt],
        s.[Name] AS SiteName,
        s.[SiteCode],
        s.[SiteNumber],
        s.[Address],
        s.[IsActive],
        -- Tipo de Grupo
        gt.[Id] AS GroupTypeId,
        gt.[Name] AS GroupTypeName,
        gt.[NameEN] AS GroupTypeNameEN,
        -- Días de Funcionamiento
        s.[OperatingFromDate],
        s.[OperatingToDate],
        -- Fecha de Aprobación
        s.[ReviewDate] AS ApprovalDate
    FROM [SchoolSite] ss
        INNER JOIN [Site] s ON ss.[SiteId] = s.[Id]
        LEFT JOIN [GroupType] gt ON s.[GroupTypeId] = gt.[Id]
    WHERE ss.[SchoolId] = @schoolId
        AND ss.[IsActive] = 1
        AND s.[IsActive] = 1
        AND (@name IS NULL OR LTRIM(RTRIM(s.[Name])) LIKE '%' + @name + '%')
    ORDER BY s.[Name] ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retorna el conteo total para paginación
    SELECT COUNT(*)
    FROM [SchoolSite] ss
        INNER JOIN [Site] s ON ss.[SiteId] = s.[Id]
    WHERE ss.[SchoolId] = @schoolId
        AND ss.[IsActive] = 1
        AND s.[IsActive] = 1
        AND (@name IS NULL OR LTRIM(RTRIM(s.[Name])) LIKE '%' + @name + '%');
END;
