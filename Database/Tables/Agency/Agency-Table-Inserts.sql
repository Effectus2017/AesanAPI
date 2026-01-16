-- Agencia NUTRE
INSERT INTO Agency
    (Name, AgencyStatusId, CityId, PostalCityId, RegionId, PostalRegionId, SdrNumber, UieNumber, EinNumber,
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageUrl,
    IsActive, IsListable, AgencyCode, CreatedAt, IsProprietary, IsRecurrent)
VALUES
    ('NUTRE', 1, 1, NULL, 1, NULL, 123456, 123456, 123456,
        'Calle Nutre #456', '00123', 'Calle Nutre #456', '00123', 18.220833, -66.590149, '787-987-6543',
        'contact@nutre.pr.gov', NULL, 1, 1, 'NUTRE-2025-001', GETDATE(), 1, 0);

-- Agencia Dario Neira  
INSERT INTO Agency
    (Name, AgencyStatusId, CityId, PostalCityId, RegionId, PostalRegionId, SdrNumber, UieNumber, EinNumber,
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageUrl,
    IsActive, IsListable, AgencyCode, CreatedAt, IsProprietary, IsRecurrent)
VALUES
    ('Dario Neira', 1, 1, NULL, 1, NULL, 123456, 123456, 123456,
        'Calle Dario Neira #456', '00123', 'Calle Dario Neira #456', '00123', 18.220833, -66.590149, '787-987-6543',
        'dario.neira@gmail.com', NULL, 1, 1, 'DNI-2025-001', GETDATE(), 0, 1);