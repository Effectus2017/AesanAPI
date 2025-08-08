-- =============================================
-- Stored Procedure: 100_GetStaffById
-- =============================================
-- Obtiene un miembro del staff específico por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.StatusId,
        os_status.Name AS StatusName,
        s.PositionId,
        os_position.Name AS PositionName,
        s.StaffTypeId,
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,
        s.BirthDate,
        s.Email,
        s.PostalAddress,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.AreaCode,
        s.Comments,
        s.UserId,
        u.FirstName + ' ' + u.FatherLastName AS UserName,
        s.CreatedAt,
        s.UpdatedAt,
        s.IsActive,
        s.ReviewResultId,
        s.ReviewDate,
        s.ReviewJustification
    FROM Staff s
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
        LEFT JOIN City c ON s.CityId = c.Id
        LEFT JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN AspNetUsers u ON s.UserId = u.Id
    WHERE s.Id = @id;
END