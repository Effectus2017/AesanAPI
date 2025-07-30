INSERT INTO Permission
    (Id, Name, Description, DescriptionEn, IsActive)
VALUES
    (1, 'UserView', 'Ver usuarios', 'View users', 1),
    (2, 'UserCreate', 'Crear usuarios', 'Create users', 1),
    (3, 'UserEdit', 'Editar usuarios', 'Edit users', 1),
    (4, 'UserDelete', 'Eliminar usuarios', 'Delete users', 1),
    (5, 'UserAssignRole', 'Asignar roles a usuarios', 'Assign roles to users', 1),
    (6, 'UserAssignAgency', 'Asignar agencias a usuarios', 'Assign agencies to users', 1),

    (7, 'RoleView', 'Ver roles', 'View roles', 1),
    (8, 'RoleCreate', 'Crear roles', 'Create roles', 1),
    (9, 'RoleEdit', 'Editar roles', 'Edit roles', 1),
    (10, 'RoleDelete', 'Eliminar roles', 'Delete roles', 1),
    (11, 'RoleAssignPermission', 'Asignar permisos a roles', 'Assign permissions to roles', 1),

    (12, 'AgencyView', 'Ver agencias', 'View agencies', 1),
    (13, 'AgencyCreate', 'Crear agencias', 'Create agencies', 1),
    (14, 'AgencyEdit', 'Editar agencias', 'Edit agencies', 1),
    (15, 'AgencyDelete', 'Eliminar agencias', 'Delete agencies', 1),
    (16, 'AgencyApprove', 'Aprobar agencias', 'Approve agencies', 1),
    (17, 'AgencyAssignUser', 'Asignar usuarios a agencias', 'Assign users to agencies', 1),

    (18, 'ProgramView', 'Ver programas', 'View programs', 1),
    (19, 'ProgramCreate', 'Crear programas', 'Create programs', 1),
    (20, 'ProgramEdit', 'Editar programas', 'Edit programs', 1),
    (21, 'ProgramDelete', 'Eliminar programas', 'Delete programs', 1),
    (22, 'ProgramAssignAgency', 'Asignar agencias a programas', 'Assign agencies to programs', 1),

    (23, 'DocumentView', 'Ver documentos', 'View documents', 1),
    (24, 'DocumentUpload', 'Subir documentos', 'Upload documents', 1),
    (25, 'DocumentDelete', 'Eliminar documentos', 'Delete documents', 1),
    (26, 'DocumentDownload', 'Descargar documentos', 'Download documents', 1),

    (27, 'SchoolView', 'Ver escuelas', 'View schools', 1),
    (28, 'SchoolCreate', 'Crear escuelas', 'Create schools', 1),
    (28, 'SchoolEdit', 'Editar escuelas', 'Edit schools', 1),
    (29, 'SchoolDelete', 'Eliminar escuelas', 'Delete schools', 1),

    (30, 'ReportView', 'Ver reportes', 'View reports', 1),
    (31, 'DashboardView', 'Ver dashboard', 'View dashboard', 1),

    (32, 'CatalogView', 'Ver catálogos/configuraciones', 'View catalogs/configurations', 1),
    (33, 'CatalogEdit', 'Editar catálogos/configuraciones', 'Edit catalogs/configurations', 1),

    (34, 'AuditView', 'Ver auditoría', 'View audit', 1);

ALTER TABLE Permission ADD IsActive BIT NOT NULL DEFAULT 1;
ALTER TABLE Permission ADD DescriptionEn NVARCHAR(MAX);
ALTER TABLE Permission ADD Id VARCHAR(36) NOT NULL PRIMARY KEY;

-- Insertar permisos para empleados
INSERT INTO Permission
    (Name, Description, DescriptionEn, IsActive)
VALUES
    ('EmployeeView', 'Ver empleados', 'View employees', 1),
    ('EmployeeCreate', 'Crear empleados', 'Create employees', 1),
    ('EmployeeEdit', 'Editar empleados', 'Edit employees', 1),
    ('EmployeeDelete', 'Eliminar empleados', 'Delete employees', 1),
    ('EmployeeAssignAgency', 'Asignar agencias a empleados', 'Assign agencies to employees', 1),
    ('EmployeeAssignRole', 'Asignar roles a empleados', 'Assign roles to employees', 1),
    ('EmployeeAssignPermission', 'Asignar permisos a empleados', 'Assign permissions to employees', 1),
    ('EmployeeAssignSchool', 'Asignar escuelas a empleados', 'Assign schools to employees', 1),
    ('EmployeeAssignProgram', 'Asignar programas a empleados', 'Assign programs to employees', 1);

-- Insertar permisos para staff
INSERT INTO Permission
    (Name, Description, DescriptionEn, IsActive)
