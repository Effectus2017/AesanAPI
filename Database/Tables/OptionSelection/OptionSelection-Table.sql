CREATE TABLE OptionSelection
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    -- nombre de la opción
    NameEN NVARCHAR(255) NOT NULL,
    -- nombre de la opción en inglés
    OptionKey NVARCHAR(255) NOT NULL,
    -- clave de la opción para identificarla
    IsActive BIT NOT NULL DEFAULT 1,
    -- si el estado sigue activo
    DisplayOrder INT NOT NULL DEFAULT 0,
    -- orden de visualización
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- fecha de creación
    UpdatedAt DATETIME NULL
    -- fecha de modificación
);

ALTER TABLE OptionSelection 
ADD NameEN NVARCHAR(255) NOT NULL, 
DisplayOrder INT NOT NULL DEFAULT 0,
OptionKey NVARCHAR(255) NOT NULL;

INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('Si', 'Yes', 'yesNo', 1, 10);
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('No', 'No', 'yesNo', 1, 20);
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('N/A', 'N/A', 'yesNo', 1, 30);


INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('En Proceso', 'In Progress', 'status', 1, 40);
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('Otorgado', 'Granted', 'status', 1, 50);
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('Denegada', 'Denied', 'status', 1, 60);





