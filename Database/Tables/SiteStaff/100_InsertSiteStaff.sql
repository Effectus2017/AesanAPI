-- =============================================
-- Stored Procedure: 100_InsertSiteStaff
-- Descripción: Inserta una asignación de personal a un sitio
-- Reemplaza: 100_InsertSchoolStaff
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteStaff]
    @siteId INT,
    @staffId INT,
    @assignmentTypeId INT = 1,
    @isPrimary BIT = 0,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteStaff
        (
        SiteId, StaffId, AssignmentDate, AssignmentTypeId, IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt
        )
    VALUES
        (
            @siteId, @staffId, GETDATE(), @assignmentTypeId, @isPrimary, @startDate, @endDate, @comments, @isActive, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;
