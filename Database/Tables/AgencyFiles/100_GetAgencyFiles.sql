SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para obtener los archivos asociados a una agencia
-- =============================================
CREATE OR ALTER PROCEDURE [100_GetAgencyFiles]
    @agencyId INT,
    @take INT = 10,
    @skip INT = 0,
    @documentType NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Seleccionar los archivos de la agencia, con paginación y filtro por tipo de documento opcional
    SELECT 
        AF.Id,
        AF.AgencyId,
        A.Name AS AgencyName,
        AF.FileName,
        AF.StoredFileName,
        AF.FileUrl,
        AF.ContentType,
        AF.FileSize,
        AF.Description,
        AF.DocumentType,
        AF.UploadDate,
        -- Obtener el nombre del usuario que subió el archivo
        CONCAT(U.FirstName, ' ', U.FatherLastName, ' ', U.MotherLastName) AS UploadedByName
    FROM 
        AgencyFiles AF
    INNER JOIN 
        Agency A ON AF.AgencyId = A.Id
    LEFT JOIN
        AspNetUsers U ON AF.UploadedBy = U.Id
    WHERE 
        AF.AgencyId = @agencyId
        AND AF.IsActive = 1
        AND (@documentType IS NULL OR AF.DocumentType = @documentType)
    ORDER BY 
        AF.UploadDate DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
    
    -- Contar el total de archivos para la paginación
    SELECT COUNT(*) 
    FROM 
        AgencyFiles
    WHERE 
        AgencyId = @agencyId
        AND IsActive = 1
        AND (@documentType IS NULL OR DocumentType = @documentType);
END;
GO 