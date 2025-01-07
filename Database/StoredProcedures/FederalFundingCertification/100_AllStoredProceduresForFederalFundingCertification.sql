CREATE OR ALTER PROCEDURE [dbo].[100_GetAllFederalFundingCertifications]
    @take INT,
    @skip INT,
    @description NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           FundingAmount,
           Description,
           UpdatedAt
    FROM FederalFundingCertification
     WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%')
    ORDER BY UpdatedAt DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM FederalFundingCertification
    WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%')
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertFederalFundingCertification]
    @fundingAmount DECIMAL(18,2),
    @description NVARCHAR(MAX),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO FederalFundingCertification (FundingAmount, Description, UpdatedAt)
    VALUES (@fundingAmount, @description, GETUTCDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteFederalFundingCertification]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM FederalFundingCertification
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_GetFederalFundingCertificationById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           FundingAmount,
           Description,
           UpdatedAt
    FROM FederalFundingCertification
    WHERE Id = @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateFederalFundingCertification]
    @id INT,
    @fundingAmount DECIMAL(18,2),
    @description NVARCHAR(MAX),
    @updatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE FederalFundingCertification
        SET FundingAmount = @fundingAmount,
            Description = @description,
            UpdatedAt = @updatedAt
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO 