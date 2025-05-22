
-- Crear tabla AspNetUsers con los campos necesarios
CREATE TABLE AspNetUsers
(
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
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    FatherLastName NVARCHAR(100) NOT NULL,
    MotherLastName NVARCHAR(100) NOT NULL,
    AdministrationTitle NVARCHAR(100) NOT NULL,
    ImageURL NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsTemporalPasswordActived BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);

CREATE TABLE AspNetRoles
(
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    Description NVARCHAR(MAX) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

ALTER TABLE AspNetRoles
ADD Description NVARCHAR(MAX) NULL;

CREATE TABLE AspNetUserRoles
(
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE AspNetUserClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE AspNetRoleClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE AspNetUserLogins
(
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Insertar roles
INSERT INTO AspNetRoles
    (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
VALUES
    ('8aeab3f3-540c-4997-8452-b151d5a40391', 'Administrator', 'ADMINISTRATOR', '72401a60-d83b-4687-9827-d4d610d1a3e4', 1, GETDATE()),
    ('9f86d081-84f7-367d-96e7-48b5e3d8a2f4', 'Monitor', 'MONITOR', 'e2fc714f-5d5b-42a4-93b2-0e9c05e6a6b5', 1, GETDATE()),
    ('4e3a3f3e-540c-4997-8452-b151d5a40392', 'SuperAdministrator', 'SUPERADMINISTRATOR', 'a2b3c4d5-e6f7-g8h9-i1j2-k3l4m5n6o7p8', 1, GETDATE()),
    ('d1e2f3g4-5678-1234-5678-1234567890ab', 'Agency-Administrator', 'AGENCY-ADMINISTRATOR', 'abcdef12-3456-7890-abcd-ef1234567890', 1, GETDATE()),
    ('e2f3g4h5-6789-0123-4567-890123456789', 'Agency-User', 'AGENCY-USER', 'abcdef12-3456-7890-abcd-ef1234567890', 1, GETDATE());

-- Insertar usuarios
-- Admin: @dmin5812931!
-- SuperAdministrator: SuperAdmin123!
-- User: UserPassword456!
INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
    LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
    PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, FirstName,
    MiddleName, FatherLastName, MotherLastName, AdministrationTitle, IsActive,
    IsTemporalPasswordActived, CreatedAt)
VALUES
    ('68828e21-4ac8-43ff-b717-160555d199e9', 0, '6f4762e6-95af-41af-a403-9ef0d426e95d',
        'admin@admin.com', 0, 1, NULL, 'ADMIN@ADMIN.COM', 'ADMIN@ADMIN.COM',
        'AE8yhHOU45pC7AX/ZZnN0vCLGwzf/JgYQAwAzjhG+9BkQfSiolhKkLL9yIto6EUz5g==',
        NULL, 0, 'I33YA5SKNJS5UAFSULWBKTUMII4RZI6G', 0, 'admin@admin.com',
        'Admin', NULL, 'User', 'Admin', 'Administrator', 1, 0, GETDATE()),

    ('12345678-1234-1234-1234-123456789012', 0, 'f8b0f8d1-9c7e-4b5a-8a1d-26c8f6ae6b6f',
        'superadmin@example.com', 1, 1, NULL, 'SUPERADMIN@EXAMPLE.COM', 'SUPERADMIN@EXAMPLE.COM',
        'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
        NULL, 0, 'RMZCOGPLZ4DAPL6VXZWFMFNX44MGGBKJ', 0, 'superadmin@example.com',
        'Super', NULL, 'Admin', 'Super', 'Super Administrator', 1, 0, GETDATE()),

    ('87654321-4321-4321-4321-210987654321', 0, 'e9b5c4a3-2d8f-4c1b-9e6d-12a3b4c5d6e7',
        'user@example.com', 1, 1, NULL, 'USER@EXAMPLE.COM', 'USER@EXAMPLE.COM',
        'AQAAAAIAAYagAAAAEO1IxTkA/qQyZkFuZtIVJWFPtke4A+UMvLZYMIKtTdCrOQPEHhUsVvrRJ0mT1Xbv1A==',
        NULL, 0, 'LMNOPQRSTUVWXYZ1234567890ABCDEF', 0, 'user@example.com',
        'User', NULL, 'User', 'User', 'User', 1, 0, GETDATE());

-- Insertar roles de usuario
INSERT INTO AspNetUserRoles
    (UserId, RoleId, IsActive, CreatedAt)
VALUES
    ('68828e21-4ac8-43ff-b717-160555d199e9', '8aeab3f3-540c-4997-8452-b151d5a40391', 1, GETDATE()),
    ('12345678-1234-1234-1234-123456789012', '4e3a3f3e-540c-4997-8452-b151d5a40392', 1, GETDATE()),
    ('87654321-4321-4321-4321-210987654321', '9f86d081-84f7-367d-96e7-48b5e3d8a2f4', 1, GETDATE());



INSERT INTO AgencyUsers
    (
    UserId,
    AgencyId,
    IsOwner,
    IsMonitor,
    IsActive,
    AssignedDate,
    AssignedBy,
    CreatedAt,
    UpdatedAt
    )
VALUES
    ('68828e21-4ac8-43ff-b717-160555d199e9',
        1,
        1,
        0,
        1,
        '2025-03-25 19:37:13.317',
        '68828e21-4ac8-43ff-b717-160555d199e9',
        '2025-03-25 19:37:13.317',
        NULL
    );