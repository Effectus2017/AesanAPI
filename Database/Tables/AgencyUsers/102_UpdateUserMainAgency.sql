-- =============================================
-- Stored Procedure: 102_UpdateUserMainAgency
-- Fecha: 2025-01-XX
-- Descripción: Actualiza la agencia principal a la que pertenece un usuario.
--              Reemplaza 101_UpdateUserMainAgency con nueva lógica.
--              Usa AgencyAssignmentType = 'AGENCY_OWNER'.
-- =============================================

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE OR ALTER PROCEDURE [dbo].[102_UpdateUserMainAgency]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @oldAgencyId INT;
    DECLARE @Id INT;
    
    -- Obtener la agencia principal actual (donde es AGENCY_OWNER)
    SELECT @oldAgencyId = AgencyId
    FROM AgencyUsers
    WHERE UserId = @userId 
        AND AgencyAssignmentType = 'AGENCY_OWNER'
        AND IsActive = 1;
    
    -- Si la agencia es diferente, actualizar
    IF @oldAgencyId IS NULL OR @oldAgencyId != @agencyId
    BEGIN
        -- Desactivar la asignación anterior si existe
        IF @oldAgencyId IS NOT NULL
        BEGIN
            UPDATE AgencyUsers
            SET IsActive = 0,
                UpdatedAt = GETUTCDATE(),
                AssignedBy = @assignedBy
            WHERE UserId = @userId 
                AND AgencyId = @oldAgencyId 
                AND AgencyAssignmentType = 'AGENCY_OWNER'
                AND IsActive = 1;
        END
        
        -- Verificar si ya existe una asignación activa para este usuario y agencia
        IF EXISTS (
            SELECT 1
            FROM AgencyUsers
            WHERE UserId = @userId
                AND AgencyId = @agencyId
                AND IsActive = 1
        )
        BEGIN
            -- Actualizar asignación existente
            UPDATE AgencyUsers
            SET AgencyAssignmentType = 'AGENCY_OWNER',
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
            -- Crear nueva asignación
            INSERT INTO AgencyUsers (
                UserId,
                AgencyId,
                AgencyAssignmentType,
                IsActive,
                CreatedAt,
                AssignedDate,
                AssignedBy
            )
            VALUES (
                @userId,
                @agencyId,
                'AGENCY_OWNER',
                1,
                GETUTCDATE(),
                GETUTCDATE(),
                @assignedBy
            );
            
            SET @Id = SCOPE_IDENTITY();
        END
        
        SELECT @Id AS Id;
    END
    ELSE
    BEGIN
        -- La agencia es la misma, retornar el ID existente
        SELECT @Id = Id
        FROM AgencyUsers
        WHERE UserId = @userId
            AND AgencyId = @agencyId
            AND AgencyAssignmentType = 'AGENCY_OWNER'
            AND IsActive = 1;
        
        SELECT @Id AS Id;
    END
END
GO
