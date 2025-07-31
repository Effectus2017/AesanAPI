-- =============================================
-- Stored Procedure: 100_InsertStaffClassification
-- =============================================
-- Inserta una nueva clasificación de staff en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaffClassification]
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100),
    @sortOrder INT = 0,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO StaffClassification
        (Name, NameEn, SortOrder, IsActive, CreatedAt)
    VALUES
        (@name, @nameEn, @sortOrder, @isActive, GETDATE());

    SELECT SCOPE_IDENTITY() AS Id;
END 