-- Facilidades
-- VER SI ESTA DEPRECADA ESTA TABLA
CREATE TABLE Facility
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);


INSERT INTO Facility
    (Name)
VALUES
    ('Almacén');
INSERT INTO Facility
    (Name)
VALUES
    ('Cocina');
INSERT INTO Facility
    (Name)
VALUES
    ('Salón Comedor');
