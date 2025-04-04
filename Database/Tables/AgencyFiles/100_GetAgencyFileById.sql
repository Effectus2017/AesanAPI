SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para obtener un archivo específico por su ID
-- =============================================
CREATE OR ALTER PROCEDURE [100_GetAgencyFileById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
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
        AF.UploadedBy,
        CONCAT(U.FirstName, ' ', U.FatherLastName, ' ', U.MotherLastName) AS UploadedByName
    FROM 
        AgencyFiles AF
    INNER JOIN 
        Agency A ON AF.AgencyId = A.Id
    LEFT JOIN
        AspNetUsers U ON AF.UploadedBy = U.Id
    WHERE 
        AF.Id = @id
        AND AF.IsActive = 1;
END;
GO 