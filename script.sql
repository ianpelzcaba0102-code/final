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

INSERT INTO dbo.Categorias (Nombre)
VALUES
    ('Tecnología'),
    ('Arte'),
    ('Deportes'),
    ('Música'),
    ('Idiomas');
GO

INSERT INTO dbo.Talleres (Nombre, Descripcion, Imagen, IdCategoria, Activo)
VALUES
    ('Robótica para principiantes', 'Construí y programá robots sencillos mientras aprendés conceptos básicos de electrónica. No se requiere experiencia previa.', 'robotica.jpg', 1, 1),
    ('Programación con Python', 'Aprendé los fundamentos de Python creando pequeños programas. El taller incluye ejercicios prácticos para resolver problemas cotidianos.', 'python.jpg', 1, 1),
    ('Diseño e impresión 3D', 'Conocé las herramientas para diseñar objetos tridimensionales y prepararlos para impresión. Al final crearás un modelo propio.', 'impresion3d.jpg', 1, 1),
    ('Desarrollo de videojuegos', 'Explorá las etapas de creación de un videojuego y armá una experiencia interactiva sencilla. Trabajarás con lógica, diseño y pruebas.', 'videojuegos.jpg', 1, 1),
    ('Dibujo y observación', 'Practicá técnicas de dibujo a partir de objetos y escenas cotidianas. Aprenderás a observar formas, proporciones y luces.', 'dibujo.jpg', 2, 1),
    ('Pintura acrílica', 'Experimentá con colores y pinceladas usando pintura acrílica. Crearás una obra propia y conocerás técnicas de composición.', 'pintura-acrilica.jpg', 2, 1),
    ('Cerámica inicial', 'Modelá piezas simples en arcilla y aprendé técnicas básicas de construcción manual. El taller combina práctica y exploración de formas.', 'ceramica.jpg', 2, 1),
    ('Fotografía creativa', 'Aprendé a componer imágenes y aprovechar la luz disponible. Pondrás en práctica distintas ideas fotográficas con cámara o celular.', 'fotografia.jpg', 2, 1),
    ('Iniciación al fútbol', 'Practicá conducción, pases y coordinación mediante juegos en equipo. Las actividades se adaptan a distintos niveles de experiencia.', 'futbol.jpg', 3, 1),
    ('Vóley recreativo', 'Aprendé las técnicas esenciales del vóley con ejercicios individuales y grupales. El taller prioriza el juego y la colaboración.', 'voley.jpg', 3, 1),
    ('Yoga y movimiento', 'Explorá posturas básicas, respiración y movimientos suaves. Cada encuentro propone una práctica accesible para distintos niveles.', 'yoga.jpg', 3, 1),
    ('Ajedrez para todos', 'Aprendé las reglas y principios estratégicos del ajedrez. Resolverás desafíos y jugarás partidas amistosas.', 'ajedrez.jpg', 3, 1),
    ('Guitarra desde cero', 'Aprendé acordes, ritmos y melodías sencillas en guitarra. No necesitás conocimientos musicales previos.', 'guitarra.jpg', 4, 1),
    ('Canto grupal', 'Practicá respiración, afinación y expresión vocal en grupo. El repertorio incluye canciones de distintos estilos.', 'canto.jpg', 4, 1),
    ('Percusión latina', 'Conocé ritmos latinoamericanos y practicá patrones básicos de percusión. Podrás participar con instrumentos o elementos cotidianos.', 'percusion.jpg', 4, 1),
    ('Producción musical inicial', 'Descubrí los pasos básicos para crear y organizar una pieza musical digital. Trabajarás con ritmo, estructura y sonidos.', 'produccion-musical.jpg', 4, 1),
    ('Inglés conversacional', 'Practicá vocabulario y expresiones útiles para conversaciones cotidianas. Las actividades incluyen diálogos y juegos de comunicación.', 'ingles.jpg', 5, 1),
    ('Francés inicial', 'Aprendé saludos, frases básicas y vocabulario frecuente en francés. También conocerás aspectos culturales de distintas regiones francófonas.', 'frances.jpg', 5, 1),
    ('Italiano para principiantes', 'Iniciate en la pronunciación y las expresiones más comunes del italiano. Practicarás situaciones simples de viaje y presentación personal.', 'italiano.jpg', 5, 1),
    ('Portugués práctico', 'Aprendé expresiones y vocabulario para comunicarte en portugués en situaciones cotidianas. El taller combina escucha y conversación guiada.', 'portugues.jpg', 5, 1);
GO

INSERT INTO dbo.Usuarios (NombreUsuario, Contrasena, Nombre, Apellido)
VALUES
    ('usuario_prueba1', 'Prueba123', 'Ana', 'García'),
    ('usuario_prueba2', 'Prueba123', 'Bruno', 'López'),
    ('usuario_prueba3', 'Prueba123', 'Carla', 'Martínez');
GO
