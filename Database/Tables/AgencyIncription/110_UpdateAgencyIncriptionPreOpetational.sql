-- Procedimiento almacenado para actualizar la inscripción de una agencia desde formulario pre-operacional
CREATE OR ALTER PROCEDURE [110_UpdateAgencyIncriptionPreOpetational]
    @agencyId INT,
    @statusId INT,
    @comments NVARCHAR(MAX) = NULL,
    @appointmentCoordinated BIT = NULL,
    @appointmentDate DATETIME = NULL,
    @rejectionJustification NVARCHAR(MAX) = NULL
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
            RejectionJustification = @rejectionJustification,
            UpdatedAt = GETDATE()
        WHERE AgencyId = @agencyId;
        DECLARE @rowsAffectedInscription INT = @@ROWCOUNT; 

        UPDATE Agency
        SET AgencyStatusId = @statusId
        WHERE Id = @agencyId;
        DECLARE @rowsAffectedAgency INT = @@ROWCOUNT;

        SET @rowsAffected = @rowsAffectedInscription + @rowsAffectedAgency;

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