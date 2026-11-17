USE [master];
GO

IF DB_ID(N'TalleresDB') IS NOT NULL
BEGIN
    ALTER DATABASE [TalleresDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TalleresDB];
END;
GO

CREATE DATABASE [TalleresDB];
GO

USE [TalleresDB];
GO

CREATE TABLE dbo.Usuarios (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Contrasena VARCHAR(50) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL
);
GO

CREATE TABLE dbo.Categorias (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);
GO

CREATE TABLE dbo.Talleres (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    Imagen VARCHAR(200) NOT NULL,
    IdCategoria INT NOT NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Talleres_Activo DEFAULT (1),
    CONSTRAINT FK_Talleres_Categorias
        FOREIGN KEY (IdCategoria) REFERENCES dbo.Categorias(Id)
);
GO

CREATE TABLE dbo.Encuentros (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdTaller INT NOT NULL,
    FechaHora DATETIME NOT NULL,
    DuracionMinutos INT NOT NULL,
    Cupo INT NOT NULL,
    CONSTRAINT FK_Encuentros_Talleres
        FOREIGN KEY (IdTaller) REFERENCES dbo.Talleres(Id)
);
GO

CREATE TABLE dbo.Inscripciones (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdEncuentro INT NOT NULL,
    IdUsuario INT NOT NULL,
    FechaInscripcion DATETIME NOT NULL,
    Cancelado BIT NOT NULL CONSTRAINT DF_Inscripciones_Cancelado DEFAULT (0),
    FechaCancelacion DATETIME NULL,
    CONSTRAINT FK_Inscripciones_Encuentros
        FOREIGN KEY (IdEncuentro) REFERENCES dbo.Encuentros(Id),
    CONSTRAINT FK_Inscripciones_Usuarios
        FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(Id)
);
GO
