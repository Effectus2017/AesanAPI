-- =============================================
-- Migration Script: Add Contract Dates to Staff Table
-- =============================================
-- This script adds the ContractStartDate and ContractEndDate columns to the Staff table
-- for existing databases that don't have these columns yet.

-- Check if ContractStartDate column exists, if not add it
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Staff' AND COLUMN_NAME = 'ContractStartDate')
BEGIN
    ALTER TABLE Staff ADD ContractStartDate DATETIME NULL;
    PRINT 'Added ContractStartDate column to Staff table';
END
ELSE
BEGIN
    PRINT 'ContractStartDate column already exists in Staff table';
END

-- Check if ContractEndDate column exists, if not add it
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Staff' AND COLUMN_NAME = 'ContractEndDate')
BEGIN
    ALTER TABLE Staff ADD ContractEndDate DATETIME NULL;
    PRINT 'Added ContractEndDate column to Staff table';
END
ELSE
BEGIN
    PRINT 'ContractEndDate column already exists in Staff table';
END

-- Add indexes if they don't exist
IF NOT EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_Staff_ContractStartDate' AND object_id = OBJECT_ID('Staff'))
BEGIN
    CREATE INDEX IX_Staff_ContractStartDate ON Staff(ContractStartDate);
    PRINT 'Added IX_Staff_ContractStartDate index';
END
ELSE
BEGIN
    PRINT 'IX_Staff_ContractStartDate index already exists';
END

IF NOT EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_Staff_ContractEndDate' AND object_id = OBJECT_ID('Staff'))
BEGIN
    CREATE INDEX IX_Staff_ContractEndDate ON Staff(ContractEndDate);
    PRINT 'Added IX_Staff_ContractEndDate index';
END
ELSE
BEGIN
    PRINT 'IX_Staff_ContractEndDate index already exists';
END

PRINT 'Migration completed successfully';
