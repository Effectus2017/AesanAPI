-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para actualizar la tabla AgencyStatus con el orden especificado
-- =============================================

-- Primero, eliminamos los registros existentes en AgencyStatus
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AgencyStatus')
BEGIN
    DELETE FROM AgencyStatus;
    
    -- Reiniciamos el contador de identidad
    DBCC CHECKIDENT ('AgencyStatus', RESEED, 0);
    
    -- Insertamos los nuevos registros con el orden especificado
    INSERT INTO AgencyStatus (Name, IsActive, CreatedAt)
    VALUES 
        ('Pendiente a validar', 1, GETDATE()),           -- 1 (default cuando cae en el listado)
        ('Coordinar Visita Pre-Operacional', 1, GETDATE()), -- 2
        ('Orientación de Programa', 1, GETDATE()),       -- 3
        ('Orientación de Contabilidad', 1, GETDATE()),   -- 4
        ('Cumple con los requisitos', 1, GETDATE()),     -- 5
        ('No cumple con los requisitos', 1, GETDATE());  -- 6
    
    PRINT 'La tabla AgencyStatus ha sido actualizada con el nuevo orden.';
END
ELSE
BEGIN
    PRINT 'La tabla AgencyStatus no existe.';
END 