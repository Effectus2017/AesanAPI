-- Stored Procedure para completar operación masiva de auditoría
-- Versión: 1.0.0
CREATE OR ALTER PROCEDURE [102_CompleteBulkOperation]
    @OperationId UNIQUEIDENTIFIER,
    @TotalRecords INT,
    @SuccessfulRecords INT,
    @FailedRecords INT,
    @Status NVARCHAR(20) = 'COMPLETED'
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE AuditOperationSummary
        SET 
            CompletedAt = GETUTCDATE(),
            TotalRecords = @TotalRecords,
            SuccessfulRecords = @SuccessfulRecords,
            FailedRecords = @FailedRecords,
            Status = @Status
        WHERE OperationId = @OperationId;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO
