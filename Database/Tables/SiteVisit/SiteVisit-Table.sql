-- =============================================
-- Tabla: SiteVisit — visitas programadas por sitio
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SiteVisit]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[SiteVisit] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SiteId] INT NOT NULL,
        [VisitTypeId] INT NOT NULL,
        [VisitDate] DATE NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL,
        [Comments] NVARCHAR(1000) NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_SiteVisit_IsDeleted] DEFAULT (0),
        [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_SiteVisit_CreatedAt] DEFAULT (GETDATE()),
        [UpdatedAt] DATETIME NOT NULL CONSTRAINT [DF_SiteVisit_UpdatedAt] DEFAULT (GETDATE()),
        [CreatedBy] NVARCHAR(255) NULL,
        [UpdatedBy] NVARCHAR(255) NULL,
        CONSTRAINT [FK_SiteVisit_Site] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[Site]([Id]),
        CONSTRAINT [FK_SiteVisit_VisitType] FOREIGN KEY ([VisitTypeId]) REFERENCES [dbo].[VisitType]([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_SiteVisit_SiteId_VisitDate] ON [dbo].[SiteVisit]([SiteId], [VisitDate]) WHERE [IsDeleted] = 0;
END
GO
