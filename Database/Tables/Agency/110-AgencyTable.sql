-- Agencias Auspiciadoras (Sponsoring Agencies)
CREATE TABLE Agency
(
    Id int NOT NULL IDENTITY(1,1),
	AgencyStatusId int NOT NULL,
    AgencyInscriptionId int NULL,
    AgencyCode nvarchar(50) NULL,
	CityId int NOT NULL,
	PostalCityId int NULL,
	RegionId int NOT NULL,
	PostalRegionId int NULL,
	Name nvarchar(255) NOT NULL,
	UieNumber int NOT NULL,
	EinNumber int NOT NULL,
	SdrNumber int NOT NULL,
	Address nvarchar(255) NOT NULL,
	ZipCode nvarchar(20) NULL,
	PostalAddress nvarchar(255) NOT NULL,
	PostalZipCode nvarchar(20) NULL,
	Phone nvarchar(20) NOT NULL DEFAULT (''),
	Email nvarchar(255) NOT NULL DEFAULT (''),
	Latitude real NOT NULL DEFAULT (0),
	Longitude real NOT NULL DEFAULT (0),
	ImageURL nvarchar(max) NULL,
	IsActive bit NOT NULL DEFAULT (1),
	IsListable bit NOT NULL DEFAULT (1),
    IsPropietary bit NULL DEFAULT (0),
	CreatedAt datetime NOT NULL DEFAULT GETDATE(),
	UpdatedAt datetime NULL,
    FOREIGN KEY (AgencyStatusId) REFERENCES AgencyStatus(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (PostalCityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (PostalRegionId) REFERENCES Region(Id),
    FOREIGN KEY (AgencyInscriptionId) REFERENCES AgencyInscription(Id)
);
GO
ALTER TABLE Agency
ADD AgencyInscriptionId int NULL;
ALTER TABLE Agency
ADD FOREIGN KEY (AgencyInscriptionId) REFERENCES AgencyInscription(Id);
GO