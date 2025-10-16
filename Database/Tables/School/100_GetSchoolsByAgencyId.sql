-- =============================================
-- Stored Procedure: 100_GetSchoolsByAgencyId
-- Descripción: Obtiene todas las escuelas de una agencia específica con conteo
-- Fecha: 2025-10-15
-- Versión: 2.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolsByAgencyId]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Retornar los datos de las escuelas
    SELECT
        s.[Id],
        s.[AgencyId],
        s.[Name],
        s.[SchoolCode],
        s.[SchoolNumber],
        s.[IsActive],
        s.[CreatedAt],
        s.[UpdatedAt],
        a.[Name] AS AgencyName,
        a.[AgencyCode] AS AgencyCode
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE s.[AgencyId] = @agencyId
        AND s.[IsActive] = 1
    ORDER BY s.[Name] ASC;

    -- Retornar el conteo total
    SELECT COUNT(*)
    FROM [School] s
    WHERE s.[AgencyId] = @agencyId
        AND s.[IsActive] = 1;
END;

--EXEC [dbo].[100_GetSchoolsByAgencyId] @agencyId = 1;