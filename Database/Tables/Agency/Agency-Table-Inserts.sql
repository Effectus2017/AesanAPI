-- Agencias Auspiciadoras (Sponsoring Agencies)
INSERT INTO Agency
    (Name, AgencyStatusId, CityId, PostalCityId, RegionId, PostalRegionId, SdrNumber, UieNumber, EinNumber,
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageURL,
    IsActive, IsListable, AgencyCode, CreatedAt, AgencyInscriptionId)
VALUES
    ('NUTRE', 1, 1, NULL, 1, NULL, 123456, 123456, 123456,
        'Calle Nutre #456', 00123, 'Calle Nutre #456', 00123, 18.220833, -66.590149, '787-987-6543',
        'contact@nutre.pr.gov', NULL, 1, 1, 'NUTRE-2025-001', GETDATE(), 1);
