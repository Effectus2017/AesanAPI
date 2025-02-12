-- Procedimiento para actualizar una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    @Address NVARCHAR(255),
    @ZipCode INT,
    @Phone NVARCHAR(20),
    @Email NVARCHAR(255),
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL,
    @Comment NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL,
    @AgencyCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET Name = @Name,
        AgencyStatusId = @AgencyStatusId,
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        Address = @Address,
        ZipCode = @ZipCode,
        Phone = @Phone,
        Email = @Email,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = @Latitude,
        Longitude = @Longitude,
        PostalAddress = @PostalAddress,
        PostalZipCode = @PostalZipCode,
        PostalCityId = @PostalCityId,
        PostalRegionId = @PostalRegionId,
        NonProfit = @NonProfit,
        FederalFundsDenied = @FederalFundsDenied,
        StateFundsDenied = @StateFundsDenied,
        OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
        RejectionJustification = @RejectionJustification,
        ImageURL = @ImageURL,
        Comment = @Comment,
        AppointmentCoordinated = @AppointmentCoordinated,
        AppointmentDate = @AppointmentDate,
        AgencyCode = @AgencyCode,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

-- Procedimiento para eliminar una agencia (soft delete)
CREATE OR ALTER PROCEDURE [100_DeleteAgency]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 

-- Procedimiento almacenado para obtener una agencia por su ID para la visita preoperacional
CREATE OR ALTER PROCEDURE [dbo].[101_GetAgencyByIdAndUserId]
    @agencyId INT,
    @userId NVARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de la agencia
    SELECT
        a.*,
        -- Ciudad y Región
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        -- Ciudad y Región Postal
        pc.Id AS PostalCityId,
        pc.Name AS PostalCityName,
        pr.Id AS PostalRegionId,
        pr.Name AS PostalRegionName,
        -- Estado
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Usuario
        u.Id AS UserId,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        -- Comments
        ap.Comment,
        ap.AppointmentCoordinated AS AppointmentCoordinated,
        ap.AppointmentDate AS AppointmentDate
    FROM Agency a
        -- Joins para Ciudad y Región
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        -- Joins para Ciudad y Región Postal
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        -- Join para Estado
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        -- Join para Usuario
        LEFT JOIN AspNetUsers u ON a.Id = u.AgencyId
        -- Join para Programas
        LEFT JOIN AgencyProgram ap ON ap.AgencyId = a.id
        LEFT JOIN UserProgram up ON up.ProgramId = ap.ProgramId
    WHERE a.Id = @agencyId AND up.UserId = @userId;

    -- Segunda consulta: Obtener los programas asociados al usuario y la agencia
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        INNER JOIN UserProgram up ON p.Id = up.ProgramId
    WHERE ap.AgencyId = @agencyId AND up.UserId = @userId;

    -- Obtener el usuario que hizo el appointment
    SELECT
        u.Id,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName
    FROM AspNetUsers u
        INNER JOIN AgencyProgram ap ON ap.UserId = u.Id AND ap.AgencyId = @agencyId;
END;
GO