SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
ALTER PROCEDURE [dbo].[101_UpdateUserMainAgency]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @oldAgencyId INT;
    
    -- Obtener la agencia principal actual
    SELECT @oldAgencyId = AgencyId
    FROM AgencyUsers
    WHERE UserId = @userId AND IsOwner = 1;
    
    -- Si la agencia es diferente, actualizar
    IF @oldAgencyId != @agencyId
    BEGIN
        -- Desactivar la asignación anterior
        UPDATE AgencyUsers
        SET IsActive = 0,
            UpdatedAt = GETUTCDATE(),
            AssignedBy = @assignedBy
        WHERE UserId = @userId AND AgencyId = @oldAgencyId AND IsOwner = 1;
        
        -- Crear nueva asignación
        INSERT INTO AgencyUsers (
            UserId,
            AgencyId,
            IsOwner,
            IsMonitor,
            IsActive,
            CreatedAt,
            AssignedBy
        )
        VALUES (
            @userId,
            @agencyId,
            1,          -- IsOwner
            0,          -- IsMonitor
            1,          -- IsActive
            GETUTCDATE(),
            @assignedBy
        );
        
        SELECT SCOPE_IDENTITY() AS Id;
    END
    ELSE
    BEGIN
        SELECT @oldAgencyId AS Id;
    END
END
GO