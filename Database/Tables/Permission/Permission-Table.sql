-- Tabla para permisos
-- 1.0.0
CREATE TABLE Permission
(
    Id int NOT NULL IDENTITY(1,1),
    Name nvarchar(100) NOT NULL,
    Description nvarchar(255) NULL
);

-- Tabla para permisos de usuarios
-- 1.0.0
CREATE TABLE UserPermission
(
    UserId nvarchar(450) NOT NULL,
    PermissionId int NOT NULL
);

-- Tabla para listar los permisos que se pueden asignar al usuario al seleccionar un de roles
-- 1.0.0
CREATE TABLE RolePermission
(
    RoleId nvarchar(450) NOT NULL,
    PermissionId int NOT NULL
);