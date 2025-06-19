-- Agregar campos para manejo de inactivación de sitios
-- Versión: 1.0.3 - Agregar campos InactiveJustification e InactiveDate

-- Agregar campo para justificación de inactivación
IF NOT EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID(N'School') AND name = 'InactiveJustification')
BEGIN
    ALTER TABLE School
    ADD InactiveJustification NVARCHAR(500) NULL;
END

-- Agregar campo para fecha de inactivación
IF NOT EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID(N'School') AND name = 'InactiveDate')
BEGIN
    ALTER TABLE School
    ADD InactiveDate DATETIME NULL;
END

-- Comentarios de los campos agregados:
-- InactiveJustification: Campo de texto para almacenar la justificación cuando se inactiva un sitio
-- InactiveDate: Campo de fecha/hora para almacenar cuándo se inactivó el sitio (se pone en NULL cuando se reactiva) 