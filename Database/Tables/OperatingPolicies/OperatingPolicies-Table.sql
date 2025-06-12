CREATE TABLE [dbo].[OperatingPolicies]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [NameEN] NVARCHAR(255) NOT NULL,
    [IsActive] BIT NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedBy] NVARCHAR(255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id])
);

-- Política de Funcionamiento 1 Gratis
-- Política de Funcionamiento 2 Pagando
-- Política de Funcionamiento 3 Provisión I
-- Política de Funcionamiento 4 Provisión 2
-- Política de Funcionamiento 5 Provisión 3

INSERT INTO [dbo].[OperatingPolicies]
    ([Name], [NameEN], [IsActive], [DisplayOrder], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy])
VALUES
    ('Gratis', 'Free', 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
    ('Pagando', 'Paying', 1, 2, GETDATE(), GETDATE(), 'System', 'System'),
    ('Provisión I', 'Provision I', 1, 3, GETDATE(), GETDATE(), 'System', 'System'),
    ('Provisión 2', 'Provision 2', 1, 4, GETDATE(), GETDATE(), 'System', 'System'),
    ('Provisión 3', 'Provision 3', 1, 5, GETDATE(), GETDATE(), 'System', 'System');