-- Agregar la columna FederalFundsDeniedReason a la tabla AgencyInscription
-- 1.1.6
ALTER TABLE AgencyInscription
ADD FederalFundsDeniedReason NVARCHAR(MAX) NULL;

-- Agregar comentario para documentar el campo
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Razón por la cuál fue descalificado o denegado de fondos federales. Se activa cuando FederalFundsDenied = true',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AgencyInscription',
    @level2type = N'COLUMN',
    @level2name = N'FederalFundsDeniedReason';
GO
