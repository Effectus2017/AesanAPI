-- Procedimiento almacenado para actualizar la fecha de registro completado de una agencia
-- Stored procedure to update the completed registration date of an agency
-- 1.1.4

CREATE OR ALTER PROCEDURE [110_UpdateCompletedRegistrationDate]
    @agencyId INT,
    @completedRegistrationDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE AgencyInscription
        SET 
            CompletedRegistrationDate = @completedRegistrationDate,
            UpdatedAt = GETDATE()
        WHERE AgencyId = @agencyId;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;
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
