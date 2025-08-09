-- =============================================
-- Stored Procedure: 100_GetStaffTypeById
-- =============================================
-- Obtiene un tipo de staff específico por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        NameEn,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM StaffType
    WHERE Id = @id;
END