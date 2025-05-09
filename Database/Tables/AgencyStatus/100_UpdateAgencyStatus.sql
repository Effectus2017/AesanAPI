-- 100_UpdateAgencyStatus.sql
-- Actualiza un estado de agencia en la tabla AgencyStatus
-- Convención: nombre del SP en PascalCase, parámetros y alias en lowercase
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_UpdateAgencyStatus]
    @id INT,
    @name NVARCHAR(255),
    @nameen NVARCHAR(255),
    @isactive BIT,
    @displayorder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE AgencyStatus
    SET Name = @name,
        NameEN = @nameen,
        IsActive = @isactive,
        DisplayOrder = @displayorder,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END;