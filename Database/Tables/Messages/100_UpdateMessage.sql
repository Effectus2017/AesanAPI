-- =============================================
-- Stored Procedure: 100_UpdateMessage
-- =============================================
-- Actualiza un mensaje existente en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateMessage]
    @id INT,
    @icon NVARCHAR(255) = NULL,
    @image NVARCHAR(500) = NULL,
    @title NVARCHAR(255) = NULL,
    @description NVARCHAR(1000) = NULL,
    @time DATETIME = NULL,
    @link NVARCHAR(500) = NULL,
    @useRouter BIT = NULL,
    @read BIT = NULL,
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Messages
    SET
        Icon = ISNULL(@icon, Icon),
        Image = ISNULL(@image, Image),
        Title = ISNULL(@title, Title),
        Description = ISNULL(@description, Description),
        Time = ISNULL(@time, Time),
        Link = ISNULL(@link, Link),
        UseRouter = ISNULL(@useRouter, UseRouter),
        [Read] = ISNULL(@read, [Read]),
        UserId = ISNULL(@userId, UserId),
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsDeleted = 0;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 