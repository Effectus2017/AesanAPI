INSERT INTO Agency
    (Name, AgencyStatusId, CityId, PostalCityId, RegionId, PostalRegionId, SdrNumber, UieNumber, EinNumber, 
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageURL, 
    IsActive, IsListable, AgencyCode, CreatedAt)
VALUES
    ('AESAN', 5, 1, 1, 1, 1, 787, 123456, 123456, 123456, 'Calle Principal #123', 00123, 
    'Calle Principal #123', 00123, 18.220833, -66.590149, '787-123-4567', 
    'admin@aesan.pr.gov', NULL, 1, 1, 'AESAN-2025-001', GETDATE());