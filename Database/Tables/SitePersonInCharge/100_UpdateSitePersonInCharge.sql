-- =============================================
-- Stored Procedure: 100_UpdateSitePersonInCharge
-- Descripción: Actualiza información de Persona a Cargo para un sitio
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSitePersonInCharge]
    @siteId INT,
    @firstName NVARCHAR(100) = NULL,
    @middleName NVARCHAR(100) = NULL,
    @fatherLastName NVARCHAR(100) = NULL,
    @motherLastName NVARCHAR(100) = NULL,
    @sitePhone NVARCHAR(20) = NULL,
    @extension NVARCHAR(10) = NULL,
    @mobilePhone NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE SitePersonInCharge 
        SET 
            FirstName = @firstName,
            MiddleName = @middleName,
            FatherLastName = @fatherLastName,
            MotherLastName = @motherLastName,
            SitePhone = @sitePhone,
            Extension = @extension,
            MobilePhone = @mobilePhone,
            UpdatedAt = GETDATE()
        WHERE SiteId = @siteId;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

