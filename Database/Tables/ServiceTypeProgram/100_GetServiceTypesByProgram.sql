-- =============================================
-- Stored Procedure: 100_GetServiceTypesByProgram
-- Descripción: Devuelve tipos de servicio válidos para un programa con isstrongservice y minimumminutestonextservice
-- AESAN-257
-- Convención SPs: parámetros y alias de salida lowercase
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetServiceTypesByProgram]
    @programid INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id = st.Id,
        name = st.Name,
        nameen = st.NameEN,
        code = st.Code,
        displayorder = stp.DisplayOrder,
        isstrongservice = stp.IsStrongService,
        minimumminutestonextservice = stp.MinimumMinutesToNextService,
        isactive = st.IsActive
    FROM [dbo].[ServiceType] st
        INNER JOIN [dbo].[ServiceTypeProgram] stp ON st.Id = stp.ServiceTypeId
    WHERE stp.ProgramId = @programid
        AND st.IsActive = 1
        AND stp.IsActive = 1
    ORDER BY stp.DisplayOrder, st.Name;
END;
GO
