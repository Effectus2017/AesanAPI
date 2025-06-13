CREATE TABLE OperatingPolicy
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    PRIMARY KEY CLUSTERED ([Id])
);

-- Política de Funcionamiento 1 Gratis
-- Política de Funcionamiento 2 Pagando
-- Política de Funcionamiento 3 Provisión I
-- Política de Funcionamiento 4 Provisión 2
-- Política de Funcionamiento 5 Provisión 3

INSERT INTO OperatingPolicy
    (Name, NameEN, IsActive, DisplayOrder, CreatedAt, UpdatedAt)
VALUES
    ('Gratis', 'Free', 1, 10, GETDATE(), GETDATE()),
    ('Pagando', 'Paying', 1, 20, GETDATE(), GETDATE()),
    ('Provisión I', 'Provision I', 1, 30, GETDATE(), GETDATE()),
    ('Provisión 2', 'Provision 2', 1, 40, GETDATE(), GETDATE()),
    ('Provisión 3', 'Provision 3', 1, 50, GETDATE(), GETDATE());