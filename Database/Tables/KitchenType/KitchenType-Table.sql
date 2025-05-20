CREATE TABLE [dbo].[KitchenType]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [NameEN] NVARCHAR(255) NOT NULL DEFAULT '',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL
);

-- Modificación de la tabla KitchenType para agregar la columna NameEN y DisplayOrder si no existen
IF COL_LENGTH('KitchenType', 'NameEN') IS NULL
BEGIN
    ALTER TABLE KitchenType ADD NameEN NVARCHAR(255) NOT NULL DEFAULT '';
END
IF COL_LENGTH('KitchenType', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE KitchenType ADD DisplayOrder INT NOT NULL DEFAULT 0;
END 