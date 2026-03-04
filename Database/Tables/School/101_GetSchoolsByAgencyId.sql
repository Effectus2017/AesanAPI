-- =============================================
-- Stored Procedure: 101_GetSchoolsByAgencyId
-- Descripción: Obtiene todas las escuelas de una agencia específica con paginación, filtros y conteo.
--              OBLIGATORIO: ambos SELECT deben filtrar por WHERE s.[AgencyId] = @agencyId.
-- Fecha: 2026-03-03
-- Versión: 4.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetSchoolsByAgencyId]
    @agencyId INT,
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @isActive BIT = NULL,
    @schoolCode NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Retornar los datos de las escuelas con filtros y paginación
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
    WHERE s.[AgencyId] = @agencyId
        AND (@isActive IS NULL OR s.[IsActive] = @isActive)
        AND (@name IS NULL OR s.[Name] LIKE '%' + @name + '%')
        AND (@schoolCode IS NULL OR 
            s.[SchoolCode] LIKE '%' + @schoolCode + '%' OR 
            (RIGHT(a.[AgencyCode], 3) + '-' + s.[SchoolCode]) LIKE '%' + @schoolCode + '%')
    ORDER BY s.[CreatedAt] DESC, s.[SchoolNumber] DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retornar el conteo total con los mismos filtros
    SELECT COUNT(*)
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE s.[AgencyId] = @agencyId
        AND (@isActive IS NULL OR s.[IsActive] = @isActive)
        AND (@name IS NULL OR s.[Name] LIKE '%' + @name + '%')
        AND (@schoolCode IS NULL OR 
            s.[SchoolCode] LIKE '%' + @schoolCode + '%' OR 
            (RIGHT(a.[AgencyCode], 3) + '-' + s.[SchoolCode]) LIKE '%' + @schoolCode + '%');
END;
