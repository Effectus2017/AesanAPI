CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolParticipants]
    @schoolId INT,
    @participantTypeIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Eliminar participantes existentes para esta escuela
    DELETE FROM SchoolParticipant WHERE SchoolId = @schoolId;

    -- Insertar nuevos participantes
    INSERT INTO SchoolParticipant (SchoolId, ParticipantTypeId, IsActive, CreatedAt)
    SELECT 
        @schoolId,
        CAST(value AS INT),
        1,
        GETDATE()
    FROM STRING_SPLIT(@participantTypeIds, ',')
    WHERE value != '' AND value IS NOT NULL;
END;
