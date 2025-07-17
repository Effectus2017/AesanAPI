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


UPDATE EducationLevel
SET Name = 'Pre-Kinder', NameEN = 'Pre-Kinder'
WHERE Id = 2;

UPDATE EducationLevel
SET Name = 'Primero', NameEN = 'First'
WHERE Id = 3;

UPDATE EducationLevel
SET Name = 'Segundo', NameEN = 'Second'
WHERE Id = 4;


INSERT INTO EducationLevel
    (Name, NameEN, IsActive, CreatedAt)
VALUES
    ('Tercero', 'Third', 1, GETDATE()),
    ('Cuarto', 'Fourth', 1, GETDATE()),
    ('Quinto', 'Fifth', 1, GETDATE()),
    ('Sexto', 'Sixth', 1, GETDATE()),
    ('Séptimo', 'Seventh', 1, GETDATE()),
    ('Octavo', 'Eighth', 1, GETDATE()),
    ('Noveno', 'Ninth', 1, GETDATE()),
    ('Decimo', 'Tenth', 1, GETDATE()),
    ('Undécimo', 'Eleventh', 1, GETDATE()),
    ('Duodécimo', 'Twelfth', 1, GETDATE());



