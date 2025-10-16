-- =============================================
-- Stored Procedure: 100_InsertSchool
-- Descripción: Inserta una nueva escuela con SchoolNumber y SchoolCode automáticos
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchool]
    @agencyId INT,
    @name NVARCHAR(255),
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @schoolNumber INT;
    DECLARE @schoolCode NVARCHAR(50);

    -- Obtener el siguiente SchoolNumber disponible para esta agencia
    SELECT @schoolNumber = ISNULL(MAX(SchoolNumber), 0) + 1
    FROM [School]
    WHERE AgencyId = @agencyId AND IsActive = 1;

    -- Generar SchoolCode basado en SchoolNumber (formato "01", "02", etc.)
    SET @schoolCode = RIGHT('00' + CAST(@schoolNumber AS VARCHAR(2)), 2);

    -- Insertar nueva escuela
    INSERT INTO [School]
        (
        [AgencyId],
        [Name],
        [SchoolCode],
        [SchoolNumber],
        [IsActive],
        [CreatedAt]
        )
    VALUES
        (
            @agencyId,
            @name,
            @schoolCode,
            @schoolNumber,
            @isActive,
            GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;