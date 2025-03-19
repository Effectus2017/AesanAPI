CREATE OR ALTER PROCEDURE [dbo].[101_AssignAgencyToUser]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450),
    @isOwner BIT,
    @isMonitor BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;

    -- si ya tiene una asignación un monitor, eliminarla
    IF EXISTS (SELECT 1 FROM AgencyUsers WHERE AgencyId = @agencyId AND IsMonitor = 1)
    BEGIN
        DELETE FROM AgencyUsers WHERE AgencyId = @agencyId AND IsMonitor = 1;
    END

    -- Insertar la nueva asignación
    INSERT INTO AgencyUsers (UserId, AgencyId, AssignedBy, IsOwner, IsMonitor)
    VALUES (@userId, @agencyId, @assignedBy, @isOwner, @isMonitor);
    
    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END; 