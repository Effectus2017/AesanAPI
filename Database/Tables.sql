-- Ciudades
CREATE TABLE City (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL
);

-- Regiones
CREATE TABLE Region (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CityId INT,
    Name VARCHAR(50) NOT NULL,
    FOREIGN KEY (CityId) REFERENCES City(Id)
);

-- Estados de la agencia
CREATE TABLE AgencyStatus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
);

-- Insertar estados de la agencia
INSERT INTO AgencyStatus (Name) VALUES ('Pendiente de validar');
INSERT INTO AgencyStatus (Name) VALUES ('Orientación');
INSERT INTO AgencyStatus (Name) VALUES ('Visita Pre-operacional');
INSERT INTO AgencyStatus (Name) VALUES ('No cumple con los requisitos');
INSERT INTO AgencyStatus (Name) VALUES ('Cumple con los requisitos');
INSERT INTO AgencyStatus (Name) VALUES ('Rechazado');
INSERT INTO AgencyStatus (Name) VALUES ('Aprobado');


-- Agencias Auspiciadoras (Sponsoring Agencies)
CREATE TABLE Agency (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusId INT NOT NULL,
    CityId INT NOT NULL,
    RegionId INT NOT NULL,
    PostalCityId INT NOT NULL,
    PostalRegionId INT NOT NULL,
    ProgramId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    SdrNumber INT NOT NULL,
    UieNumber INT NOT NULL,
    EinNumber INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    ZipCode INT NOT NULL,
    PostalAddress NVARCHAR(255) NOT NULL,
    PostalZipCode INT NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    ImageURL NVARCHAR(MAX) NULL,
    RejectionJustification NVARCHAR(MAX) NULL,
    IsActive BIT NULL DEFAULT 1,
    IsListable BIT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (StatusId) REFERENCES AgencyStatus(Id)
);
GO

-- Agregar la columna IsActive a la tabla Agency
--ALTER TABLE Agency ADD IsActive BIT NULL DEFAULT 1;
-- Agregar la columna IsListable a la tabla Agency
--ALTER TABLE Agency ADD IsListable BIT NULL DEFAULT 1;

-- Insertar la agencia AESAN por defecto
INSERT INTO Agency (Name, StatusId, CityId, RegionId, ProgramId, SdrNumber, UieNumber, EinNumber, Address, ZipCode, PostalAddress, Latitude, Longitude, Phone, Email, ImageURL, IsActive, IsListable) VALUES (
    'AESAN',
    5, -- Status "Cumple con los requisitos"
    1, -- Ciudad por defecto
    1, -- Región por defecto  
    7, -- Programa AESAN
    123456,
    123456,
    123456,
    'Calle Principal #123',
    00123, 
    'Calle Principal #123',
    18.220833, 
    -66.590149, 
    '787-123-4567', 
    'admin@aesan.pr.gov',
    NULL,
    1,
    1
);

-- Programas de AESAN, agregar un id de registro unico entre programa y agencia
CREATE TABLE Program (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

INSERT INTO Program (Name, Description) VALUES ('PDAM', 'Programa de Desayuno Escolar');
INSERT INTO Program (Name, Description) VALUES ('PSAV', 'Programa de Servicios de Alimentos de Verano');
INSERT INTO Program (Name, Description) VALUES ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto');
INSERT INTO Program (Name, Description) VALUES ('PFHF', 'Programa de Frutas y Hortalizas Frescas');
INSERT INTO Program (Name, Description) VALUES ('PAF', 'Programa de Distribución de Alimentos Federales');
INSERT INTO Program (Name, Description) VALUES ('PDFE', 'Programa de la Finca a la Escuela');
INSERT INTO Program (Name, Description) VALUES ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos');


-- Asignación de programas a agencias
CREATE TABLE AgencyProgram (
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

--INSERT INTO AgencyProgram (AgencyId, ProgramId) VALUES (1, 1);
--INSERT INTO AgencyProgram (AgencyId, ProgramId) VALUES (1, 2);

-- Crear tabla AspNetUsers
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    AgencyId INT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    -- Datos del usuario
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    FatherLastName NVARCHAR(100) NOT NULL,
    MotherLastName NVARCHAR(100) NOT NULL,
    AdministrationTitle NVARCHAR(100) NOT NULL,
    ImageURL NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);
GO
-- Crear tabla AspNetRoles
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);

-- Crear tabla AspNetUserRoles
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

