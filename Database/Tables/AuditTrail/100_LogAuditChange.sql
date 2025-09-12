-- Stored Procedure para registrar cambios individuales en la auditoría
-- Versión: 1.0.0
CREATE OR ALTER PROCEDURE [100_LogAuditChange]
    @TableName NVARCHAR(128),
    @EntityId NVARCHAR(100),
    @Action NVARCHAR(20),
    @ChangedBy NVARCHAR(450),
    -- REQUERIDO
    @OldValues NVARCHAR(MAX) = NULL,
    @NewValues NVARCHAR(MAX) = NULL,
    @ChangedFields NVARCHAR(MAX) = NULL,
    @Reason NVARCHAR(500) = NULL,
    @BusinessContext NVARCHAR(200) = NULL,
    @ParentOperationId UNIQUEIDENTIFIER = NULL,
    @IPAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @SessionId NVARCHAR(128) = NULL,
    @RequestId NVARCHAR(100) = NULL,
    @Metadata NVARCHAR(MAX) = NULL,
    @Tags NVARCHAR(500) = NULL,
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

    -- Generar OperationId si no se proporciona
    IF @OperationId IS NULL
        SET @OperationId = NEWID();

    BEGIN TRY
        INSERT INTO AuditTrail
        (
        OperationId, TableName, EntityId, Action, ChangedBy, ChangedAt,
        OldValues, NewValues, ChangedFields, Reason, BusinessContext,
        ParentOperationId, IPAddress, UserAgent, SessionId, RequestId,
        Metadata, Tags
        )
    VALUES
        (
            @OperationId, @TableName, @EntityId, @Action, @ChangedBy, GETUTCDATE(),
            @OldValues, @NewValues, @ChangedFields, @Reason, @BusinessContext,
            @ParentOperationId, @IPAddress, @UserAgent, @SessionId, @RequestId,
            @Metadata, @Tags
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
