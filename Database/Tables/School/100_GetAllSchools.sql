-- =============================================
-- Stored Procedure: 100_GetAllSchools
-- Descripción: Obtiene todas las escuelas con paginación y filtros
-- Fecha: 2025-10-15
-- Versión: 1.1
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllSchools]
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Query principal con filtros
    SELECT
        s.[Id],
        s.[AgencyId],
        s.[Name],
        -- Formatear SchoolCode como XXX-XX (últimos 3 dígitos de agencia - código de escuela)
        CASE 
            WHEN a.[AgencyCode] IS NOT NULL AND s.[SchoolCode] IS NOT NULL 
            THEN RIGHT(a.[AgencyCode], 3) + '-' + s.[SchoolCode]
            ELSE s.[SchoolCode]
        END AS [SchoolCode],
        s.[SchoolNumber],
        s.[IsActive],
        s.[CreatedAt],
        s.[UpdatedAt],
        a.[Name] AS AgencyName,
        a.[AgencyCode] AS AgencyCode,
        (SELECT COUNT(*)
        FROM SchoolSite ss
        WHERE ss.SchoolId = s.Id AND ss.IsActive = 1) AS SitesCount
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE s.[IsActive] = 1
        AND (
            @alls = 1
        OR (
                (@name IS NULL OR s.[Name] LIKE '%' + @name + '%')
        AND (@agencyId IS NULL OR s.[AgencyId] = @agencyId)
            )
        )
    ORDER BY s.[Name] ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Query para contar total
    SELECT COUNT(*)
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE s.[IsActive] = 1
        AND (
            @alls = 1
        OR (
                (@name IS NULL OR s.[Name] LIKE '%' + @name + '%')
        AND (@agencyId IS NULL OR s.[AgencyId] = @agencyId)
            )
        );
END;
