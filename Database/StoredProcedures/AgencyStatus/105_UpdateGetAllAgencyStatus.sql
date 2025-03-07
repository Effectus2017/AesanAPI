-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para actualizar el procedimiento almacenado que obtiene todos los estados de agencia, ordenándolos por DisplayOrder
-- =============================================

-- Actualizamos el procedimiento almacenado para obtener todos los estados de agencia
CREATE OR ALTER PROCEDURE [dbo].[105_GetAllAgencyStatus]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, IsActive, DisplayOrder
    FROM AgencyStatus
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND IsActive = 1
    )
    ORDER BY DisplayOrder -- Ordenamos por DisplayOrder en lugar de Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM AgencyStatus
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND IsActive = 1
    );
END; 