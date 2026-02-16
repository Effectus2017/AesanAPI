/*
===========================================
Tabla: UserSecondaryRoleHistory
===========================================
Registro histórico de roles secundarios asignados a usuarios.
Cada vez que un rol secundario termina (expira o se reemplaza) se inserta aquí.

Estructura:
- Id: PK
- UserId: usuario
- RoleId: rol
- ValidFrom, ValidTo: vigencia que tuvo
- CreatedAt: cuándo se asignó
- CreatedBy: quien asignó (opcional)
- EndedAt: cuándo dejó de estar activo (expiró o se quitó)
*/

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserSecondaryRoleHistory')
BEGIN
    CREATE TABLE [dbo].[UserSecondaryRoleHistory]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        [ValidFrom] DATE NOT NULL,
        [ValidTo] DATE NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(450) NULL,
        [EndedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_UserSecondaryRoleHistory_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
        CONSTRAINT [FK_UserSecondaryRoleHistory_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_UserSecondaryRoleHistory_UserId] ON [dbo].[UserSecondaryRoleHistory]([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserSecondaryRoleHistory_EndedAt] ON [dbo].[UserSecondaryRoleHistory]([EndedAt] DESC);
    PRINT 'Tabla UserSecondaryRoleHistory creada.';
END
GO
