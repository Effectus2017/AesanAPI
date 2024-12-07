
-- Insertar estados de la agencia
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Pendiente de validar');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Orientación');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Visita Pre-operacional');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('No cumple con los requisitos');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Cumple con los requisitos');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Rechazado');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Aprobado');

-- Insertar la agencia AESAN por defecto
INSERT INTO Agency
    (Name, StatusId, CityId, RegionId, ProgramId, SdrNumber, UieNumber, EinNumber, Address, ZipCode, PostalAddress, Latitude, Longitude, Phone, Email, ImageURL, IsActive, IsListable)
VALUES
    ('AESAN', 5, 1, 1, 7, 123456, 123456, 123456, 'Calle Principal #123', 00123, 'Calle Principal #123', 18.220833, -66.590149, '787-123-4567', 'admin@aesan.pr.gov', NULL, 1, 1);

INSERT INTO UserProgram
    (UserId, ProgramId, CreatedAt)
VALUES
    ('87654321-4321-4321-4321-210987654321', 1, GETDATE());

INSERT INTO Program
    (Name, Description)
VALUES
    ('PDAM', 'Programa de Desayuno Escolar');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PSAV', 'Programa de Servicios de Alimentos de Verano');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PFHF', 'Programa de Frutas y Hortalizas Frescas');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PAF', 'Programa de Distribución de Alimentos Federales');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PDFE', 'Programa de la Finca a la Escuela');
INSERT INTO Program
    (Name, Description)
VALUES
    ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos');

INSERT INTO City
    (Name)
VALUES
    ('Arecibo');
INSERT INTO City
    (Name)
VALUES
    ('Bayamón');
INSERT INTO City
    (Name)
VALUES
    ('Mayagüez');
INSERT INTO City
    (Name)
VALUES
    ('Ponce');
INSERT INTO City
    (Name)
VALUES
    ('Caguas');
INSERT INTO City
    (Name)
VALUES
    ('San Juan');
INSERT INTO City
    (Name)
VALUES
    ('Humacao');

INSERT INTO Region
    (Name, CityId)