-- Crear tabla AspNetUserClaims
CREATE TABLE AspNetUserClaims (
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Crear tabla AspNetRoleClaims
CREATE TABLE AspNetRoleClaims (
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

-- Crear tabla AspNetUserLogins
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Crear tabla para almacenar contraseñas temporales sin restricciones de clave foránea
CREATE TABLE TemporaryPasswords (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    TemporaryPassword NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
);


INSERT INTO City (name) VALUES
('Arecibo'),
('Bayamón'),
('Mayagüez'),
('Ponce'),
('Caguas'),
('San Juan'),
('Humacao');

INSERT INTO Region (name, CityId) VALUES
('Aguada', 1),
('Barceloneta', 1),
('Camuy', 1),
('Ciales', 1),
('Dorado', 1),
('Florida', 1),
('Hatillo', 1),
('Lares', 1),
('Manatí', 1),
('Quebradillas', 1),
('Vega Alta', 1),
('Vega Baja', 1),
('Bayamón', 2),
('Cataño', 2),
('Corozal', 2),
('Cabo Rojo', 2),
('Aguadilla', 3),
('Isabela', 3),
('Lajas', 3),
('Las Marías', 3),
('Maricao', 3),
('Mayagüez', 3),
('Adjuntas', 4),
('Coamo', 4),
('Guayanilla', 4),
('Guánica', 4),
('Jayuya', 4),
('Ponce', 4),
('Aguas Buenas', 5),
('Aibonito', 5),
('Arroyo', 5),
('Barranquitas', 5),
('Caguas', 5),
('Cayey', 5),
('Cidra', 5),
('Guayama', 5),
('Carolina', 6),
('Guaynabo', 6),
('San Juan', 6),
('Trujillo Alto', 6),
('Ceiba', 7),
('Fajardo', 7),
('Juncos', 7),
('Las Piedras', 7),
('Rio Grande', 7),
('San Lorenzo', 7),
('Vieques', 7),
('Yabucoa', 7);

-- Insertar roles
INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES
('8aeab3f3-540c-4997-8452-b151d5a40391', '72401a60-d83b-4687-9827-d4d610d1a3e4', 'Administrator', 'ADMINISTRATOR'),
('9f86d081-84f7-367d-96e7-48b5e3d8a2f4', 'e2fc714f-5d5b-42a4-93b2-0e9c05e6a6b5', 'User', 'USER'),
('4e3a3f3e-540c-4997-8452-b151d5a40392', 'a2b3c4d5-e6f7-g8h9-i1j2-k3l4m5n6o7p8', 'SuperAdministrator', 'SUPERADMINISTRATOR');

-- Admin: @dmin5812931!
-- SuperAdministrator: SuperAdmin123!
-- User: UserPassword456!
-- Inserting 28 rows into dbo.AspNetUsers
-- Insert batch #1
INSERT INTO AspNetUsers (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled, LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, FirstName, MiddleName, FatherLastName, MotherLastName, AdministrationTitle) VALUES
('68828e21-4ac8-43ff-b717-160555d199e9', 0, '6f4762e6-95af-41af-a403-9ef0d426e95d', 'admin@admin.com', 0, 1, NULL, 'ADMIN@ADMIN.COM', 'ADMIN@ADMIN.COM', 'AE8yhHOU45pC7AX/ZZnN0vCLGwzf/JgYQAwAzjhG+9BkQfSiolhKkLL9yIto6EUz5g==', NULL, 0, 'I33YA5SKNJS5UAFSULWBKTUMII4RZI6G', 0, 'admin@admin.com', 'Admin', NULL, 'User', 'Admin', 'Administrator'),
('12345678-1234-1234-1234-123456789012', 0, 'f8b0f8d1-9c7e-4b5a-8a1d-26c8f6ae6b6f', 'superadmin@example.com', 1, 1, NULL, 'SUPERADMIN@EXAMPLE.COM', 'SUPERADMIN@EXAMPLE.COM', 'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==', NULL, 0, 'RMZCOGPLZ4DAPL6VXZWFMFNX44MGGBKJ', 0, 'superadmin@example.com', 'Super', NULL, 'Admin', 'Super', 'Super Administrator'),
('87654321-4321-4321-4321-210987654321', 0, 'e9b5c4a3-2d8f-4c1b-9e6d-12a3b4c5d6e7', 'user@example.com', 1, 1, NULL, 'USER@EXAMPLE.COM', 'USER@EXAMPLE.COM', 'AQAAAAIAAYagAAAAEO1IxTkA/qQyZkFuZtIVJWFPtke4A+UMvLZYMIKtTdCrOQPEHhUsVvrRJ0mT1Xbv1A==', NULL, 0, 'LMNOPQRSTUVWXYZ1234567890ABCDEF', 0, 'user@example.com', 'User', NULL, 'User', 'User', 'User');


INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
('68828e21-4ac8-43ff-b717-160555d199e9', '8aeab3f3-540c-4997-8452-b151d5a40391'),
('12345678-1234-1234-1234-123456789012', '4e3a3f3e-540c-4997-8452-b151d5a40392'), -- SuperAdministrator
('87654321-4321-4321-4321-210987654321', '9f86d081-84f7-367d-96e7-48b5e3d8a2f4');


--INSERT INTO Agency (AgencyStatusId, CityId, RegionId, Name, StateDepartmentRegistration, UieNumber, EinNumber, Address, PostalCode, Latitude, Longitude, Phone, CreatedAt, UpdatedAt)
--VALUES (1, 1, 1, 'Nombre de la Agencia', 'Registro del Departamento de Estado', 123456, 789012, 'Dirección de la Agencia', 'Código Postal', 37.7749, -122.4194, 'Teléfono de la Agencia', GETDATE(), NULL);
