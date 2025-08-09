-- =============================================
-- Stored Procedure: 100_InsertMessage
-- =============================================
-- Crea un nuevo mensaje en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_InsertMessage]
    @icon NVARCHAR(255) = NULL,
    @image NVARCHAR(500) = NULL,
    @title NVARCHAR(255),
    @description NVARCHAR(1000) = NULL,
    @time DATETIME,
    @link NVARCHAR(500) = NULL,
    @useRouter BIT = 0,
    @read BIT = 0,
    @userId NVARCHAR(450) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Messages
        (
        Icon,
        Image,
        Title,
        Description,
        Time,
        Link,
        UseRouter,
        [Read],
        UserId,
        CreatedAt,
        UpdatedAt,
        IsDeleted
        )
    VALUES
        (
            @icon,
            @image,
            @title,
            @description,
            @time,
            @link,
            @useRouter,
            @read,
            @userId,
            GETDATE(),
            GETDATE(),
            0
    );

    SET @id = SCOPE_IDENTITY();
END 