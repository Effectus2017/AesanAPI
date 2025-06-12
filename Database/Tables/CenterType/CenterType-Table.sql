-- Center Type
CREATE TABLE CenterType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

INSERT INTO CenterType
    (Name, NameEN, DisplayOrder, IsActive)
VALUES
    ('Orfanato', 'Orphanage', 10, 1),
    ('Centro de tratamiento residencial para salud mental', 'Residential Treatment Center for Mental Health', 20, 1),
    ('Centro Correccional Juvenil', 'Juvenile Correctional Center', 30, 1);