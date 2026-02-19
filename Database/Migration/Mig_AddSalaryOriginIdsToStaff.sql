-- =============================================
-- Migration: Add SalaryOriginIds column to Staff
-- =============================================
-- Columna CapitalCase; valores separados por coma (ej. "1,2,3")

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND name = 'SalaryOriginIds')
BEGIN
    ALTER TABLE Staff ADD SalaryOriginIds NVARCHAR(50) NULL;
    PRINT 'Added column SalaryOriginIds to Staff table';
END
ELSE
BEGIN
    PRINT 'Column SalaryOriginIds already exists on Staff table';
END
