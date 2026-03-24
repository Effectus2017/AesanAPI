-- =============================================
-- Tabla: VisitType — catálogo de tipos de visita (sponsor evaluation)
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
END
GO
