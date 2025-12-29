-- 100_GetAllDeliveryTypes.sql
-- Obtiene todos los tipos de entrega
CREATE OR ALTER PROCEDURE [100_GetAllDeliveryTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, CreatedAt, UpdatedAt, DisplayOrder
    FROM DeliveryType
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder, Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM DeliveryType
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%');
END; 