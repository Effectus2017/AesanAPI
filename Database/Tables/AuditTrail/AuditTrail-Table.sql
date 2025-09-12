-- Tabla principal de auditoría para el sistema AESAN
-- Registra todos los cambios realizados en las operaciones CRUD

CREATE TABLE AuditTrail
(
    Id BIGINT PRIMARY KEY IDENTITY(1,1),

    -- Identificación de la operación
    OperationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    TableName NVARCHAR(128) NOT NULL,
    EntityId NVARCHAR(100) NOT NULL,

    -- Información del cambio
    Action NVARCHAR(20) NOT NULL,
    -- INSERT, UPDATE, DELETE, BULK_INSERT, BULK_UPDATE, BULK_DELETE
    ChangedBy NVARCHAR(450) NOT NULL,
    -- SIEMPRE requerido
    ChangedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),

    -- Datos del cambio
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    ChangedFields NVARCHAR(MAX) NULL,

    -- Contexto de la operación
    Reason NVARCHAR(500) NULL,
    BusinessContext NVARCHAR(200) NULL,
    ParentOperationId UNIQUEIDENTIFIER NULL,

    -- Información técnica
    IPAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    SessionId NVARCHAR(128) NULL,
    RequestId NVARCHAR(100) NULL,

    -- Metadatos adicionales
    Metadata NVARCHAR(MAX) NULL,
    Tags NVARCHAR(500) NULL,

    -- Índices para consultas rápidas
    INDEX IX_AuditTrail_TableName_EntityId (TableName, EntityId),
    INDEX IX_AuditTrail_OperationId (OperationId),
    INDEX IX_AuditTrail_ChangedBy (ChangedBy),
    INDEX IX_AuditTrail_ChangedAt (ChangedAt),
    INDEX IX_AuditTrail_Action (Action),
    INDEX IX_AuditTrail_BusinessContext (BusinessContext)
);

-- Tabla de resumen de operaciones masivas
CREATE TABLE AuditOperationSummary
(
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    OperationId UNIQUEIDENTIFIER NOT NULL,

    OperationType NVARCHAR(50) NOT NULL,
    TableName NVARCHAR(128) NOT NULL,
    ChangedBy NVARCHAR(450) NOT NULL,
    -- SIEMPRE requerido
    StartedAt DATETIME2(7) NOT NULL,
    CompletedAt DATETIME2(7) NULL,

    TotalRecords INT NOT NULL DEFAULT 0,
    SuccessfulRecords INT NOT NULL DEFAULT 0,
    FailedRecords INT NOT NULL DEFAULT 0,

    Description NVARCHAR(500) NULL,
    SourceSystem NVARCHAR(100) NULL,
    FileName NVARCHAR(255) NULL,

    Status NVARCHAR(20) NOT NULL DEFAULT 'IN_PROGRESS',

    INDEX IX_AuditOperationSummary_OperationId (OperationId),
    INDEX IX_AuditOperationSummary_ChangedBy (ChangedBy),
    INDEX IX_AuditOperationSummary_StartedAt (StartedAt),
    INDEX IX_AuditOperationSummary_Status (Status)
);
