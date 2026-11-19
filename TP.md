Estoy haciendo un TP individual de la facultad: una web para que alumnos se inscriban a
encuentros de talleres extracurriculares. Stack: ASP.NET MVC, Session, SQL Server con
Dapper y Fetch.
Reglas que tengo que respetar:
- Todo el acceso a datos está en una clase estática Models/BD.cs, con "using (SqlConnection
...)" y Dapper (Query, QueryFirstOrDefault, Execute) con parámetros @pNombre.
- Las propiedades de los modelos se llaman IGUAL que las columnas y tienen { get; set; }.
- Session guarda solo strings: solo guardo el IdUsuario.
- HTML semántico, un solo CSS (wwwroot/css/estilos.css), JavaScript con let/const,
getElementById e innerHTML.
- Los forms usan method="post" y su action apunta a una acción del Controller; los name de
los inputs son iguales a las propiedades del modelo.
- Uso RedirectToAction para ir a otra pantalla y ViewBag para mandar datos/mensajes a la
vista.
- Nada de código complicado que no se vea en una cursada de primer nivel de .NET: quiero
poder explicar cada línea.
DER (tablas y columnas tal cual):
Usuarios(Id PK identity, NombreUsuario varchar(50) único, Contrasena varchar(50), Nombre
varchar(50), Apellido varchar(50))
Categorias(Id PK identity, Nombre varchar(50))
Talleres(Id PK identity, Nombre varchar(100), Descripcion varchar(500), Imagen
varchar(200), IdCategoria FK, Activo bit)
Encuentros(Id PK identity, IdTaller FK, FechaHora datetime, DuracionMinutos int, Cupo int)
Inscripciones(Id PK identity, IdEncuentro FK, IdUsuario FK, FechaInscripcion datetime,
Cancelado bit, FechaCancelacion datetime NULL)
Después de darme el código, explicámelo línea por línea en lenguaje simple.