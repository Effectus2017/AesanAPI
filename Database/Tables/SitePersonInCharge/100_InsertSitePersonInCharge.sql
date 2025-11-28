-- =============================================
-- Stored Procedure: 100_InsertSitePersonInCharge
-- Descripción: Inserta información de Persona a Cargo para un sitio
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSitePersonInCharge]
    @siteId INT,
    @firstName NVARCHAR(100) = NULL,
    @middleName NVARCHAR(100) = NULL,
    @fatherLastName NVARCHAR(100) = NULL,
    @motherLastName NVARCHAR(100) = NULL,
    @sitePhone NVARCHAR(20) = NULL,
    @extension NVARCHAR(10) = NULL,
    @mobilePhone NVARCHAR(20) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SitePersonInCharge
        (
        SiteId, FirstName, MiddleName, FatherLastName, MotherLastName,
        SitePhone, Extension, MobilePhone, CreatedAt
        )
    VALUES
        (
            @siteId, @firstName, @middleName, @fatherLastName, @motherLastName,
            @sitePhone, @extension, @mobilePhone, GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;

