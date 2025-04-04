SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para actualizar la información de un archivo
-- =============================================
CREATE OR ALTER PROCEDURE [100_UpdateAgencyFile]
    @id INT,
    @description NVARCHAR(500) = NULL,
    @documentType NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;
    
    -- Actualizar la información del archivo
    -- Solo se permite actualizar la descripción y el tipo de documento
    UPDATE AgencyFiles
    SET 
        Description = @description,
        DocumentType = @documentType
    WHERE 
        Id = @id
        AND IsActive = 1;
    
    SELECT @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 