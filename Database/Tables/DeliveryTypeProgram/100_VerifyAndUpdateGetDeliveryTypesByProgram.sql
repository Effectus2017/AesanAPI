-- Script de verificación y actualización del stored procedure 100_GetDeliveryTypesByProgram
-- Este script verifica si el stored procedure existe y si contiene referencias a SelectionNotification
-- y lo actualiza si es necesario

-- Paso 1: Verificar si el stored procedure existe
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypesByProgram]') AND type in (N'P', N'PC'))
BEGIN
    PRINT 'Stored procedure 100_GetDeliveryTypesByProgram existe.';

    -- Verificar si contiene referencias a SelectionNotification
    DECLARE @ProcedureDefinition NVARCHAR(MAX);
    SET @ProcedureDefinition = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypesByProgram'));

    IF @ProcedureDefinition LIKE '%SelectionNotification%'
    BEGIN
        PRINT 'ADVERTENCIA: El stored procedure contiene referencias a SelectionNotification.';
        PRINT 'Actualizando el stored procedure...';

        -- Eliminar el stored procedure antiguo
        DROP PROCEDURE [100_GetDeliveryTypesByProgram];
        PRINT 'Stored procedure antiguo eliminado.';
    END
    ELSE
    BEGIN
        PRINT 'El stored procedure no contiene referencias a SelectionNotification.';
        PRINT 'Recreando el stored procedure para asegurar que esté actualizado...';

        -- Eliminar el stored procedure para recrearlo
        DROP PROCEDURE [100_GetDeliveryTypesByProgram];
    END
END
ELSE
BEGIN
    PRINT 'Stored procedure 100_GetDeliveryTypesByProgram no existe. Creándolo...';
END
GO

-- Paso 2: Crear/Actualizar el stored procedure con la versión correcta
CREATE OR ALTER PROCEDURE [100_GetDeliveryTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dt.Id,
        dt.Name,
        dt.NameEN,
        dt.IsActive,
        dt.DisplayOrder,
        dt.CreatedAt,
        dt.UpdatedAt
    FROM DeliveryType dt
        INNER JOIN DeliveryTypeProgram dtp ON dt.Id = dtp.DeliveryTypeId
    WHERE dtp.ProgramId = @programId
        AND dt.IsActive = 1
        AND dtp.IsActive = 1
    ORDER BY dt.DisplayOrder, dt.Name;
END;
GO

PRINT 'Stored procedure 100_GetDeliveryTypesByProgram creado/actualizado correctamente.';
GO

