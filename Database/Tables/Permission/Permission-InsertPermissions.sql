INSERT INTO Permission
    (Name, Description, DescriptionEn, IsActive)
VALUES
    ('UserView', 'Ver usuarios', 'View users', 1),
    ('UserCreate', 'Crear usuarios', 'Create users', 1),
    ('UserEdit', 'Editar usuarios', 'Edit users', 1),
    ('UserDelete', 'Eliminar usuarios', 'Delete users', 1),
    ('UserAssignRole', 'Asignar roles a usuarios', 'Assign roles to users', 1),
    ('UserAssignAgency', 'Asignar agencias a usuarios', 'Assign agencies to users', 1),

    ('RoleView', 'Ver roles', 'View roles', 1),
    ('RoleCreate', 'Crear roles', 'Create roles', 1),
    ('RoleEdit', 'Editar roles', 'Edit roles', 1),
    ('RoleDelete', 'Eliminar roles', 'Delete roles', 1),
    ('RoleAssignPermission', 'Asignar permisos a roles', 'Assign permissions to roles', 1),

    ('AgencyView', 'Ver agencias', 'View agencies', 1),
    ('AgencyCreate', 'Crear agencias', 'Create agencies', 1),
    ('AgencyEdit', 'Editar agencias', 'Edit agencies', 1),
    ('AgencyDelete', 'Eliminar agencias', 'Delete agencies', 1),
    ('AgencyApprove', 'Aprobar agencias', 'Approve agencies', 1),
    ('AgencyAssignUser', 'Asignar usuarios a agencias', 'Assign users to agencies', 1),

    ('ProgramView', 'Ver programas', 'View programs', 1),
    ('ProgramCreate', 'Crear programas', 'Create programs', 1),
    ('ProgramEdit', 'Editar programas', 'Edit programs', 1),
    ('ProgramDelete', 'Eliminar programas', 'Delete programs', 1),
    ('ProgramAssignAgency', 'Asignar agencias a programas', 'Assign agencies to programs', 1),

    ('DocumentView', 'Ver documentos', 'View documents', 1),
    ('DocumentUpload', 'Subir documentos', 'Upload documents', 1),
    ('DocumentDelete', 'Eliminar documentos', 'Delete documents', 1),
    ('DocumentDownload', 'Descargar documentos', 'Download documents', 1),

    ('SchoolView', 'Ver escuelas', 'View schools', 1),
    ('SchoolCreate', 'Crear escuelas', 'Create schools', 1),
    ('SchoolEdit', 'Editar escuelas', 'Edit schools', 1),
    ('SchoolDelete', 'Eliminar escuelas', 'Delete schools', 1),

    ('ReportView', 'Ver reportes', 'View reports', 1),
    ('DashboardView', 'Ver dashboard', 'View dashboard', 1),

    ('CatalogView', 'Ver catálogos/configuraciones', 'View catalogs/configurations', 1),
    ('CatalogEdit', 'Editar catálogos/configuraciones', 'Edit catalogs/configurations', 1),

    ('AuditView', 'Ver auditoría', 'View audit', 1);

ALTER TABLE Permission ADD IsActive BIT NOT NULL DEFAULT 1;
ALTER TABLE Permission ADD DescriptionEn NVARCHAR(MAX);
ALTER TABLE Permission ADD Id VARCHAR(36) NOT NULL PRIMARY KEY;

-- Insertar permisos para empleados
INSERT INTO Permission
    (Id, Name, Description, DescriptionEn, IsActive)
VALUES
    (UUID(), 'EmployeeView', 'Ver empleados', 'View employees', 1),
    (UUID(), 'EmployeeCreate', 'Crear empleados', 'Create employees', 1),
    (UUID(), 'EmployeeEdit', 'Editar empleados', 'Edit employees', 1),
    (UUID(), 'EmployeeDelete', 'Eliminar empleados', 'Delete employees', 1),
    (UUID(), 'EmployeeAssignAgency', 'Asignar agencias a empleados', 'Assign agencies to employees', 1),
    (UUID(), 'EmployeeAssignRole', 'Asignar roles a empleados', 'Assign roles to employees', 1),
    (UUID(), 'EmployeeAssignPermission', 'Asignar permisos a empleados', 'Assign permissions to employees', 1),
    (UUID(), 'EmployeeAssignSchool', 'Asignar escuelas a empleados', 'Assign schools to employees', 1),
    (UUID(), 'EmployeeAssignProgram', 'Asignar programas a empleados', 'Assign programs to employees', 1);

