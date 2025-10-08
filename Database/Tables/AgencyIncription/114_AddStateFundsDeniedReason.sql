-- Agregar campo para razón de descalificación de fondos estatales
-- 1.1.5 - Nuevo campo para capturar la razón cuando StateFundsDenied = true
-- Add field for state funds denied reason
-- 1.1.5 - New field to capture the reason when StateFundsDenied = true

ALTER TABLE AgencyInscription
ADD StateFundsDeniedReason NVARCHAR(MAX) NULL;

-- Comentario del campo
-- Comment for the field
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Razón por la cuál fue descalificado o denegado de fondos estatales. Se activa cuando StateFundsDenied = true. Reason why the sponsor was disqualified or denied state funds. Activated when StateFundsDenied = true.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'AgencyInscription', 
    @level2type = N'COLUMN', @level2name = N'StateFundsDeniedReason';

GO
