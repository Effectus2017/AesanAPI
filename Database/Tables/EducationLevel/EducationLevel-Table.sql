-- Niveles de Educación
CREATE TABLE EducationLevel
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

INSERT INTO EducationLevel
    (Name, NameEN, IsActive, CreatedAt)
VALUES
    ('Kinder', 'Kinder', 1, GETDATE()),
    ('Elemental', 'Elemental', 1, GETDATE()),
    ('Intermedio', 'Intermedio', 1, GETDATE()),
    ('Superior', 'Superior', 1, GETDATE());