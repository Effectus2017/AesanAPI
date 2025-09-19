-- Agregar columna LocationTypeId a la tabla School
-- Esta columna tendrá los mismos valores que AreaType (Rural/Urbana)
-- Fecha: 2025-01-15

-- Agregar la nueva columna LocationTypeId
ALTER TABLE School
    ADD LocationTypeId INT NULL;

-- Agregar la relación con la tabla AreaType
ALTER TABLE School
    ADD CONSTRAINT FK_School_LocationType FOREIGN KEY (LocationTypeId) REFERENCES AreaType(Id);

-- Comentario para documentar el propósito de la nueva columna
EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Tipo de localización de la escuela (Rural/Urbana) - Usa los mismos valores que AreaType',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'School',
    @level2type = N'COLUMN', @level2name = N'LocationTypeId';
