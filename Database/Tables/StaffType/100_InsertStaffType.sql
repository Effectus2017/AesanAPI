-- =============================================
-- Stored Procedure: 100_InsertStaffType
-- =============================================
-- Crea un nuevo tipo de staff en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaffType]
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100),
    @sortOrder INT = 0,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO StaffType
        (
        Name,
        NameEn,
        SortOrder,
        CreatedAt,
        IsActive
        )
    VALUES
        (
            @name,
            @nameEn,
            @sortOrder,
            GETDATE(),
            1
    );

    SET @id = SCOPE_IDENTITY();
END