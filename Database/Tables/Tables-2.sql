-- Tabla para almacenar la relación entre usuarios y agencias
CREATE TABLE [dbo].[UserAgencyAssignment] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [AgencyId] INT NOT NULL,
    [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [AssignedBy] NVARCHAR(450) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_UserAgencyAssignment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAgencyAssignment_Agency] FOREIGN KEY ([AgencyId]) REFERENCES [dbo].[Agency] ([Id]),
    CONSTRAINT [UQ_UserAgencyAssignment] UNIQUE ([UserId], [AgencyId])
);

GO

CREATE INDEX [IX_UserAgencyAssignment_UserId] ON [dbo].[UserAgencyAssignment] ([UserId]);
CREATE INDEX [IX_UserAgencyAssignment_AgencyId] ON [dbo].[UserAgencyAssignment] ([AgencyId]); 