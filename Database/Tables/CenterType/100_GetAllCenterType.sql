-- 100_GetAllCenterType
-- Obtiene todos los tipos de centro
CREATE OR ALTER PROCEDURE [100_GetAllCenterType]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,
        Name,
        NameEN,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM CenterType
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder, Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM CenterType
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%');
END;
GO 