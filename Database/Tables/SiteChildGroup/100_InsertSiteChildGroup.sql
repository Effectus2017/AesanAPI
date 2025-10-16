-- =============================================
-- Stored Procedure: 100_InsertSiteChildGroup
-- Descripción: Inserta un grupo de niños específico para un sitio
-- Reemplaza: 100_InsertSchoolChildGroup
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteChildGroup]
    @siteId INT,
    @groupName NVARCHAR(255),
    @numberOfChildren INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteChildGroup
        (
        SiteId, GroupName, NumberOfChildren, CreatedAt
        )
    VALUES
        (
            @siteId, @groupName, @numberOfChildren, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;
