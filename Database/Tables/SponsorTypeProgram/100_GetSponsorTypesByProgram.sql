CREATE OR ALTER PROCEDURE [100_GetSponsorTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT st.Id,
        st.Name,
        st.NameEN,
        st.IsActive,
        st.DisplayOrder,
        st.SelectionNotification,
        st.CreatedAt,
        st.UpdatedAt
    FROM SponsorType st
        INNER JOIN SponsorTypeProgram stp ON st.Id = stp.SponsorTypeId
    WHERE stp.ProgramId = @programId
        AND st.IsActive = 1
        AND stp.IsActive = 1
    ORDER BY st.DisplayOrder, st.Name;
END;
GO

