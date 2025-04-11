-- Procedimiento almacenado para actualizar el programa de una agencia
CREATE OR ALTER PROCEDURE [110_UpdateAgencyProgram]
    @agencyId INT,
    @programId INT,
    @userId NVARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE AgencyProgram
        SET 
            ProgramId = @programId,
            UserId = @userId,
            UpdatedAt = GETDATE()
        WHERE AgencyId = @agencyId;

        COMMIT TRANSACTION;
        SET @rowsAffected = @@ROWCOUNT;
        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Registrar el error y relanzarlo
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1;
    END CATCH;
END;
GO