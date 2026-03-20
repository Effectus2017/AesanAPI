-- =============================================
-- Tabla: SchoolStaff
-- Descripción: Asignaciones de personal (Staff) a escuelas (School)
-- Reemplaza: SiteStaff (asignación por sitio)
-- =============================================

CREATE TABLE [dbo].[SchoolStaff]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SchoolId] [int] NOT NULL,
    [StaffId] [int] NOT NULL,
    [AssignmentDate] [date] NOT NULL DEFAULT GETDATE(),
    [AssignmentTypeId] [int] NOT NULL DEFAULT 1,
    [IsPrimary] [bit] NOT NULL DEFAULT 0,
    [StartDate] [date] NULL,
    [EndDate] [date] NULL,
    [Comments] [nvarchar](500) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SchoolStaff] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE INDEX [IX_SchoolStaff_SchoolId] ON [SchoolStaff]([SchoolId]);
CREATE INDEX [IX_SchoolStaff_StaffId] ON [SchoolStaff]([StaffId]);
CREATE INDEX [IX_SchoolStaff_AssignmentTypeId] ON [SchoolStaff]([AssignmentTypeId]);
CREATE INDEX [IX_SchoolStaff_IsActive] ON [SchoolStaff]([IsActive]);
CREATE INDEX [IX_SchoolStaff_IsPrimary] ON [SchoolStaff]([IsPrimary]);

ALTER TABLE [SchoolStaff] ADD CONSTRAINT [FK_SchoolStaff_School] FOREIGN KEY([SchoolId]) REFERENCES [School]([Id]);
ALTER TABLE [SchoolStaff] ADD CONSTRAINT [FK_SchoolStaff_Staff] FOREIGN KEY([StaffId]) REFERENCES [Staff]([Id]);
ALTER TABLE [SchoolStaff] ADD CONSTRAINT [FK_SchoolStaff_AssignmentType] FOREIGN KEY([AssignmentTypeId]) REFERENCES [OptionSelection]([Id]);
