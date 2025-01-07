CREATE OR ALTER PROCEDURE [dbo].[100_InsertFoodAuthority]
    @name NVARCHAR(100),
    @description NVARCHAR(MAX),
    @createdAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.FoodAuthority (Name, Description, CreatedAt)
    VALUES (@name, @description, @createdAt);

    SELECT SCOPE_IDENTITY() AS Id;
END; 