-- Agregar campo SiteLocationId a la tabla School
-- Site Location ID - FK to OptionSelection where OptionKey = siteLocation
-- Versión: 1.0
-- Fecha: 2025-01-15

-- Agregar campo SiteLocationId a la tabla School
ALTER TABLE School
ADD SiteLocationId INT NULL;

-- Agregar foreign key constraint
ALTER TABLE School
ADD CONSTRAINT FK_School_SiteLocation FOREIGN KEY (SiteLocationId) REFERENCES OptionSelection(Id);

-- Agregar comentario
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Site Location ID - FK to OptionSelection where OptionKey = siteLocation', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'School', 
    @level2type = N'COLUMN', @level2name = N'SiteLocationId';
