-- Tabla para permisos
-- 2.0.0 - Estructura mejorada con ValueKey y NameEn
CREATE TABLE Permission
(
    Id VARCHAR(36) NOT NULL PRIMARY KEY,
    ValueKey VARCHAR(50) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    NameEn VARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- Tabla para permisos de usuarios
-- 2.0.0 - Actualizada para usar PermissionId como VARCHAR(36)
CREATE TABLE UserPermission
(
    UserId NVARCHAR(450) NOT NULL,
    PermissionId VARCHAR(36) NOT NULL,
    CONSTRAINT FK_UserPermission_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_UserPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(Id),
    CONSTRAINT PK_UserPermission PRIMARY KEY (UserId, PermissionId)
);

-- Tabla para listar los permisos que se pueden asignar al usuario al seleccionar un rol
-- 2.0.0 - Actualizada para usar PermissionId como VARCHAR(36)
CREATE TABLE RolePermission
(
    RoleId NVARCHAR(450) NOT NULL,
    PermissionId VARCHAR(36) NOT NULL,
    CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id),
    CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(Id),
    CONSTRAINT PK_RolePermission PRIMARY KEY (RoleId, PermissionId)
);

-- Índices para mejorar rendimiento
CREATE INDEX IX_Permission_ValueKey ON Permission(ValueKey);
CREATE INDEX IX_Permission_IsActive ON Permission(IsActive);
CREATE INDEX IX_UserPermission_UserId ON UserPermission(UserId);
CREATE INDEX IX_UserPermission_PermissionId ON UserPermission(PermissionId);
CREATE INDEX IX_RolePermission_RoleId ON RolePermission(RoleId);
CREATE INDEX IX_RolePermission_PermissionId ON RolePermission(PermissionId);