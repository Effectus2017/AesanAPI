-- Stored Procedure para iniciar operación masiva de auditoría
-- Versión: 1.0.0
CREATE OR ALTER PROCEDURE [100_StartBulkOperation]
    @OperationType NVARCHAR(50),
    @TableName NVARCHAR(128),
    @ChangedBy NVARCHAR(450),
    -- REQUERIDO
    @Description NVARCHAR(500) = NULL,
    @SourceSystem NVARCHAR(100) = 'WebPortal',
    @FileName NVARCHAR(255) = NULL,
    @OperationId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que ChangedBy no esté vacío
    IF @ChangedBy IS NULL OR @ChangedBy = ''
    BEGIN
        RAISERROR('ChangedBy es requerido y no puede estar vacío', 16, 1);
        RETURN -1;
    END

    SET @OperationId = NEWID();

    BEGIN TRY
        INSERT INTO AuditOperationSummary
        (
        OperationId, OperationType, TableName, ChangedBy, StartedAt,
        Description, SourceSystem, FileName, Status
        )
    VALUES
        (
            @OperationId, @OperationType, @TableName, @ChangedBy, GETUTCDATE(),
            @Description, @SourceSystem, @FileName, 'IN_PROGRESS'
        );
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO
