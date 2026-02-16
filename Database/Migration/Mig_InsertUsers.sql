-- =============================================
-- Migración: Insertar usuarios NUTRE y administradores (seed)
-- =============================================
-- REQUISITO: Los roles deben existir en AspNetRoles antes de ejecutar este script.
--   - NUTRE: 'Coordinadora de Monitoría' o 'Coordinador'
--   - Super-Administrator: 'Super-Administrator'
--   - Administrator: 'Administrator' o 'Administrador'
-- Ejecutar antes, si aplica: Database/Migration/Mig_AesanRolesNuevos.sql (o el script que cree los roles).
-- AspNetUsers: solo columnas de Identity. FirstName, MiddleName, FatherLastName, MotherLastName van en Staff (INSERT más abajo).
-- =============================================

INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
    LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
    PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, IsActive,
    IsTemporalPasswordActived, CreatedAt)
VALUES
    (NEWID(), 0, NEWID(), 'abnerys.rivera@nutre.com', 1, 1, NULL,
    'ABNERYS.RIVERA@NUTRE.COM', 'ABNERYS.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'abnerys.rivera@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'brunilda.perez@nutre.com', 1, 1, NULL,
    'BRUNILDA.PEREZ@NUTRE.COM', 'BRUNILDA.PEREZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'brunilda.perez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'carmen.rodriguez@nutre.com', 1, 1, NULL,
    'CARMEN.RODRIGUEZ@NUTRE.COM', 'CARMEN.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'carmen.rodriguez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'daisy.rivera@nutre.com', 1, 1, NULL,
    'DAISY.RIVERA@NUTRE.COM', 'DAISY.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'daisy.rivera@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'emma.colom@nutre.com', 1, 1, NULL,
    'EMMA.COLOM@NUTRE.COM', 'EMMA.COLOM@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'emma.colom@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'enid.llabres@nutre.com', 1, 1, NULL,
    'ENID.LLABRES@NUTRE.COM', 'ENID.LLABRES@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'enid.llabres@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'jeanning.rodriguez@nutre.com', 1, 1, NULL,
    'JEANNING.RODRIGUEZ@NUTRE.COM', 'JEANNING.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'jeanning.rodriguez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'lillian.feliciano@nutre.com', 1, 1, NULL,
    'LILLIAN.FELICIANO@NUTRE.COM', 'LILLIAN.FELICIANO@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'lillian.feliciano@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'maria.rivera@nutre.com', 1, 1, NULL,
    'MARIA.RIVERA@NUTRE.COM', 'MARIA.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'maria.rivera@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'maritza.gomez@nutre.com', 1, 1, NULL,
    'MARITZA.GOMEZ@NUTRE.COM', 'MARITZA.GOMEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'maritza.gomez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'martha.berrios@nutre.com', 1, 1, NULL,
    'MARTHA.BERRIOS@NUTRE.COM', 'MARTHA.BERRIOS@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'martha.berrios@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'miliana.rivera@nutre.com', 1, 1, NULL,
    'MILIANA.RIVERA@NUTRE.COM', 'MILIANA.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'miliana.rivera@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'yessica.rodriguez@nutre.com', 1, 1, NULL,
    'YESSICA.RODRIGUEZ@NUTRE.COM', 'YESSICA.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'yessica.rodriguez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'yolanda.ortiz@nutre.com', 1, 1, NULL,
    'YOLANDA.ORTIZ@NUTRE.COM', 'YOLANDA.ORTIZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'yolanda.ortiz@nutre.com', 1, 0, GETDATE());
GO

-- Staff: FirstName, MiddleName, FatherLastName, MotherLastName para los usuarios NUTRE insertados arriba.
INSERT INTO Staff (FirstName, MiddleName, FatherLastName, MotherLastName, StatusId, PositionId, StaffTypeId, BirthDate, Email, PostalAddress, CityId, RegionId, ZipCode, AgencyId, UserId, CreatedAt, IsActive)
SELECT v.FirstName, v.MiddleName, v.FatherLastName, v.MotherLastName, 1, 37, 1, '2000-01-01', v.Email, N'Dirección por definir', 1, 1, '00901', NULL, u.Id, GETDATE(), 1
FROM (VALUES
    (N'abnerys.rivera@nutre.com', N'Abnerys', NULL, N'Rivera', N'Rodrigez'),
    (N'brunilda.perez@nutre.com', N'Brunilda', NULL, N'Pérez', N'Pérez'),
    (N'carmen.rodriguez@nutre.com', N'Carmen', N'M.', N'Rodríguez', N'Corales'),
    (N'daisy.rivera@nutre.com', N'Daisy', NULL, N'Rivera', N'Roldán'),
    (N'emma.colom@nutre.com', N'Emma', N'I.', N'Colom', N'Ríos'),
    (N'enid.llabres@nutre.com', N'Enid', N'Y.', N'Llabrés', N'Santana'),
    (N'jeanning.rodriguez@nutre.com', N'Jeanning', NULL, N'Rodríguez', N'Rodríguez'),
    (N'lillian.feliciano@nutre.com', N'Lillian', N'M.', N'Feliciano', N'Ramírez'),
    (N'maria.rivera@nutre.com', N'María', N'de los A.', N'Rivera', N'Rosario'),
    (N'maritza.gomez@nutre.com', N'Maritza', NULL, N'Gómez', N'Orlando'),
    (N'martha.berrios@nutre.com', N'Martha', N'X.', N'Berrios', N'Reyes'),
    (N'miliana.rivera@nutre.com', N'Miliana', NULL, N'Rivera', N'Colón'),
    (N'yessica.rodriguez@nutre.com', N'Yessica', NULL, N'Rodríguez', N'Torres'),
    (N'yolanda.ortiz@nutre.com', N'Yolanda', N'J.', N'Ortiz', N'Rivera')
) AS v(Email, FirstName, MiddleName, FatherLastName, MotherLastName)
INNER JOIN AspNetUsers u ON u.Email = v.Email;
GO

-- Asignar rol NUTRE (Coordinadora de Monitoría o Coordinador). Si el rol no existe, fallar para no dejar usuarios sin rol.
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = N'Coordinadora de Monitoría' OR Name = N'Coordinador') AND IsActive = 1)
BEGIN
    RAISERROR(N'No existe rol Coordinadora de Monitoría ni Coordinador en AspNetRoles. Ejecute antes la migración de roles (p. ej. Mig_AesanRolesNuevos.sql).', 16, 1);
    RETURN;
