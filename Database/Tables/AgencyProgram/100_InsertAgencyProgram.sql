-- Procedimiento almacenado para insertar un programa de agencia
-- Previene duplicados verificando si ya existe el registro
CREATE OR ALTER PROCEDURE [100_InsertAgencyProgram]
    @agencyId INT,
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si ya existe el registro
    IF NOT EXISTS (SELECT 1
    FROM AgencyProgram
    WHERE AgencyId = @agencyId AND ProgramId = @programId)
    BEGIN
        INSERT INTO AgencyProgram
            (AgencyId, ProgramId)
        VALUES
            (@agencyId, @programId);
    END
END
GO
