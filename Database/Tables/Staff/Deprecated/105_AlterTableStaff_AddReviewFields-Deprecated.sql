-- =============================================
-- Script de migración: Agregar campos de revisión a Staff
-- =============================================
-- Agrega los campos de revisión a la tabla Staff para empleados

-- Agregar campos de revisión
ALTER TABLE Staff ADD ReviewResultId INT NULL;
ALTER TABLE Staff ADD ReviewDate DATETIME NULL;
ALTER TABLE Staff ADD ReviewJustification NVARCHAR(500) NULL;

-- Agregar foreign key para ReviewResultId
ALTER TABLE Staff ADD CONSTRAINT FK_Staff_ReviewResultId 
FOREIGN KEY (ReviewResultId) REFERENCES OptionSelection(Id);