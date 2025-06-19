CREATE OR ALTER PROCEDURE [dbo].[103_UpdateSchoolActiveStatus]
    @id INT,
    @isActive BIT,
    @inactiveJustification NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Si se está inactivando, se requiere justificación
    IF @isActive = 0 AND (@inactiveJustification IS NULL OR LTRIM(RTRIM(@inactiveJustification)) = '')
    BEGIN
        RAISERROR ('Se requiere justificación para inactivar el sitio', 16, 1);
        RETURN -1;
    END

    -- Si se está activando, limpiar justificación y fecha de inactivación
    IF @isActive = 1
    BEGIN
        UPDATE School
        SET IsActive = @isActive,
            InactiveJustification = NULL,
            InactiveDate = NULL,
            UpdatedAt = GETDATE()
        WHERE Id = @id;
    END
    ELSE
    BEGIN
        -- Si se está inactivando, agregar justificación y fecha
        UPDATE School
        SET IsActive = @isActive,
            InactiveJustification = @inactiveJustification,
            InactiveDate = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Id = @id;
    END

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 