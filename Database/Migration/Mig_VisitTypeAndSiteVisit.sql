-- =============================================
-- Migration: VisitType + SiteVisit + seed tipos de visita
-- Fecha: 2026-03-24
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VisitType]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[VisitType] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Code] NVARCHAR(50) NOT NULL,
        [NameEs] NVARCHAR(200) NOT NULL,
        [NameEN] NVARCHAR(200) NOT NULL,
        [SortOrder] INT NOT NULL CONSTRAINT [DF_VisitType_SortOrder] DEFAULT (0),
        [RuleMaxWeeksFromProgramStart] INT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_VisitType_IsActive] DEFAULT (1),
        CONSTRAINT [UQ_VisitType_Code] UNIQUE ([Code])
    );
    PRINT 'Tabla VisitType creada.';
END
GO

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
    PRINT 'Tabla SiteVisit creada.';
END
GO

MERGE [dbo].[VisitType] AS tgt
USING (
    SELECT * FROM (VALUES
        (N'INITIAL', N'Visita Inicial', N'Initial Visit', 1, NULL),
        (N'SPECIALIST_4W', N'Especialista (Primeras 4 semanas)', N'Specialist (First 4 weeks)', 2, 4),
        (N'REVIEW_2W', N'Revisión (Primeras 2 semanas)', N'Review (First 2 weeks)', 3, 2),
        (N'FOLLOWUP', N'Seguimiento', N'Follow-up', 4, NULL)
    ) AS v(Code, NameEs, NameEN, SortOrder, RuleMaxWeeksFromProgramStart)
) AS src
ON tgt.[Code] = src.Code
WHEN NOT MATCHED THEN
    INSERT ([Code], [NameEs], [NameEN], [SortOrder], [RuleMaxWeeksFromProgramStart], [IsActive])
    VALUES (src.Code, src.NameEs, src.NameEN, src.SortOrder, src.RuleMaxWeeksFromProgramStart, 1)
WHEN MATCHED THEN
    UPDATE SET
        [NameEs] = src.NameEs,
        [NameEN] = src.NameEN,
        [SortOrder] = src.SortOrder,
        [RuleMaxWeeksFromProgramStart] = src.RuleMaxWeeksFromProgramStart;
GO

PRINT 'Seed VisitType completado (MERGE). Ejecutar scripts 100_GetVisitTypes, 100_GetSiteVisits, 100_InsertSiteVisit, 100_UpdateSiteVisit, 100_DeleteSiteVisit si aún no están en BD.';
GO