VALUES
    ('Arecibo', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguada', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Barceloneta', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Camuy', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ciales', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Dorado', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Florida', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Hatillo', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Lares', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Manatí', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Quebradillas', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vega Alta', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vega Baja', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Bayamón', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cataño', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Corozal', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cabo Rojo', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguadilla', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Isabela', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Lajas', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Las Marías', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Maricao', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Mayagüez', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Adjuntas', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Coamo', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guayanilla', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guánica', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Jayuya', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ponce', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguas Buenas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aibonito', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Arroyo', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Barranquitas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Caguas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cayey', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cidra', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guayama', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Carolina', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guaynabo', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('San Juan', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Trujillo Alto', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ceiba', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Fajardo', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Juncos', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Las Piedras', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Rio Grande', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('San Lorenzo', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vieques', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Yabucoa', 7);

-- Insertar roles
INSERT INTO AspNetRoles
    (Id, ConcurrencyStamp, Name, NormalizedName)
VALUES
    ('8aeab3f3-540c-4997-8452-b151d5a40391', '72401a60-d83b-4687-9827-d4d610d1a3e4', 'Administrator', 'ADMINISTRATOR'),
    ('9f86d081-84f7-367d-96e7-48b5e3d8a2f4', 'e2fc714f-5d5b-42a4-93b2-0e9c05e6a6b5', 'Monitor', 'MONITOR'),
    ('4e3a3f3e-540c-4997-8452-b151d5a40392', 'a2b3c4d5-e6f7-g8h9-i1j2-k3l4m5n6o7p8', 'SuperAdministrator', 'SUPERADMINISTRATOR');

INSERT INTO AspNetRoles
    (Id, ConcurrencyStamp, Name, NormalizedName)
VALUES
    ('d1e2f3g4-5678-1234-5678-1234567890ab', 'abcdef12-3456-7890-abcd-ef1234567890', 'Agency-Administrator', 'AGENCY-ADMINISTRATOR');

INSERT INTO AspNetRoles
    (Id, ConcurrencyStamp, Name, NormalizedName)
VALUES
    ('e2f3g4h5-6789-0123-4567-890123456789', 'abcdef12-3456-7890-abcd-ef1234567890', 'Agency-User', 'AGENCY-USER');

-- Admin: @dmin5812931!
-- SuperAdministrator: SuperAdmin123!
-- User: UserPassword456!
-- Inserting 28 rows into dbo.AspNetUsers
-- Insert batch #1
INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled, LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, FirstName, MiddleName, FatherLastName, MotherLastName, AdministrationTitle)
VALUES
    ('68828e21-4ac8-43ff-b717-160555d199e9', 0, '6f4762e6-95af-41af-a403-9ef0d426e95d', 'admin@admin.com', 0, 1, NULL, 'ADMIN@ADMIN.COM', 'ADMIN@ADMIN.COM', 'AE8yhHOU45pC7AX/ZZnN0vCLGwzf/JgYQAwAzjhG+9BkQfSiolhKkLL9yIto6EUz5g==', NULL, 0, 'I33YA5SKNJS5UAFSULWBKTUMII4RZI6G', 0, 'admin@admin.com', 'Admin', NULL, 'User', 'Admin', 'Administrator'),
    ('12345678-1234-1234-1234-123456789012', 0, 'f8b0f8d1-9c7e-4b5a-8a1d-26c8f6ae6b6f', 'superadmin@example.com', 1, 1, NULL, 'SUPERADMIN@EXAMPLE.COM', 'SUPERADMIN@EXAMPLE.COM', 'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==', NULL, 0, 'RMZCOGPLZ4DAPL6VXZWFMFNX44MGGBKJ', 0, 'superadmin@example.com', 'Super', NULL, 'Admin', 'Super', 'Super Administrator'),
    ('87654321-4321-4321-4321-210987654321', 0, 'e9b5c4a3-2d8f-4c1b-9e6d-12a3b4c5d6e7', 'user@example.com', 1, 1, NULL, 'USER@EXAMPLE.COM', 'USER@EXAMPLE.COM', 'AQAAAAIAAYagAAAAEO1IxTkA/qQyZkFuZtIVJWFPtke4A+UMvLZYMIKtTdCrOQPEHhUsVvrRJ0mT1Xbv1A==', NULL, 0, 'LMNOPQRSTUVWXYZ1234567890ABCDEF', 0, 'user@example.com', 'User', NULL, 'User', 'User', 'User');

-- Insertar roles de usuario
-- INSERT INTO AspNetUsers
--     (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled, LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, FirstName, MiddleName, FatherLastName, MotherLastName, AdministrationTitle)
-- VALUES
--     ('abcdef12-3456-7890-abcd-ef1234567890', 0, 'abcdef12-3456-7890-abcd-ef1234567890', 'monitor@example.com', 1, 1, NULL, 'MONITOR@EXAMPLE.COM', 'MONITOR@EXAMPLE.COM', 'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==', NULL, 0, 'RMZCOGPLZ4DAPL6VXZWFMFNX44MGGBKJ', 0, 'monitor@example.com', 'Monitor', NULL, 'User', 'Monitor', 'Monitor');

-- INSERT INTO AspNetUserRoles
--     (UserId, RoleId)
-- VALUES
--     ('abcdef12-3456-7890-abcd-ef1234567890', '9f86d081-84f7-367d-96e7-48b5e3d8a2f4');


INSERT INTO AspNetUserRoles
    (UserId, RoleId)
VALUES
    ('68828e21-4ac8-43ff-b717-160555d199e9', '8aeab3f3-540c-4997-8452-b151d5a40391'),
    ('12345678-1234-1234-1234-123456789012', '4e3a3f3e-540c-4997-8452-b151d5a40392'),
    -- SuperAdministrator
    ('87654321-4321-4321-4321-210987654321', '9f86d081-84f7-367d-96e7-48b5e3d8a2f4');


--INSERT INTO Agency (AgencyStatusId, CityId, RegionId, Name, StateDepartmentRegistration, UieNumber, EinNumber, Address, PostalCode, Latitude, Longitude, Phone, CreatedAt, UpdatedAt)
--VALUES (1, 1, 1, 'Nombre de la Agencia', 'Registro del Departamento de Estado', 123456, 789012, 'Dirección de la Agencia', 'Código Postal', 37.7749, -122.4194, 'Teléfono de la Agencia', GETDATE(), NULL);
