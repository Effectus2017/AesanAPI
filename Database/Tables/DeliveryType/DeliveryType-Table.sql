CREATE TABLE DeliveryType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL DEFAULT '',
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);


-- remover la columna SelectionNotification de la tabla DeliveryType
-- Primero eliminar el constraint por defecto que depende de la columna
IF EXISTS (SELECT *
FROM sys.default_constraints
WHERE name = 'DF__DeliveryT__Selec__3552E9B6')
BEGIN
    ALTER TABLE DeliveryType DROP CONSTRAINT DF__DeliveryT__Selec__3552E9B6;
END

-- Luego eliminar la columna
IF EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID('DeliveryType') AND name = 'SelectionNotification')
BEGIN
    ALTER TABLE DeliveryType DROP COLUMN SelectionNotification;
END