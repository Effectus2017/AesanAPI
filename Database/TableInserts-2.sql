-- Insertar estados de la agencia
INSERT INTO AgencyStatus
    (Name, IsActive, CreatedAt)
VALUES
    ('Pendiente de validar', 1, GETDATE()),
    ('Orientación', 1, GETDATE()),
    ('Visita Pre-operacional', 1, GETDATE()),
    ('No cumple con los requisitos', 1, GETDATE()),
    ('Cumple con los requisitos', 1, GETDATE()),
    ('Rechazado', 1, GETDATE()),
    ('Aprobado', 1, GETDATE());

-- Insertar programas
INSERT INTO Program
    (Name, Description, IsActive, CreatedAt)
VALUES
    ('PDAM', 'Programa de Desayuno Escolar', 1, GETDATE()),
    ('PSAV', 'Programa de Servicios de Alimentos de Verano', 1, GETDATE()),
    ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto', 1, GETDATE()),
    ('PFHF', 'Programa de Frutas y Hortalizas Frescas', 1, GETDATE()),
    --('PAF', 'Programa de Distribución de Alimentos Federales', 0, GETDATE()),
    ('PDFE', 'Programa de la Finca a la Escuela', 1, GETDATE()),
    ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos', 1, GETDATE());

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

-- Insertar programas de usuario
INSERT INTO UserProgram
    (UserId, ProgramId, IsActive, CreatedAt)
VALUES
    ('87654321-4321-4321-4321-210987654321', 1, 1, GETDATE());