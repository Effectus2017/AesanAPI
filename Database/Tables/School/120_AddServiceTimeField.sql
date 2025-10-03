-- =============================================
-- Migration: Add ServiceTime field to School table
-- Description: Add field to track how long the school/site has been providing services
-- Date: 2025-03-14
-- Version: 2.4
-- =============================================

-- Add ServiceTime field to School table
ALTER TABLE School 
ADD ServiceTime DATETIME NULL;

-- Add comment explaining the field purpose
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'¿Desde cuándo su Entidad ofrece servicios con una matrícula establecida? / Since when has your Entity offered services with an established registration?', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'School', 
    @level2type = N'COLUMN', @level2name = N'ServiceTime';

-- Create index for ServiceTime if needed for performance (optional)
-- CREATE INDEX IX_School_ServiceTime ON School(ServiceTime);
