CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolParticipant]
    @schoolId INT,
    @participantTypeId INT,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SchoolParticipant
        (
        SchoolId, ParticipantTypeId, IsActive, CreatedAt
        )
    VALUES
        (
            @schoolId, @participantTypeId, @isActive, GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;
