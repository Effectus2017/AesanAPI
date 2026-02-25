-- Actualiza el estado de una agencia, registra en AgencyStatusHistory y en AuditTrail.
CREATE OR ALTER PROCEDURE [dbo].[104_UpdateAgencyStatus]
    @agencyId INT,
    @statusId INT,
    @changedBy NVARCHAR(450),
    @rejectionJustification NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;
    DECLARE @validStatus BIT = 0;
    DECLARE @auditOpId UNIQUEIDENTIFIER = NULL;

    IF @changedBy IS NULL OR LTRIM(RTRIM(@changedBy)) = ''
    BEGIN
        RAISERROR('changedBy es requerido', 16, 1);
        RETURN -1;
    END

    IF EXISTS (SELECT 1 FROM AgencyStatus WHERE Id = @statusId)
        SET @validStatus = 1;

    IF @validStatus = 1
    BEGIN
        UPDATE Agency
        SET AgencyStatusId = @statusId,
            UpdatedAt = GETDATE()
        WHERE Id = @agencyId;

        SET @rowsAffected = @@ROWCOUNT;

        IF @rowsAffected > 0
        BEGIN
            INSERT INTO AgencyStatusHistory (AgencyId, StatusId, ChangedBy, ChangedAt, Justification)
            VALUES (@agencyId, @statusId, @changedBy, GETUTCDATE(), @rejectionJustification);

            DECLARE @entityIdStr NVARCHAR(100) = CAST(@agencyId AS NVARCHAR(100));
            DECLARE @newValuesStr NVARCHAR(20) = CAST(@statusId AS NVARCHAR(20));
            EXEC [dbo].[100_LogAuditChange]
                @TableName = 'Agency',
                @EntityId = @entityIdStr,
                @Action = 'UPDATE',
                @ChangedBy = @changedBy,
                @ChangedFields = 'AgencyStatusId',
                @NewValues = @newValuesStr,
                @BusinessContext = 'AgencyStatusChange',
                @OperationId = @auditOpId OUTPUT;
        END

        RETURN @rowsAffected;
    END

    RETURN 0;
END;
GO 