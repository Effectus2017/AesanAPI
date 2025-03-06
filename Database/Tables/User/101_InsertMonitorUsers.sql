-- Insertar usuarios Monitores
INSERT INTO AspNetUsers
    (Id, AccessFailedCount, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled, 
    LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, 
    PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName, FirstName, 
    MiddleName, FatherLastName, MotherLastName, AdministrationTitle, IsActive, 
    IsTemporalPasswordActived, CreatedAt)
VALUES
    (NEWID(), 0, NEWID(), 'abnerys.rivera@nutre.com', 1, 1, NULL, 
    'ABNERYS.RIVERA@NUTRE.COM', 'ABNERYS.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'abnerys.rivera@nutre.com', 'Abnerys', NULL, 'Rivera', 'Rodrigez', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'brunilda.perez@nutre.com', 1, 1, NULL,
    'BRUNILDA.PEREZ@NUTRE.COM', 'BRUNILDA.PEREZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'brunilda.perez@nutre.com', 'Brunilda', NULL, 'Pérez', 'Pérez', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'carmen.rodriguez@nutre.com', 1, 1, NULL,
    'CARMEN.RODRIGUEZ@NUTRE.COM', 'CARMEN.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'carmen.rodriguez@nutre.com', 'Carmen', 'M.', 'Rodríguez', 'Corales', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'daisy.rivera@nutre.com', 1, 1, NULL,
    'DAISY.RIVERA@NUTRE.COM', 'DAISY.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'daisy.rivera@nutre.com', 'Daisy', NULL, 'Rivera', 'Roldán', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'emma.colom@nutre.com', 1, 1, NULL,
    'EMMA.COLOM@NUTRE.COM', 'EMMA.COLOM@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'emma.colom@nutre.com', 'Emma', 'I.', 'Colom', 'Ríos', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'enid.llabres@nutre.com', 1, 1, NULL,
    'ENID.LLABRES@NUTRE.COM', 'ENID.LLABRES@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'enid.llabres@nutre.com', 'Enid', 'Y.', 'Llabrés', 'Santana', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'jeanning.rodriguez@nutre.com', 1, 1, NULL,
    'JEANNING.RODRIGUEZ@NUTRE.COM', 'JEANNING.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'jeanning.rodriguez@nutre.com', 'Jeanning', NULL, 'Rodríguez', 'Rodríguez', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'lillian.feliciano@nutre.com', 1, 1, NULL,
    'LILLIAN.FELICIANO@NUTRE.COM', 'LILLIAN.FELICIANO@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'lillian.feliciano@nutre.com', 'Lillian', 'M.', 'Feliciano', 'Ramírez', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'maria.rivera@nutre.com', 1, 1, NULL,
    'MARIA.RIVERA@NUTRE.COM', 'MARIA.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'maria.rivera@nutre.com', 'María', 'de los A.', 'Rivera', 'Rosario', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'maritza.gomez@nutre.com', 1, 1, NULL,
    'MARITZA.GOMEZ@NUTRE.COM', 'MARITZA.GOMEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'maritza.gomez@nutre.com', 'Maritza', NULL, 'Gómez', 'Orlando', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'martha.berrios@nutre.com', 1, 1, NULL,
    'MARTHA.BERRIOS@NUTRE.COM', 'MARTHA.BERRIOS@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'martha.berrios@nutre.com', 'Martha', 'X.', 'Berrios', 'Reyes', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'miliana.rivera@nutre.com', 1, 1, NULL,
    'MILIANA.RIVERA@NUTRE.COM', 'MILIANA.RIVERA@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'miliana.rivera@nutre.com', 'Miliana', NULL, 'Rivera', 'Colón', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'yessica.rodriguez@nutre.com', 1, 1, NULL,
    'YESSICA.RODRIGUEZ@NUTRE.COM', 'YESSICA.RODRIGUEZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'yessica.rodriguez@nutre.com', 'Yessica', NULL, 'Rodríguez', 'Torres', 'Monitor', 1, 0, GETDATE()),

    (NEWID(), 0, NEWID(), 'yolanda.ortiz@nutre.com', 1, 1, NULL,
    'YOLANDA.ORTIZ@NUTRE.COM', 'YOLANDA.ORTIZ@NUTRE.COM',
    'AQAAAAIAAYagAAAAELBG1xFqjmYTTyA4QQtU6oT3wSxgYTK4M5zzWuXYhLuA8+f+bKxxqtJJtbTDnB2+tw==',
    NULL, 0, NEWID(), 0, 'yolanda.ortiz@nutre.com', 'Yolanda', 'J.', 'Ortiz', 'Rivera', 'Monitor', 1, 0, GETDATE());
GO

-- Asignar rol Monitor a los usuarios específicos
INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
SELECT u.Id, '9f86d081-84f7-367d-96e7-48b5e3d8a2f4', 1, GETDATE()
FROM AspNetUsers u 
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