-- =============================================
-- Stored Procedure: 102_AssignAgencyToUser
-- Fecha: 2025-01-XX
-- Descripción: Asigna una agencia a un usuario usando AgencyAssignmentType.
--              Reemplaza 101_AssignAgencyToUser con nueva lógica.
--              El rol se obtiene mediante JOIN con AspNetUserRoles.
--              Usa AgencyAssignmentType en lugar de IsOwner/IsMonitor.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_AssignAgencyToUser]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450),
    @agencyAssignmentType VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;
    DECLARE @userRoleId NVARCHAR(450);
    DECLARE @userRoleName NVARCHAR(256);
    DECLARE @assignmentCategory VARCHAR(50);
    DECLARE @canBeOwner BIT = 0;

    -- Obtener el rol del usuario (solo tiene uno)
    SELECT TOP 1 
        @userRoleId = ur.RoleId,
        @userRoleName = r.Name
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId;
    
    IF @userRoleId IS NULL
    BEGIN
        RAISERROR('El usuario no tiene un rol asignado.', 16, 1);
        RETURN -1;
    END
    
    -- Obtener AssignmentCategory del rol desde RoleAssignmentCategory
    SELECT 
        @assignmentCategory = rac.AssignmentCategory,
        @canBeOwner = rac.CanBeOwner
    FROM RoleAssignmentCategory rac
    WHERE rac.RoleId = @userRoleId;
    
    -- Si el rol no está en RoleAssignmentCategory, es SuperAdministrator o Administrator
    -- Estos roles no necesitan validación porque ven todas las agencias
    IF @assignmentCategory IS NULL
    BEGIN
        -- SuperAdministrator y Administrator no se asignan a agencias específicas
        -- Pero permitimos la asignación si se requiere
        IF @userRoleName NOT IN ('SuperAdministrator', 'Super-Administrador', 'Administrator', 'Administrador')
        BEGIN
            RAISERROR('El rol del usuario no está configurado en RoleAssignmentCategory.', 16, 1);
            RETURN -2;
        END
    END
    
    -- Validar que AgencyAssignmentType coincide con AssignmentCategory
    IF @assignmentCategory = 'AGENCY'
    BEGIN
        IF @agencyAssignmentType NOT LIKE 'AGENCY_%'
        BEGIN
            RAISERROR('AgencyAssignmentType debe comenzar con AGENCY_ para roles de agencia.', 16, 1);
            RETURN -3;
        END
        
        -- Validar que el rol puede ser owner
        IF @agencyAssignmentType = 'AGENCY_OWNER'
        BEGIN
            IF NOT @canBeOwner = 1
            BEGIN
                RAISERROR('Este rol no puede ser propietario de una agencia.', 16, 1);
                RETURN -4;
            END
        END
    END
    ELSE IF @assignmentCategory = 'NUTRE'
    BEGIN
        IF @agencyAssignmentType NOT LIKE 'NUTRE_%'
        BEGIN
            RAISERROR('AgencyAssignmentType debe comenzar con NUTRE_ para roles NUTRE.', 16, 1);
            RETURN -6;
        END
        
        -- Desactivar asignaciones anteriores si es monitor/coordinador/evaluador
        -- (solo un monitor activo por agencia para roles NUTRE)
        IF @agencyAssignmentType IN ('NUTRE_EVALUATOR', 'NUTRE_COORDINATOR')
        BEGIN
            UPDATE AgencyUsers 
            SET IsActive = 0,
                UpdatedAt = GETUTCDATE()
            WHERE AgencyId = @agencyId
                AND AgencyAssignmentType IN ('NUTRE_EVALUATOR', 'NUTRE_COORDINATOR')
                AND IsActive = 1
                AND UserId != @userId;
        END
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
        SET AgencyAssignmentType = @agencyAssignmentType,
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
        INSERT INTO AgencyUsers
            (
            UserId,
            AgencyId,
            AssignedBy,
            AgencyAssignmentType,
            IsActive,
            CreatedAt,
            AssignedDate
            )
        VALUES
            (
                @userId,
                @agencyId,
                @assignedBy,
                @agencyAssignmentType,
                1,
                GETUTCDATE(),
                GETUTCDATE()
            );

        SET @Id = SCOPE_IDENTITY();
    END

    SELECT @Id AS Id;
END
GO
