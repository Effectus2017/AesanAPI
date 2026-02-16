-- =============================================
-- Migration Script: Rename AreaCode to ZipCode in Staff table
-- =============================================
-- This script renames the AreaCode column to ZipCode in the Staff table
-- and updates all related stored procedures and views

-- Step 1: Rename the column in the Staff table
IF EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND name = 'AreaCode')
BEGIN
    EXEC sp_rename '[dbo].[Staff].[AreaCode]', 'ZipCode', 'COLUMN';
    PRINT 'Column AreaCode renamed to ZipCode in Staff table';
END
ELSE
BEGIN
    PRINT 'Column AreaCode does not exist in Staff table';
END

-- Step 2: Update the default constraint if it exists
IF EXISTS (SELECT *
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID(N'[dbo].[Staff]') AND name LIKE '%AreaCode%')
BEGIN
    DECLARE @constraintName NVARCHAR(256);
    SELECT @constraintName = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[Staff]') AND name LIKE '%AreaCode%';
    EXEC('ALTER TABLE [dbo].[Staff] DROP CONSTRAINT ' + @constraintName);
    PRINT 'Default constraint for AreaCode dropped';
END

-- Note: The stored procedures have already been updated to use @zipCode parameter
-- and ZipCode column name. This migration script only handles the table column rename.

PRINT 'Migration completed: AreaCode renamed to ZipCode in Staff table';

