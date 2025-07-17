-- Tabla de tipo de área
CREATE TABLE AreaType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    DisplayOrder INT NOT NULL DEFAULT 0
);

INSERT INTO AreaType
    (Name, NameEN, IsActive, CreatedAt, DisplayOrder)
VALUES
    ('Rural', 'Rural', 1, GETDATE(), 10),
    ('Urbana', 'Urban', 1, GETDATE(), 20);