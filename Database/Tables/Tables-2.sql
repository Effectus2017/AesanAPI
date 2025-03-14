-- Tabla para almacenar la relación entre usuarios y agencias
CREATE TABLE [dbo].[AgencyUserAssignment] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [AgencyId] INT NOT NULL,
    [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [AssignedBy] NVARCHAR(450) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AgencyUserAssignment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AgencyUserAssignment_Agency] FOREIGN KEY ([AgencyId]) REFERENCES [dbo].[Agency] ([Id]),
    CONSTRAINT [UQ_AgencyUserAssignment] UNIQUE ([UserId], [AgencyId])
);

GO

CREATE INDEX [IX_AgencyUserAssignment_UserId] ON [dbo].[AgencyUserAssignment] ([UserId]);
CREATE INDEX [IX_AgencyUserAssignment_AgencyId] ON [dbo].[AgencyUserAssignment] ([AgencyId]); 