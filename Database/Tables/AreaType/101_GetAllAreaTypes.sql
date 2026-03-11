-- =============================================
-- Stored Procedure: 100_GetAllAreaTypes
-- Versión: 1.1 (sobre 100_GetAllAreaTypes)
-- Remueve @isList; SP siempre devuelve datos + TotalCount (2 result sets).
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllAreaTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(100) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, NameEN, IsActive, DisplayOrder
    FROM AreaType
    WHERE (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM AreaType
    WHERE (@name IS NULL OR Name LIKE '%' + @name + '%');
END