-- Actualizar permisos los demas permisos sin descripcion en ingles
-- Actualización de la descripción en inglés (DescriptionEn) para los permisos existentes
-- Se utiliza WHERE Name = '...' para asegurarse de actualizar únicamente el permiso específico identificado por su nombre único.
-- Esto evita modificar otros registros accidentalmente y garantiza que solo el permiso deseado reciba la actualización en la columna DescriptionEn.

UPDATE Permission SET DescriptionEn = 'View users', Id = UUID() WHERE Name = 'UserView';
UPDATE Permission SET DescriptionEn = 'Create users', Id = UUID() WHERE Name = 'UserCreate';
UPDATE Permission SET DescriptionEn = 'Edit users', Id = UUID() WHERE Name = 'UserEdit';
UPDATE Permission SET DescriptionEn = 'Delete users', Id = UUID() WHERE Name = 'UserDelete';
UPDATE Permission SET DescriptionEn = 'Assign roles to users', Id = UUID() WHERE Name = 'UserAssignRole';
UPDATE Permission SET DescriptionEn = 'Assign agencies to users', Id = UUID() WHERE Name = 'UserAssignAgency';

UPDATE Permission SET DescriptionEn = 'View roles', Id = UUID() WHERE Name = 'RoleView';
UPDATE Permission SET DescriptionEn = 'Create roles', Id = UUID() WHERE Name = 'RoleCreate';
UPDATE Permission SET DescriptionEn = 'Edit roles', Id = UUID() WHERE Name = 'RoleEdit';
UPDATE Permission SET DescriptionEn = 'Delete roles', Id = UUID() WHERE Name = 'RoleDelete';
UPDATE Permission SET DescriptionEn = 'Assign permissions to roles', Id = UUID() WHERE Name = 'RoleAssignPermission';

UPDATE Permission SET DescriptionEn = 'View agencies', Id = UUID() WHERE Name = 'AgencyView';
UPDATE Permission SET DescriptionEn = 'Create agencies', Id = UUID() WHERE Name = 'AgencyCreate';
UPDATE Permission SET DescriptionEn = 'Edit agencies', Id = UUID() WHERE Name = 'AgencyEdit';
UPDATE Permission SET DescriptionEn = 'Delete agencies', Id = UUID() WHERE Name = 'AgencyDelete';
UPDATE Permission SET DescriptionEn = 'Approve agencies', Id = UUID() WHERE Name = 'AgencyApprove';
UPDATE Permission SET DescriptionEn = 'Assign users to agencies', Id = UUID() WHERE Name = 'AgencyAssignUser';

UPDATE Permission SET DescriptionEn = 'View programs', Id = UUID() WHERE Name = 'ProgramView';
UPDATE Permission SET DescriptionEn = 'Create programs', Id = UUID() WHERE Name = 'ProgramCreate';
UPDATE Permission SET DescriptionEn = 'Edit programs', Id = UUID() WHERE Name = 'ProgramEdit';
UPDATE Permission SET DescriptionEn = 'Delete programs', Id = UUID() WHERE Name = 'ProgramDelete';
UPDATE Permission SET DescriptionEn = 'Assign agencies to programs', Id = UUID() WHERE Name = 'ProgramAssignAgency';

UPDATE Permission SET DescriptionEn = 'View documents', Id = UUID() WHERE Name = 'DocumentView';
UPDATE Permission SET DescriptionEn = 'Upload documents', Id = UUID() WHERE Name = 'DocumentUpload';
UPDATE Permission SET DescriptionEn = 'Delete documents', Id = UUID() WHERE Name = 'DocumentDelete';
UPDATE Permission SET DescriptionEn = 'Download documents', Id = UUID() WHERE Name = 'DocumentDownload';

UPDATE Permission SET DescriptionEn = 'View schools', Id = UUID() WHERE Name = 'SchoolView';
UPDATE Permission SET DescriptionEn = 'Create schools', Id = UUID() WHERE Name = 'SchoolCreate';
UPDATE Permission SET DescriptionEn = 'Edit schools', Id = UUID() WHERE Name = 'SchoolEdit';
UPDATE Permission SET DescriptionEn = 'Delete schools', Id = UUID() WHERE Name = 'SchoolDelete';

UPDATE Permission SET DescriptionEn = 'View reports', Id = UUID() WHERE Name = 'ReportView';
UPDATE Permission SET DescriptionEn = 'View dashboard', Id = UUID() WHERE Name = 'DashboardView';

UPDATE Permission SET DescriptionEn = 'View catalogs/configurations', Id = UUID() WHERE Name = 'CatalogView';
UPDATE Permission SET DescriptionEn = 'Edit catalogs/configurations', Id = UUID() WHERE Name = 'CatalogEdit';

UPDATE Permission SET DescriptionEn = 'View audit', Id = UUID() WHERE Name = 'AuditView';