END

INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
SELECT u.Id, r.Id, 1, GETDATE()
FROM AspNetUsers u
CROSS JOIN (SELECT TOP 1 Id FROM AspNetRoles WHERE (Name = N'Coordinadora de Monitoría' OR Name = N'Coordinador') AND IsActive = 1) r
WHERE u.Email IN (
    'abnerys.rivera@nutre.com',
    'brunilda.perez@nutre.com',
    'carmen.rodriguez@nutre.com',
    'daisy.rivera@nutre.com',
    'emma.colom@nutre.com',
    'enid.llabres@nutre.com',
    'jeanning.rodriguez@nutre.com',
    'lillian.feliciano@nutre.com',
    'maria.rivera@nutre.com',
    'maritza.gomez@nutre.com',
    'martha.berrios@nutre.com',
    'miliana.rivera@nutre.com',
    'yessica.rodriguez@nutre.com',
    'yolanda.ortiz@nutre.com'
);
GO

-- Insertar usuarios Administradores (Super-Administrator) y Empleados Administrativos (Administrator)
-- FirstName, MiddleName, FatherLastName, MotherLastName van en Staff (INSERT más abajo).
INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
    LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
    PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, IsActive,
    IsTemporalPasswordActived, CreatedAt)
VALUES
    (NEWID(), 0, NEWID(), 'marta.melendez@nutre.com', 1, 1, NULL,
    'MARTA.MELENDEZ@NUTRE.COM', 'MARTA.MELENDEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'marta.melendez@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'alberto.miranda@nutre.com', 1, 1, NULL,
    'ALBERTO.MIRANDA@NUTRE.COM', 'ALBERTO.MIRANDA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'alberto.miranda@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'lourdes.garcia@nutre.com', 1, 1, NULL,
    'LOURDES.GARCIA@NUTRE.COM', 'LOURDES.GARCIA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'lourdes.garcia@nutre.com', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'odalis.menard@nutre.com', 1, 1, NULL,
    'ODALIS.MENARD@NUTRE.COM', 'ODALIS.MENARD@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'odalis.menard@nutre.com', 1, 0, GETDATE());
GO

-- Staff: FirstName, MiddleName, FatherLastName, MotherLastName para administradores y empleados administrativos.
INSERT INTO Staff (FirstName, MiddleName, FatherLastName, MotherLastName, StatusId, PositionId, StaffTypeId, BirthDate, Email, PostalAddress, CityId, RegionId, ZipCode, AgencyId, UserId, CreatedAt, IsActive)
SELECT v.FirstName, v.MiddleName, v.FatherLastName, v.MotherLastName, 1, 37, 1, '2000-01-01', v.Email, N'Dirección por definir', 1, 1, '00901', NULL, u.Id, GETDATE(), 1
FROM (VALUES
    (N'marta.melendez@nutre.com', N'Marta', NULL, N'Meléndez', N'Meléndez'),
    (N'alberto.miranda@nutre.com', N'Alberto', NULL, N'Miranda', N'Miranda'),
    (N'lourdes.garcia@nutre.com', N'Lourdes', NULL, N'García', N'García'),
    (N'odalis.menard@nutre.com', N'Odalis', N'Ariel', N'Menard', N'Menard')
) AS v(Email, FirstName, MiddleName, FatherLastName, MotherLastName)
INNER JOIN AspNetUsers u ON u.Email = v.Email;
GO

-- Asignar rol Super-Administrator a Marta Meléndez y Alberto Miranda (resolver RoleId por nombre)
INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
SELECT u.Id, r.Id, 1, GETDATE()
FROM AspNetUsers u
CROSS JOIN (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Super-Administrator' AND IsActive = 1) r
WHERE u.Email IN (
    'marta.melendez@nutre.com',
    'alberto.miranda@nutre.com'
);
GO

-- Asignar rol Administrator a Lourdes García y Odalis Ariel Menard (resolver RoleId por nombre)
INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
SELECT u.Id, r.Id, 1, GETDATE()
FROM AspNetUsers u
CROSS JOIN (SELECT TOP 1 Id FROM AspNetRoles WHERE Name IN (N'Administrator', N'Administrador') AND IsActive = 1) r
WHERE u.Email IN (
    'lourdes.garcia@nutre.com',
    'odalis.menard@nutre.com'
);
GO
