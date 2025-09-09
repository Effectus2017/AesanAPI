INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive)
VALUES
    (NEWID(), 'user.view', 'Ver usuarios', 'View users', 1),
    (NEWID(), 'user.create', 'Crear usuarios', 'Create users', 1),
    (NEWID(), 'user.edit', 'Editar usuarios', 'Edit users', 1),
    (NEWID(), 'user.delete', 'Eliminar usuarios', 'Delete users', 1),
    (NEWID(), 'user.assign.role', 'Asignar roles a usuarios', 'Assign roles to users', 1),
    (NEWID(), 'user.assign.agency', 'Asignar agencias a usuarios', 'Assign agencies to users', 1),

    (NEWID(), 'role.view', 'Ver roles', 'View roles', 1),
    (NEWID(), 'role.create', 'Crear roles', 'Create roles', 1),
    (NEWID(), 'role.edit', 'Editar roles', 'Edit roles', 1),
    (NEWID(), 'role.delete', 'Eliminar roles', 'Delete roles', 1),
    (NEWID(), 'role.assign.permission', 'Asignar permisos a roles', 'Assign permissions to roles', 1),

    (NEWID(), 'agency.view', 'Ver agencias', 'View agencies', 1),
    (NEWID(), 'agency.create', 'Crear agencias', 'Create agencies', 1),
    (NEWID(), 'agency.edit', 'Editar agencias', 'Edit agencies', 1),
    (NEWID(), 'agency.delete', 'Eliminar agencias', 'Delete agencies', 1),
    (NEWID(), 'agency.approve', 'Aprobar agencias', 'Approve agencies', 1),
    (NEWID(), 'agency.assign.user', 'Asignar usuarios a agencias', 'Assign users to agencies', 1),

    (NEWID(), 'program.view', 'Ver programas', 'View programs', 1),
    (NEWID(), 'program.create', 'Crear programas', 'Create programs', 1),
    (NEWID(), 'program.edit', 'Editar programas', 'Edit programs', 1),
    (NEWID(), 'program.delete', 'Eliminar programas', 'Delete programs', 1),
    (NEWID(), 'program.assign.agency', 'Asignar agencias a programas', 'Assign agencies to programs', 1),

    (NEWID(), 'document.view', 'Ver documentos', 'View documents', 1),
    (NEWID(), 'document.upload', 'Subir documentos', 'Upload documents', 1),
    (NEWID(), 'document.delete', 'Eliminar documentos', 'Delete documents', 1),
    (NEWID(), 'document.download', 'Descargar documentos', 'Download documents', 1),

    (NEWID(), 'school.view', 'Ver escuelas', 'View schools', 1),
    (NEWID(), 'school.create', 'Crear escuelas', 'Create schools', 1),
    (NEWID(), 'school.edit', 'Editar escuelas', 'Edit schools', 1),
    (NEWID(), 'school.delete', 'Eliminar escuelas', 'Delete schools', 1),

    (NEWID(), 'report.view', 'Ver reportes', 'View reports', 1),
    (NEWID(), 'dashboard.view', 'Ver dashboard', 'View dashboard', 1),

    (NEWID(), 'catalog.view', 'Ver catálogos/configuraciones', 'View catalogs/configurations', 1),
    (NEWID(), 'catalog.edit', 'Editar catálogos/configuraciones', 'Edit catalogs/configurations', 1),

    (NEWID(), 'audit.view', 'Ver auditoría', 'View audit', 1);

-- Insertar permisos para empleados
INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive)
VALUES
    (NEWID(), 'employee.view', 'Ver empleados', 'View employees', 1),
    (NEWID(), 'employee.create', 'Crear empleados', 'Create employees', 1),
    (NEWID(), 'employee.edit', 'Editar empleados', 'Edit employees', 1),
    (NEWID(), 'employee.delete', 'Eliminar empleados', 'Delete employees', 1),
    (NEWID(), 'employee.assign.agency', 'Asignar agencias a empleados', 'Assign agencies to employees', 1),
    (NEWID(), 'employee.assign.role', 'Asignar roles a empleados', 'Assign roles to employees', 1),
    (NEWID(), 'employee.assign.permission', 'Asignar permisos a empleados', 'Assign permissions to employees', 1),
    (NEWID(), 'employee.assign.school', 'Asignar escuelas a empleados', 'Assign schools to employees', 1),
    (NEWID(), 'employee.assign.program', 'Asignar programas a empleados', 'Assign programs to employees', 1);

-- Insertar permisos para staff
INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive)
VALUES
    (NEWID(), 'staff.view', 'Ver staff', 'View staff', 1),
    (NEWID(), 'staff.create', 'Crear staff', 'Create staff', 1),
    (NEWID(), 'staff.edit', 'Editar staff', 'Edit staff', 1),
    (NEWID(), 'staff.delete', 'Eliminar staff', 'Delete staff', 1),
    (NEWID(), 'staff.assign.agency', 'Asignar agencias a staff', 'Assign agencies to staff', 1),
    (NEWID(), 'staff.assign.role', 'Asignar roles a staff', 'Assign roles to staff', 1),
    (NEWID(), 'staff.assign.permission', 'Asignar permisos a staff', 'Assign permissions to staff', 1),
    (NEWID(), 'staff.assign.school', 'Asignar escuelas a staff', 'Assign schools to staff', 1),
    (NEWID(), 'staff.assign.program', 'Asignar programas a staff', 'Assign programs to staff', 1);

-- Insertar permisos para tipos de staff
INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive)
VALUES
    (NEWID(), 'staff.type.view', 'Ver tipos de staff', 'View staff types', 1),
    (NEWID(), 'staff.type.create', 'Crear tipos de staff', 'Create staff types', 1),
    (NEWID(), 'staff.type.edit', 'Editar tipos de staff', 'Edit staff types', 1),
    (NEWID(), 'staff.type.delete', 'Eliminar tipos de staff', 'Delete staff types', 1);

