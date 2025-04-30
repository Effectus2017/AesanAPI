-- Procedimiento para insertar un nuevo programa
CREATE OR ALTER PROCEDURE [100_InsertProgram]
    @name NVARCHAR(255),
    @description NVARCHAR(MAX),
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Program (
        Name,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        @name,
        @description,
        @isActive,
        GETDATE(),
        NULL
    );

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO 