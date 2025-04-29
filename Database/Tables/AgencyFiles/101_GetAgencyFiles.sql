SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para obtener los archivos asociados a una agencia
-- Versión 101: Agregado filtro para archivos eliminados
-- =============================================
CREATE OR ALTER PROCEDURE [101_GetAgencyFiles]
    @agencyId INT,
    @take INT = 10,
    @skip INT = 0,
    @alls BIT = 0,
    @documentType NVARCHAR(100) = NULL,
    @name NVARCHAR(100) = NULL,
    @includeDeleted BIT = 0
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
        AF.IsDeleted,
        AF.IsVerified,
        AF.ExpirationDate,
        AF.IsActive,
        -- Obtener el nombre del usuario que subió el archivo
        CONCAT(U.FirstName, ' ', U.FatherLastName, ' ', U.MotherLastName) AS UploadedByName
    FROM 
        AgencyFiles AF
    INNER JOIN 
        Agency A ON AF.AgencyId = A.Id
    LEFT JOIN
        AspNetUsers U ON AF.UploadedBy = U.Id
    WHERE 
        (@includeDeleted = 1 OR AF.IsDeleted = 0)
        AND (
            (@alls = 1)
            OR (
                AF.AgencyId = @agencyId
                AND AF.IsActive = 1
                AND (@documentType IS NULL OR AF.DocumentType = @documentType)
                AND (@name IS NULL OR AF.FileName LIKE '%' + @name + '%')
            )
        )
    ORDER BY 
        AF.UploadDate DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
    
    -- Contar el total de archivos para la paginación
    SELECT COUNT(*) 
    FROM 
        AgencyFiles AF
    WHERE 
        (@includeDeleted = 1 OR AF.IsDeleted = 0)
        AND (
            (@alls = 1)
            OR (
                AF.AgencyId = @agencyId
                AND AF.IsActive = 1
                AND (@documentType IS NULL OR AF.DocumentType = @documentType)
                AND (@name IS NULL OR AF.FileName LIKE '%' + @name + '%')
            )
        );
END;
GO 