CREATE TABLE [dbo].[GroupType]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [NameEN] NVARCHAR(255) NOT NULL DEFAULT '',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL
);

-- Modificación de la tabla GroupType para agregar la columna NameEN y DisplayOrder si no existen
IF COL_LENGTH('GroupType', 'NameEN') IS NULL
BEGIN
    ALTER TABLE GroupType ADD NameEN NVARCHAR(255) NOT NULL DEFAULT '';
END
IF COL_LENGTH('GroupType', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE GroupType ADD DisplayOrder INT NOT NULL DEFAULT 0;
END 