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

-- Insertar regiones (principales)
INSERT INTO Region
    (Name, IsActive, CreatedAt)
VALUES
    ('Arecibo', 1, GETDATE()),
    ('Bayamón', 1, GETDATE()),
    ('Mayagüez', 1, GETDATE()),
    ('Ponce', 1, GETDATE()),
    ('Caguas', 1, GETDATE()),
    ('San Juan', 1, GETDATE()),
    ('Humacao', 1, GETDATE());

-- Insertar ciudades
INSERT INTO City
    (Name, IsActive, CreatedAt)
VALUES
    -- Ciudades de Arecibo (12 municipios)
    ('Arecibo', 1, GETDATE()),
    ('Barceloneta', 1, GETDATE()),
    ('Camuy', 1, GETDATE()),
    ('Ciales', 1, GETDATE()),
    ('Dorado', 1, GETDATE()),
    ('Florida', 1, GETDATE()),
    ('Hatillo', 1, GETDATE()),
    ('Lares', 1, GETDATE()),
    ('Manatí', 1, GETDATE()),
    ('Quebradillas', 1, GETDATE()),
    ('Vega Alta', 1, GETDATE()),
    ('Vega Baja', 1, GETDATE()),
    -- Ciudades de Bayamón (8 municipios)
    ('Bayamón', 1, GETDATE()),
    ('Cataño', 1, GETDATE()),
    ('Corozal', 1, GETDATE()),
    ('Morovis', 1, GETDATE()),
    ('Naranjito', 1, GETDATE()),
    ('Orocovis', 1, GETDATE()),
    ('Toa Alta', 1, GETDATE()),
    ('Toa Baja', 1, GETDATE()),
    -- Ciudades de Mayagüez (15 municipios)
    ('Aguada', 1, GETDATE()),
    ('Aguadilla', 1, GETDATE()),
    ('Añasco', 1, GETDATE()),
    ('Cabo Rojo', 1, GETDATE()),
    ('Hormigueros', 1, GETDATE()),
    ('Isabela', 1, GETDATE()),
    ('Lajas', 1, GETDATE()),
    ('Las Marías', 1, GETDATE()),
    ('Maricao', 1, GETDATE()),
    ('Mayagüez', 1, GETDATE()),
    ('Moca', 1, GETDATE()),
    ('Rincón', 1, GETDATE()),
    ('Sabana Grande', 1, GETDATE()),
    ('San Germán', 1, GETDATE()),
    ('San Sebastián', 1, GETDATE()),
    -- Ciudades de Ponce (12 municipios)
    ('Adjuntas', 1, GETDATE()),
    ('Coamo', 1, GETDATE()),
    ('Guayanilla', 1, GETDATE()),
    ('Guánica', 1, GETDATE()),
    ('Jayuya', 1, GETDATE()),
    ('Juana Díaz', 1, GETDATE()),
    ('Peñuelas', 1, GETDATE()),
    ('Ponce', 1, GETDATE()),
    ('Santa Isabel', 1, GETDATE()),
    ('Utuado', 1, GETDATE()),
    ('Villalba', 1, GETDATE()),
    ('Yauco', 1, GETDATE()),
    -- Ciudades de Caguas (11 municipios)
    ('Aguas Buenas', 1, GETDATE()),
    ('Aibonito', 1, GETDATE()),
    ('Arroyo', 1, GETDATE()),
    ('Barranquitas', 1, GETDATE()),
    ('Caguas', 1, GETDATE()),
    ('Cayey', 1, GETDATE()),
    ('Cidra', 1, GETDATE()),
    ('Comerío', 1, GETDATE()),
    ('Guayama', 1, GETDATE()),
    ('Gurabo', 1, GETDATE()),
    ('Salinas', 1, GETDATE()),
    -- Ciudades de San Juan (4 municipios)
    ('Carolina', 1, GETDATE()),
    ('Guaynabo', 1, GETDATE()),
    ('San Juan', 1, GETDATE()),
    ('Trujillo Alto', 1, GETDATE()),
    -- Ciudades de Humacao (16 municipios)
    ('Canóvanas', 1, GETDATE()),
    ('Ceiba', 1, GETDATE()),
    ('Culebra', 1, GETDATE()),
    ('Fajardo', 1, GETDATE()),
    ('Humacao', 1, GETDATE()),
    ('Juncos', 1, GETDATE()),
    ('Las Piedras', 1, GETDATE()),
    ('Loiza', 1, GETDATE()),
    ('Luquillo', 1, GETDATE()),
    ('Maunabo', 1, GETDATE()),
    ('Naguabo', 1, GETDATE()),
    ('Patillas', 1, GETDATE()),
    ('Rio Grande', 1, GETDATE()),
    ('San Lorenzo', 1, GETDATE()),
    ('Vieques', 1, GETDATE()),
    ('Yabucoa', 1, GETDATE());

-- Insertar relaciones Ciudad-Región
INSERT INTO CityRegion
    (CityId, RegionId, IsActive, CreatedAt)
SELECT c.Id, r.Id, 1, GETDATE()
FROM City c
    CROSS APPLY (
    SELECT r.Id
    FROM Region r
    WHERE 
        (r.Name = 'Arecibo' AND c.Id BETWEEN 1 AND 12) OR
        (r.Name = 'Bayamón' AND c.Id BETWEEN 13 AND 20) OR
        (r.Name = 'Mayagüez' AND c.Id BETWEEN 21 AND 35) OR
        (r.Name = 'Ponce' AND c.Id BETWEEN 36 AND 47) OR
        (r.Name = 'Caguas' AND c.Id BETWEEN 48 AND 58) OR
        (r.Name = 'San Juan' AND c.Id BETWEEN 59 AND 62) OR
        (r.Name = 'Humacao' AND c.Id BETWEEN 63 AND 78) -- Actualizado para 16 ciudades
) r;

-- Insertar programas
INSERT INTO Program
    (Name, Description, IsActive, CreatedAt)
VALUES
    ('PDAM', 'Programa de Desayuno Escolar', 1, GETDATE()),
    ('PSAV', 'Programa de Servicios de Alimentos de Verano', 1, GETDATE()),
    ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto', 1, GETDATE()),
    ('PFHF', 'Programa de Frutas y Hortalizas Frescas', 1, GETDATE()),
    ('PAF', 'Programa de Distribución de Alimentos Federales', 1, GETDATE()),
    ('PDFE', 'Programa de la Finca a la Escuela', 1, GETDATE()),
    ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos', 1, GETDATE());

-- Insertar la agencia AESAN por defecto
INSERT INTO Agency
    (Name, StatusId, CityRegionId, PostalCityRegionId, ProgramId, SdrNumber, UieNumber, EinNumber, 
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageURL, 
    IsActive, IsListable, CreatedAt)
VALUES
    ('AESAN', 5, 1, 1, 7, 123456, 123456, 123456, 'Calle Principal #123', 00123, 
    'Calle Principal #123', 00123, 18.220833, -66.590149, '787-123-4567', 
    'admin@aesan.pr.gov', NULL, 1, 1, GETDATE());

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

-- Crear índices para mejorar el rendimiento
CREATE INDEX IX_CityRegion_CityId ON CityRegion(CityId);
CREATE INDEX IX_CityRegion_RegionId ON CityRegion(RegionId);
CREATE INDEX IX_UserProgram_UserId ON UserProgram(UserId);
CREATE INDEX IX_UserProgram_ProgramId ON UserProgram(ProgramId); 