CREATE OR ALTER PROCEDURE [dbo].[100_AssignAgencyToUser]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;
    
    -- Eliminar cualquier asignación existente para esta agencia
    DELETE FROM UserAgencyAssignment 
    WHERE AgencyId = @agencyId;
    
    -- Insertar la nueva asignación
    INSERT INTO UserAgencyAssignment (UserId, AgencyId, AssignedBy)
    VALUES (@userId, @agencyId, @assignedBy);
    
    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END; 