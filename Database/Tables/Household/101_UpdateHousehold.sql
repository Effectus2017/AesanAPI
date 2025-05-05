CREATE OR ALTER PROCEDURE [dbo].[101_UpdateHousehold]
    @Id INT,
    @Street NVARCHAR(200),
    @Apartment NVARCHAR(50) = NULL,
    @CityId INT,
    @RegionId INT,
    @ZipCode NVARCHAR(20),
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @CompletedBy NVARCHAR(100),
    @CompletedDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Household
    SET Street = @Street,
        Apartment = @Apartment,
        CityId = @CityId,
        RegionId = @RegionId,
        ZipCode = @ZipCode,
        Phone = @Phone,
        Email = @Email,
        CompletedBy = @CompletedBy,
        CompletedDate = @CompletedDate,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 