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
    DECLARE @userRole NVARCHAR(50);

    -- Obtener el rol del usuario
    SELECT TOP 1 @userRole = r.Name
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId;

    -- Validar que un monitor no pueda ser owner
    IF @isMonitor = 1 AND @isOwner = 1
    BEGIN
        RAISERROR ('Un monitor no puede ser propietario de una agencia.', 16, 1);
        RETURN -1;
    END

    -- Validar que solo agency-administrator o agency-user puedan ser owners
    IF @isOwner = 1 AND @userRole NOT IN ('Agency-Administrator', 'Agency-User')
    BEGIN
        RAISERROR ('Solo un Agency-Administrator o Agency-User puede ser propietario de una agencia.', 16, 1);
        RETURN -2;
    END

    -- Desactivar asignaciones anteriores si es monitor
    IF @isMonitor = 1
    BEGIN
        UPDATE AgencyUsers 
        SET IsActive = 0,
            UpdatedAt = GETUTCDATE()
        WHERE AgencyId = @agencyId 
        AND IsMonitor = 1 
        AND IsActive = 1;
    END

    -- Si ya existe una asignación activa para este usuario y agencia, actualizarla
    IF EXISTS (
        SELECT 1 
        FROM AgencyUsers 
        WHERE UserId = @userId 
        AND AgencyId = @agencyId 
        AND IsActive = 1
    )
    BEGIN
        UPDATE AgencyUsers
        SET IsOwner = @isOwner,
            IsMonitor = @isMonitor,
            UpdatedAt = GETUTCDATE(),
            AssignedBy = @assignedBy
        WHERE UserId = @userId 
        AND AgencyId = @agencyId 
        AND IsActive = 1;

        SELECT @Id = Id
        FROM AgencyUsers
        WHERE UserId = @userId 
        AND AgencyId = @agencyId 
        AND IsActive = 1;
    END
    ELSE
    BEGIN
        -- Insertar la nueva asignación
        INSERT INTO AgencyUsers (
            UserId, 
            AgencyId, 
            AssignedBy, 
            IsOwner, 
            IsMonitor,
            IsActive,
            CreatedAt
        )
        VALUES (
            @userId, 
            @agencyId, 
            @assignedBy, 
            @isOwner, 
            @isMonitor,
            1,
            GETUTCDATE()
        );
        
        SET @Id = SCOPE_IDENTITY();
    END

    -- Validar que no haya más de un owner activo por agencia
    IF @isOwner = 1
    BEGIN
        IF (
            SELECT COUNT(*)
            FROM AgencyUsers
            WHERE AgencyId = @agencyId
            AND IsOwner = 1
            AND IsActive = 1
        ) > 1
        BEGIN
            RAISERROR ('Ya existe un propietario activo para esta agencia.', 16, 1);
            
            -- Revertir la operación
            IF @Id IS NOT NULL
            BEGIN
                IF EXISTS (SELECT 1 FROM AgencyUsers WHERE Id = @Id)
                    DELETE FROM AgencyUsers WHERE Id = @Id;
                ELSE
                    UPDATE AgencyUsers SET IsActive = 0 WHERE UserId = @userId AND AgencyId = @agencyId;
            END
            
            RETURN -3;
        END
    END

    SELECT @Id AS Id;
END; 