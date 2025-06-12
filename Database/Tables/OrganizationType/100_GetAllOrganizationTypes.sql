CREATE PROCEDURE [dbo].[100_GetAllOrganizationTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(100) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, DisplayOrder
    FROM OrganizationTypes
    WHERE (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM OrganizationTypes
    WHERE (@name IS NULL OR Name LIKE '%' + @name + '%');
END 