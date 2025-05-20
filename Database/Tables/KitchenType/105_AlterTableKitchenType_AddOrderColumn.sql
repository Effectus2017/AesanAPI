-- =============================================
-- Author:      Development Team
-- Create date: 2024-06-10
-- Description: Script para añadir columnas DisplayOrder y NameEN a la tabla KitchenType
-- =============================================

-- Verificamos si la tabla KitchenType existe
IF EXISTS (SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'KitchenType')
BEGIN
    -- Verificamos si la columna DisplayOrder ya existe
    IF NOT EXISTS (SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'KitchenType' AND COLUMN_NAME = 'DisplayOrder')
    BEGIN
        ALTER TABLE KitchenType ADD DisplayOrder INT NOT NULL DEFAULT 0;
        -- Actualizamos los valores de DisplayOrder según el orden deseado
        UPDATE KitchenType SET DisplayOrder = 10 WHERE Id = 1;
        UPDATE KitchenType SET DisplayOrder = 20 WHERE Id = 2;
        UPDATE KitchenType SET DisplayOrder = 30 WHERE Id = 3;
        UPDATE KitchenType SET DisplayOrder = 40 WHERE Id = 4;
        UPDATE KitchenType SET DisplayOrder = 50 WHERE Id = 5;
        UPDATE KitchenType SET DisplayOrder = 60 WHERE Id = 6;
        PRINT 'La columna DisplayOrder ha sido añadida a la tabla KitchenType y los valores han sido actualizados.';
    END
    ELSE
    BEGIN
        PRINT 'La columna DisplayOrder ya existe en la tabla KitchenType.';
    END
    -- Verificamos si la columna NameEN ya existe
    IF NOT EXISTS (SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'KitchenType' AND COLUMN_NAME = 'NameEN')
    BEGIN
        ALTER TABLE KitchenType ADD NameEN NVARCHAR(255) NOT NULL DEFAULT '';
        PRINT 'La columna NameEN ha sido añadida a la tabla KitchenType.';
    END
    ELSE
    BEGIN
        PRINT 'La columna NameEN ya existe en la tabla KitchenType.';
    END
END
ELSE
BEGIN
    PRINT 'La tabla KitchenType no existe.';
END 