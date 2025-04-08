-- Agregar campo BasicEducationRegistry a la tabla Agency
ALTER TABLE Agency
ADD BasicEducationRegistry INT NULL DEFAULT 0;
GO

-- Actualizar los registros existentes
UPDATE Agency
SET BasicEducationRegistry = 0
WHERE BasicEducationRegistry IS NULL;
GO

-- Agregar comentario para documentar los valores posibles
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Estado del Registro de Educación Básica: 0 = No definido, 1 = Otorgado, 2 = En Proceso, 3 = No',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Agency',
    @level2type = N'COLUMN',
    @level2name = N'BasicEducationRegistry';
GO 