VALUES
    ('StaffView', 'Ver staff', 'View staff', 1),
    ('StaffCreate', 'Crear staff', 'Create staff', 1),
    ('StaffEdit', 'Editar staff', 'Edit staff', 1),
    ('StaffDelete', 'Eliminar staff', 'Delete staff', 1),
    ('StaffAssignAgency', 'Asignar agencias a staff', 'Assign agencies to staff', 1),
    ('StaffAssignRole', 'Asignar roles a staff', 'Assign roles to staff', 1),
    ('StaffAssignPermission', 'Asignar permisos a staff', 'Assign permissions to staff', 1),
    ('StaffAssignSchool', 'Asignar escuelas a staff', 'Assign schools to staff', 1),
    ('StaffAssignProgram', 'Asignar programas a staff', 'Assign programs to staff', 1);

-- Insertar permisos para tipos de staff
INSERT INTO Permission
    (Name, Description, DescriptionEn, IsActive)
VALUES
    ('StaffTypeView', 'Ver tipos de staff', 'View staff types', 1),
    ('StaffTypeCreate', 'Crear tipos de staff', 'Create staff types', 1),
    ('StaffTypeEdit', 'Editar tipos de staff', 'Edit staff types', 1),
    ('StaffTypeDelete', 'Eliminar tipos de staff', 'Delete staff types', 1);

-- Actualización de la descripción en inglés (DescriptionEn) para los permisos existentes
-- Se utiliza WHERE Name = '...' para asegurarse de actualizar únicamente el permiso específico identificado por su nombre único.
-- Esto evita modificar otros registros accidentalmente y garantiza que solo el permiso deseado reciba la actualización en la columna DescriptionEn.

UPDATE Permission SET DescriptionEn = 'View users' WHERE Id = 1;
UPDATE Permission SET DescriptionEn = 'Create users' WHERE Id = 2;
UPDATE Permission SET DescriptionEn = 'Edit users' WHERE Id = 3;
UPDATE Permission SET DescriptionEn = 'Delete users' WHERE Id = 4;
UPDATE Permission SET DescriptionEn = 'Assign roles to users' WHERE Id = 5;
UPDATE Permission SET DescriptionEn = 'Assign agencies to users' WHERE Id = 6;

UPDATE Permission SET DescriptionEn = 'View roles' WHERE Id = 7;
UPDATE Permission SET DescriptionEn = 'Create roles' WHERE Id = 8;
UPDATE Permission SET DescriptionEn = 'Edit roles' WHERE Id = 9;
UPDATE Permission SET DescriptionEn = 'Delete roles' WHERE Id = 10;
UPDATE Permission SET DescriptionEn = 'Assign permissions to roles' WHERE Id = 11;

UPDATE Permission SET DescriptionEn = 'View agencies' WHERE Id = 12;
UPDATE Permission SET DescriptionEn = 'Create agencies' WHERE Id = 13;
UPDATE Permission SET DescriptionEn = 'Edit agencies' WHERE Id = 14;
UPDATE Permission SET DescriptionEn = 'Delete agencies' WHERE Id = 15;
UPDATE Permission SET DescriptionEn = 'Approve agencies' WHERE Id = 16;
UPDATE Permission SET DescriptionEn = 'Assign users to agencies' WHERE Id = 17;

UPDATE Permission SET DescriptionEn = 'View programs' WHERE Id = 18;
UPDATE Permission SET DescriptionEn = 'Create programs' WHERE Id = 19;
UPDATE Permission SET DescriptionEn = 'Edit programs' WHERE Id = 20;
UPDATE Permission SET DescriptionEn = 'Delete programs' WHERE Id = 21;
UPDATE Permission SET DescriptionEn = 'Assign agencies to programs' WHERE Id = 22;

UPDATE Permission SET DescriptionEn = 'View documents' WHERE Id = 23;
UPDATE Permission SET DescriptionEn = 'Upload documents' WHERE Id = 24;
UPDATE Permission SET DescriptionEn = 'Delete documents' WHERE Id = 25;
UPDATE Permission SET DescriptionEn = 'Download documents' WHERE Id = 26;

UPDATE Permission SET DescriptionEn = 'View schools' WHERE Id = 27;
UPDATE Permission SET DescriptionEn = 'Create schools' WHERE Id = 28;
UPDATE Permission SET DescriptionEn = 'Edit schools' WHERE Id = 29;
UPDATE Permission SET DescriptionEn = 'Delete schools' WHERE Id = 30;

UPDATE Permission SET DescriptionEn = 'View reports' WHERE Id = 31;
UPDATE Permission SET DescriptionEn = 'View dashboard' WHERE Id = 32;

UPDATE Permission SET DescriptionEn = 'View catalogs/configurations' WHERE Id = 33;
UPDATE Permission SET DescriptionEn = 'Edit catalogs/configurations' WHERE Id = 34;

UPDATE Permission SET DescriptionEn = 'View audit' WHERE Id = 35;

