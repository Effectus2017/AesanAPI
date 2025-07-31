-- =============================================
-- Stored Procedure: 100_UpdateStaffType
-- =============================================
-- Actualiza un tipo de staff existente en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffType]
    @id INT,
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100),
    @displayOrder INT,
    @isActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE StaffType
    SET
        Name = @name,
        NameEn = @nameEn,
        DisplayOrder = @displayOrder,
        IsActive = ISNULL(@isActive, IsActive),
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END