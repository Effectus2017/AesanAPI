-- =============================================
-- Stored Procedure: 100_UpdateSiteParticipants
-- Descripción: Actualiza múltiples tipos de participantes para un sitio
-- Reemplaza: 100_UpdateSchoolParticipants
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteParticipants]
    @siteId INT,
    @participantTypeIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @ParticipantTypeTable TABLE (ParticipantTypeId INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @ParticipantTypeTable
        (ParticipantTypeId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@participantTypeIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Eliminar tipos de participantes existentes
    DELETE FROM SiteParticipant WHERE SiteId = @siteId;

    -- Insertar nuevos tipos de participantes
    INSERT INTO SiteParticipant
        (SiteId, ParticipantTypeId, IsActive, CreatedAt)
    SELECT @siteId, ParticipantTypeId, 1, GETDATE()
    FROM @ParticipantTypeTable;
END;
