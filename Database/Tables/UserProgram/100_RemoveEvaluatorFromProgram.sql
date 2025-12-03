CREATE OR ALTER PROCEDURE [dbo].[100_RemoveEvaluatorFromProgram]
    @userId NVARCHAR(450),
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la asignación existe
    IF NOT EXISTS (
        SELECT 1
    FROM UserProgram
    WHERE UserId = @userId
        AND ProgramId = @programId
        AND IsActive = 1
    )
    BEGIN
        RAISERROR('La asignación especificada no existe o ya está inactiva.', 16, 1);
        RETURN -1;
    END

    -- Marcar como inactiva en lugar de eliminar físicamente
    UPDATE UserProgram
    SET IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE UserId = @userId
        AND ProgramId = @programId
        AND IsActive = 1;

    -- Retornar éxito
    SELECT 1 AS Success;
END
GO

