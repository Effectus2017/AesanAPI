-- =============================================
-- Script: 117_InsertSiteTypeNAOption
-- Fecha: 2026-02-12
-- Descripción: Inserta opción N/A para Tipo de Sitio (siteType) en PSAV
-- =============================================

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'siteType' AND Name = 'N/A')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES
        ('N/A', 'N/A', 'siteType', 0, 1, 665);
END
GO
