/*
===========================================
Tabla: UserRoleExtensionRequest
===========================================
Solicitudes de extensión de vigencia de un rol secundario.
El usuario solicita extender la fecha tope; un admin aprueba o rechaza.

Estructura:
- Id: PK
- UserId, RoleId: usuario y rol secundario
- RequestedValidTo: nueva fecha tope solicitada
- Reason: motivo opcional
- Status: Pending, Approved, Rejected
- RequestedAt, ProcessedAt, ProcessedBy
*/

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserRoleExtensionRequest')
BEGIN
    CREATE TABLE [dbo].[UserRoleExtensionRequest]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        [RequestedValidTo] DATE NOT NULL,
        [Reason] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [RequestedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [ProcessedAt] DATETIME2(7) NULL,
        [ProcessedBy] NVARCHAR(450) NULL,
        CONSTRAINT [FK_UserRoleExtensionRequest_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
        CONSTRAINT [FK_UserRoleExtensionRequest_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]),
        CONSTRAINT [CK_UserRoleExtensionRequest_Status] CHECK ([Status] IN ('Pending', 'Approved', 'Rejected'))
    );

    CREATE NONCLUSTERED INDEX [IX_UserRoleExtensionRequest_UserId] ON [dbo].[UserRoleExtensionRequest]([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserRoleExtensionRequest_Status] ON [dbo].[UserRoleExtensionRequest]([Status]);
    CREATE NONCLUSTERED INDEX [IX_UserRoleExtensionRequest_RequestedAt] ON [dbo].[UserRoleExtensionRequest]([RequestedAt] DESC);
    PRINT 'Tabla UserRoleExtensionRequest creada.';
END
GO
