-- Tabla para almacenar archivos asociados a las agencias
CREATE TABLE AgencyFiles
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    FileName NVARCHAR(255) NOT NULL, -- Nombre original del archivo
    StoredFileName NVARCHAR(255) NOT NULL, -- Nombre del archivo en el sistema
    FileUrl NVARCHAR(MAX) NOT NULL, -- URL donde se almacena el archivo
    ContentType NVARCHAR(100) NOT NULL, -- Tipo de contenido MIME
    FileSize BIGINT NOT NULL, -- Tamaño del archivo en bytes
    Description NVARCHAR(500) NULL, -- Descripción opcional del archivo
    DocumentType NVARCHAR(100) NULL, -- Tipo de documento
    UploadDate DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de subida, esta fecha NO se actualiza ya que va a existir un servicio que contabiliza 
    UploadedBy NVARCHAR(128) NULL, -- ID del usuario que subió el archivo
    IsActive BIT NOT NULL DEFAULT 1, -- Indica si el archivo está activo
    IsDeleted BIT NOT NULL DEFAULT 0, -- Indica si el archivo está eliminado
    IsVerified BIT NOT NULL DEFAULT 0, -- Indica si el archivo está verificado por AESAN para terminar el contador para no rechazar el archivo
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
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