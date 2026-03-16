-- =============================================
-- Stored Procedure: 101_GetSchoolSitesBySchoolId
-- Descripción: Obtiene todos los Sites asignados a una School específica con paginación.
--              SiteCode mostrado como XXX-XX-X (agencia-escuela-ordinal por escuela).
-- Fecha: 2026-03-13
-- Versión: 2.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetSchoolSitesBySchoolId]
    @schoolId INT,
    @take INT = 50,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @name = LTRIM(RTRIM(ISNULL(@name, '')));
    IF @name = ''
        SET @name = NULL;

    ;WITH Ranked AS (
        SELECT
            ss.[Id],
            ss.[SchoolId],
            ss.[SiteId],
            ss.[AssignmentDate],
            ss.[Comment],
            ss.[CreatedAt],
            ss.[UpdatedAt],
            s.[Name] AS SiteName,
            s.[SiteNumber],
            s.[Address],
            s.[IsActive],
            s.[SiteCode],
            s.[OperatingFromDate],
            s.[OperatingToDate],
            s.[ReviewDate],
            a.[AgencyCode],
            sch.[SchoolCode],
            gt.[Id] AS GroupTypeId,
            gt.[Name] AS GroupTypeName,
            gt.[NameEN] AS GroupTypeNameEN,
            ROW_NUMBER() OVER (ORDER BY s.[SiteNumber], s.[Id]) AS SiteOrdinalInSchool
        FROM [SchoolSite] ss
            INNER JOIN [Site] s ON ss.[SiteId] = s.[Id]
            INNER JOIN [School] sch ON ss.[SchoolId] = sch.[Id]
            LEFT JOIN [Agency] a ON s.[AgencyId] = a.[Id]
            LEFT JOIN [GroupType] gt ON s.[GroupTypeId] = gt.[Id]
        WHERE ss.[SchoolId] = @schoolId
            AND ss.[IsActive] = 1
            AND s.[IsActive] = 1
            AND (@name IS NULL OR LTRIM(RTRIM(s.[Name])) LIKE '%' + @name + '%')
    )
    SELECT
        r.[Id],
        r.[SchoolId],
        r.[SiteId],
        r.[AssignmentDate],
        r.[Comment],
        r.[IsActive],
        r.[CreatedAt],
        r.[UpdatedAt],
        r.[SiteName],
        SiteCode = CASE
            WHEN r.[AgencyCode] IS NOT NULL AND r.[SchoolCode] IS NOT NULL AND r.[SiteCode] IS NOT NULL
            THEN RIGHT(r.[AgencyCode], 3) + '-' + r.[SchoolCode] + '-' + CAST(r.[SiteOrdinalInSchool] AS VARCHAR(10))
            ELSE r.[SiteCode]
        END,
        r.[SiteNumber],
        r.[Address],
        r.[GroupTypeId],
        r.[GroupTypeName],
        r.[GroupTypeNameEN],
        r.[OperatingFromDate],
        r.[OperatingToDate],
        ApprovalDate = r.[ReviewDate]
    FROM Ranked r
    ORDER BY r.[SiteName] ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM [SchoolSite] ss
        INNER JOIN [Site] s ON ss.[SiteId] = s.[Id]
    WHERE ss.[SchoolId] = @schoolId
        AND ss.[IsActive] = 1
        AND s.[IsActive] = 1
        AND (@name IS NULL OR LTRIM(RTRIM(s.[Name])) LIKE '%' + @name + '%');
END;
