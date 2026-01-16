-- =============================================
-- Trigger: trg_PreventDeleteAgency1
-- Descripción: Previene la eliminación directa de agencias propietarias (isPropietary = 1) desde cualquier script SQL
-- Fecha: 2025-01-XX
-- Versión: 2.0
-- =============================================
-- IMPORTANTE: Este trigger protege las agencias propietarias (isPropietary = 1) de ser eliminadas
-- bajo ninguna circunstancia, incluso si se intenta un DELETE directo en la tabla
-- =============================================

CREATE OR ALTER TRIGGER [dbo].[trg_PreventDeleteAgency1]
ON [dbo].[Agency]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si se intenta eliminar alguna agencia propietaria (isPropietary = 1)
    IF EXISTS (SELECT 1 FROM deleted d INNER JOIN Agency a ON d.Id = a.Id WHERE a.IsPropietary = 1)
    BEGIN
        RAISERROR('No se puede eliminar una agencia propietaria. Esta agencia está protegida y no puede ser eliminada bajo ninguna circunstancia.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Permitir eliminación de otras agencias
    DELETE a
    FROM [dbo].[Agency] a
    INNER JOIN deleted d ON a.Id = d.Id;
END;
GO
