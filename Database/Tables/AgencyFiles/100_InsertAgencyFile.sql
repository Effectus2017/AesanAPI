SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para insertar un nuevo archivo asociado a una agencia
-- =============================================
CREATE OR ALTER PROCEDURE [100_InsertAgencyFile]
    @agencyId INT,
    @fileName NVARCHAR(255),
    @storedFileName NVARCHAR(255),
    @fileUrl NVARCHAR(MAX),
    @contentType NVARCHAR(100),
    @fileSize BIGINT,
    @description NVARCHAR(500) = NULL,
    @documentType NVARCHAR(100) = NULL,
    @uploadedBy NVARCHAR(128) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO AgencyFiles (
        AgencyId,
        FileName,
        StoredFileName,
        FileUrl,
        ContentType,
        FileSize,
        Description,
        DocumentType,
        UploadDate,
        UploadedBy,
        IsActive
    )
    VALUES (
        @agencyId,
        @fileName,
        @storedFileName,
        @fileUrl,
        @contentType,
        @fileSize,
        @description,
        @documentType,
        GETDATE(),
        @uploadedBy,
        1
    );
    
    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO 