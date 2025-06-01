INSERT INTO Permission
    (Name, Description)
VALUES
    ('UserView', 'Ver usuarios'),
    ('UserCreate', 'Crear usuarios'),
    ('UserEdit', 'Editar usuarios'),
    ('UserDelete', 'Eliminar usuarios'),
    ('UserAssignRole', 'Asignar roles a usuarios'),
    ('UserAssignAgency', 'Asignar agencias a usuarios'),

    ('RoleView', 'Ver roles'),
    ('RoleCreate', 'Crear roles'),
    ('RoleEdit', 'Editar roles'),
    ('RoleDelete', 'Eliminar roles'),
    ('RoleAssignPermission', 'Asignar permisos a roles'),

    ('AgencyView', 'Ver agencias'),
    ('AgencyCreate', 'Crear agencias'),
    ('AgencyEdit', 'Editar agencias'),
    ('AgencyDelete', 'Eliminar agencias'),
    ('AgencyApprove', 'Aprobar agencias'),
    ('AgencyAssignUser', 'Asignar usuarios a agencias'),

    ('ProgramView', 'Ver programas'),
    ('ProgramCreate', 'Crear programas'),
    ('ProgramEdit', 'Editar programas'),
    ('ProgramDelete', 'Eliminar programas'),
    ('ProgramAssignAgency', 'Asignar agencias a programas'),

    ('DocumentView', 'Ver documentos'),
    ('DocumentUpload', 'Subir documentos'),
    ('DocumentDelete', 'Eliminar documentos'),
    ('DocumentDownload', 'Descargar documentos'),

    ('SchoolView', 'Ver escuelas'),
    ('SchoolCreate', 'Crear escuelas'),
    ('SchoolEdit', 'Editar escuelas'),
    ('SchoolDelete', 'Eliminar escuelas'),

    ('ReportView', 'Ver reportes'),
    ('DashboardView', 'Ver dashboard'),

    ('CatalogView', 'Ver catálogos/configuraciones'),
    ('CatalogEdit', 'Editar catálogos/configuraciones'),

    ('AuditView', 'Ver auditoría'); 

