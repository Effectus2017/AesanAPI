CREATE TABLE OrganizationType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL DEFAULT '',
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Modificación de la tabla OrganizationType para agregar la columna NameEN y DisplayOrder si no existen

IF COL_LENGTH('OrganizationType', 'NameEN') IS NULL
BEGIN
    ALTER TABLE OrganizationType ADD NameEN NVARCHAR(255) NOT NULL DEFAULT '';
END
IF COL_LENGTH('OrganizationType', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE OrganizationType ADD DisplayOrder INT NOT NULL DEFAULT 0;
END

-- Escuela
-- Satélite
-- Institución Residencial
-- Otros

INSERT INTO OrganizationType
    (Name, NameEN, DisplayOrder, IsActive)
VALUES
    ('Escuela', 'School', 10, 1);
INSERT INTO OrganizationType
    (Name, NameEN, DisplayOrder, IsActive)
VALUES
    ('Satélite', 'Satellite', 20, 1);
INSERT INTO OrganizationType
    (Name, NameEN, DisplayOrder, IsActive)
VALUES
    ('Institución Residencial', 'Residential Institution', 30, 1);
INSERT INTO OrganizationType
    (Name, NameEN, DisplayOrder, IsActive)
VALUES
    ('Otros', 'Others', 40, 1);