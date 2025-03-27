-- Procedimiento almacenado para actualizar el programa de una agencia
CREATE OR ALTER PROCEDURE [102_UpdateAgencyProgram]
    @agencyId INT,
    @programId INT,
    @agencyStatusId INT,
    @userId NVARCHAR(36),
    @rejectionJustification NVARCHAR(MAX) = NULL,
    @appointmentCoordinated BIT = NULL,
    @appointmentDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el estado existe
    IF NOT EXISTS (SELECT 1 FROM AgencyStatus WHERE Id = @agencyStatusId)
    BEGIN
        RAISERROR ('El estado de agencia especificado no existe.', 16, 1);
        RETURN -1;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE AgencyProgram
        SET 
            ProgramId = @programId,
            UserId = @userId,
            Comments = @rejectionJustification,
            AppointmentCoordinated = @appointmentCoordinated,
            AppointmentDate = @appointmentDate,
            UpdatedAt = GETDATE()
        WHERE AgencyId = @agencyId;

        UPDATE Agency
        SET AgencyStatusId = @agencyStatusId
        WHERE Id = @agencyId;

        COMMIT TRANSACTION;
        RETURN 1; -- Éxito
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