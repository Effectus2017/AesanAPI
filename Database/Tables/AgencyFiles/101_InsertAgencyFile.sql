SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para insertar un nuevo archivo asociado a una agencia
-- Versión 101: Agregada validación de existencia de agencia
-- =============================================
CREATE OR ALTER PROCEDURE [101_InsertAgencyFile]
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
    
    -- Validar que la agencia existe y está activa
    IF NOT EXISTS (SELECT 1 FROM Agency WHERE Id = @agencyId AND IsActive = 1)
    BEGIN
        RAISERROR ('La agencia especificada no existe o no está activa.', 16, 1);
        RETURN -1;
    END
    
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
        IsActive,
        IsDeleted
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
        1,  -- IsActive
        0   -- IsDeleted
    );
    
    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO 