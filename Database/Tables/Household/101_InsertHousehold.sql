CREATE OR ALTER PROCEDURE [dbo].[101_InsertHousehold]
    @Street NVARCHAR(200),
    @Apartment NVARCHAR(50) = NULL,
    @CityId INT,
    @RegionId INT,
    @ZipCode NVARCHAR(20),
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @CompletedBy NVARCHAR(100),
    @CompletedDate DATE,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Household (
        Street, Apartment, CityId, RegionId, ZipCode, Phone, Email, CompletedBy, CompletedDate, IsActive, CreatedAt
    )
    VALUES (
        @Street, @Apartment, @CityId, @RegionId, @ZipCode, @Phone, @Email, @CompletedBy, @CompletedDate, 1, GETDATE()
    );
    SET @Id = SCOPE_IDENTITY();
END; 