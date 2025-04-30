-- Procedimiento almacenado para insertar un programa de agencia
CREATE OR ALTER PROCEDURE [100_InsertAgencyProgram]
    @agencyId INT,
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AgencyProgram
        (AgencyId, ProgramId)
    VALUES
        (@agencyId, @programId);
END
GO
