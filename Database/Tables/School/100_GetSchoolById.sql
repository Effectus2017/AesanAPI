-- =============================================
-- Stored Procedure: 100_GetSchoolById
-- Descripción: Obtiene una escuela por su ID
-- Fecha: 2025-10-15
-- Versión: 1.1
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

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
        a.[AgencyCode] AS AgencyCode
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE s.[Id] = @id;
END;
