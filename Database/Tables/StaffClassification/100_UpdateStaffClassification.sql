-- =============================================
-- Stored Procedure: 100_UpdateStaffClassification
-- =============================================
-- Actualiza una clasificación de staff existente en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffClassification]
    @id INT,
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100),
    @sortOrder INT = 0,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE StaffClassification
    SET Name = @name,
        NameEn = @nameEn,
        SortOrder = @sortOrder,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SELECT @@ROWCOUNT AS RowsAffected;
END 