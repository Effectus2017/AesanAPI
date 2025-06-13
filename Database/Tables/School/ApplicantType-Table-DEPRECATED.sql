-- Tipo de Solicitante
-- Laico (15), Base de fe (16)
-- No se usa, se esta usando tabla OptionSelection para este tipo de datos
CREATE TABLE ApplicantType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    NameEN NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

INSERT INTO ApplicantType
    (Name, NameEN, IsActive, CreatedAt)
VALUES
    ('Laico', 'Laico', 1, GETDATE()),
    ('Base de fe', 'Base de fe', 1, GETDATE());