-- =============================================
-- Stored Procedure: 100_InsertSiteProgram
-- Descripción: Inserta una nueva relación sitio-programa
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteProgram]
    @siteId INT,
    @programId INT,
    @startDate DATE,
    @endDate DATE,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteProgram
        (SiteId, ProgramId, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (@siteId, @programId, @startDate, @endDate, @isActive, GETDATE(), NULL);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

