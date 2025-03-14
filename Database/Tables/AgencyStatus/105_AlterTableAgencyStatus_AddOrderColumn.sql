-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para añadir una columna de orden a la tabla AgencyStatus
-- =============================================

-- Verificamos si la tabla AgencyStatus existe
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AgencyStatus')
BEGIN
    -- Verificamos si la columna DisplayOrder ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AgencyStatus' AND COLUMN_NAME = 'DisplayOrder')
    BEGIN
        -- Añadimos la columna DisplayOrder
        ALTER TABLE AgencyStatus
        ADD DisplayOrder INT NOT NULL DEFAULT 0;
        
        -- Actualizamos los valores de DisplayOrder según el orden deseado
        UPDATE AgencyStatus SET DisplayOrder = 10 WHERE Id = 1; -- Pendiente a validar
        UPDATE AgencyStatus SET DisplayOrder = 20 WHERE Id = 2; -- Coordinar Visita Pre-Operacional
        UPDATE AgencyStatus SET DisplayOrder = 30 WHERE Id = 3; -- Orientación de Programa
        UPDATE AgencyStatus SET DisplayOrder = 40 WHERE Id = 4; -- Orientación de Contabilidad
        UPDATE AgencyStatus SET DisplayOrder = 50 WHERE Id = 5; -- Cumple con los requisitos
        UPDATE AgencyStatus SET DisplayOrder = 60 WHERE Id = 6; -- No cumple con los requisitos
        
        PRINT 'La columna DisplayOrder ha sido añadida a la tabla AgencyStatus y los valores han sido actualizados.';
    END
    ELSE
    BEGIN
        PRINT 'La columna DisplayOrder ya existe en la tabla AgencyStatus.';
    END
END
ELSE
BEGIN
    PRINT 'La tabla AgencyStatus no existe.';
END 