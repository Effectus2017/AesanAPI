-- =============================================
-- Trigger: trg_PreventDeleteAgency1
-- Descripción: Previene la eliminación directa de la agencia 1 (NUTRE) desde cualquier script SQL
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================
-- IMPORTANTE: Este trigger protege la agencia 1 (NUTRE) de ser eliminada
-- bajo ninguna circunstancia, incluso si se intenta un DELETE directo en la tabla
-- =============================================

CREATE OR ALTER TRIGGER [dbo].[trg_PreventDeleteAgency1]
ON [dbo].[Agency]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si se intenta eliminar la agencia 1 (NUTRE)
    IF EXISTS (SELECT 1 FROM deleted WHERE Id = 1)
    BEGIN
        RAISERROR('No se puede eliminar la agencia 1 (NUTRE). Esta agencia está protegida y no puede ser eliminada bajo ninguna circunstancia.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Permitir eliminación de otras agencias
    DELETE a
    FROM [dbo].[Agency] a
    INNER JOIN deleted d ON a.Id = d.Id;
END;
GO
