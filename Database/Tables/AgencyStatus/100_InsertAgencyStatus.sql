-- 100_InsertAgencyStatus.sql
-- Inserta un nuevo estado de agencia
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_InsertAgencyStatus]
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AgencyStatus (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES (@name, @nameEN, @isActive, @displayOrder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 