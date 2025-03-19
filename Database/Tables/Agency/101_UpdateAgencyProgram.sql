-- Procedimiento almacenado para actualizar el programa de una agencia
CREATE OR ALTER PROCEDURE [101_UpdateAgencyProgram]
    @AgencyId INT,
    @ProgramId INT,
    @AgencyStatusId INT,
    @UserId NVARCHAR(36),
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AgencyProgram
    SET 
        ProgramId = @ProgramId,
        UserId = @UserId,
        Comments = @RejectionJustification,
        AppointmentCoordinated = @AppointmentCoordinated,
        AppointmentDate = @AppointmentDate,
        UpdatedAt = GETDATE()
    WHERE AgencyId = @AgencyId AND ProgramId = @ProgramId;

    UPDATE Agency
    SET AgencyStatusId = @AgencyStatusId
    WHERE Id = @AgencyId;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
GO