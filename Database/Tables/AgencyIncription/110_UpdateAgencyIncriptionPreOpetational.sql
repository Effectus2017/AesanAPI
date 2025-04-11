-- Procedimiento almacenado para actualizar la inscripción de una agencia desde formulario pre-operacional
CREATE OR ALTER PROCEDURE [110_UpdateAgencyIncriptionPreOpetational]
    @agencyId INT,
    @statusId INT,
    @comments NVARCHAR(MAX) = NULL,
    @appointmentCoordinated BIT = NULL,
    @appointmentDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE AgencyInscription
        SET 
            Comments = @comments,
            AppointmentCoordinated = @appointmentCoordinated,
            AppointmentDate = @appointmentDate,
            UpdatedAt = GETDATE()
        WHERE AgencyId = @agencyId;

        UPDATE Agency
        SET AgencyStatusId = @statusId
        WHERE Id = @agencyId;

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