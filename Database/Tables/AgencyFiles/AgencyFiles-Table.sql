-- Tabla para almacenar archivos asociados a las agencias
CREATE TABLE AgencyFiles
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL, -- Agencia a la que pertenece el archivo
    DocumentTypeId INT NULL, -- Tipo de documento
    FileName NVARCHAR(255) NOT NULL, -- Nombre original del archivo
    StoredFileName NVARCHAR(255) NOT NULL, -- Nombre del archivo en el sistema
    FileUrl NVARCHAR(MAX) NOT NULL, -- URL donde se almacena el archivo
    ContentType NVARCHAR(100) NOT NULL, -- Tipo de contenido MIME
    FileSize BIGINT NOT NULL, -- Tamaño del archivo en bytes
    Description NVARCHAR(500) NULL, -- Descripción opcional del archivo
    UploadDate DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de subida, esta fecha NO se actualiza ya que va a existir un servicio que contabiliza 
    UploadedBy NVARCHAR(128) NULL, -- ID del usuario que subió el archivo
    IsActive BIT NOT NULL DEFAULT 1, -- Indica si el archivo está activo, se puede desactivar para que no se pueda ver
    IsDeleted BIT NOT NULL DEFAULT 0, -- Indica si el archivo está eliminado
    IsVerified BIT NOT NULL DEFAULT 0, -- Indica si el archivo está verificado por AESAN para terminar el contador para no rechazar el archivo
    ExpirationDate DATETIME NULL, -- Fecha de expiración del archivo del documento, lo puede cambiar el usuario de la agencia
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentTypes(Id)
    -- asociar el documento a la solicitud (ver el numero de la solicitud)
);
GO


-- Índice para mejorar el rendimiento en las búsquedas por agencia
CREATE INDEX IX_AgencyFiles_AgencyId ON AgencyFiles(AgencyId);
GO 

-- Índice para mejorar el rendimiento en las búsquedas por archivo
CREATE INDEX IX_AgencyFiles_FileName ON AgencyFiles(FileName);
GO

-- Índice para mejorar el rendimiento en las búsquedas por fecha de subida
CREATE INDEX IX_AgencyFiles_UploadDate ON AgencyFiles(UploadDate);
GO

-- agregar el tipo de documento a la tabla de documentos    
ALTER TABLE AgencyFiles
ADD CONSTRAINT FK_AgencyFiles_DocumentTypeId FOREIGN KEY (DocumentTypeId) REFERENCES DocumentTypes(Id); 
GO


-- alter table para agregar el tipo de documento y el campo de fecha de expiración
ALTER TABLE AgencyFiles
ADD DocumentTypeId INT NULL,
    ExpirationDate DATETIME NULL;
GO

-- tipos de documentos
CREATE TABLE DocumentTypes
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);


-- tipos de documentos
-- facturas
-- fecha
-- menu
-- lista de escuelas
-- documento pre award

INSERT INTO DocumentTypes (Name, Description) VALUES ('Factura', 'Factura');
INSERT INTO DocumentTypes (Name, Description) VALUES ('Fecha', 'Fecha');
INSERT INTO DocumentTypes (Name, Description) VALUES ('Menu', 'Menu');
INSERT INTO DocumentTypes (Name, Description) VALUES ('Lista de escuelas', 'Lista de escuelas');
INSERT INTO DocumentTypes (Name, Description) VALUES ('Documento pre award', 'Documento pre award');

