-- =============================================
-- Stored Procedure: 100_InsertSiteParticipants
-- Descripción: Inserta múltiples tipos de participantes para un sitio
-- Reemplaza: 100_InsertSchoolParticipants
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteParticipants]
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

    -- Insertar tipos de participantes
    INSERT INTO SiteParticipant
        (SiteId, ParticipantTypeId, IsActive, CreatedAt)
    SELECT @siteId, ParticipantTypeId, 1, GETDATE()
    FROM @ParticipantTypeTable;
END;
