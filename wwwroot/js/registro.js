const formulario = document.getElementById("registroForm");

formulario.addEventListener("submit", function (event) {
    const campos = [
        { id: "Email", etiqueta: "Correo electrónico", maximo: 254 },
        { id: "NombreUsuario", etiqueta: "Nombre de usuario", maximo: 50 },
        { id: "Contrasena", etiqueta: "Contraseña", maximo: 50 },
        { id: "RepetirContrasena", etiqueta: "Repetir contraseña", maximo: 50 },
        { id: "Nombre", etiqueta: "Nombre", maximo: 50 },
        { id: "Apellido", etiqueta: "Apellido", maximo: 50 }
    ];
    let errores = [];

    for (const campo of campos) {
        const valor = document.getElementById(campo.id).value;

        if (valor.trim() === "") {
            errores.push(campo.etiqueta + " es obligatorio.");
        }

        if (valor.length > campo.maximo) {
            errores.push(campo.etiqueta + " no puede superar los " + campo.maximo + " caracteres.");
        }
    }

    const contrasena = document.getElementById("Contrasena").value;
    const repetirContrasena = document.getElementById("RepetirContrasena").value;
    if (contrasena !== repetirContrasena) {
        errores.push("Las contraseñas no coinciden.");
    }

    const contenedorErrores = document.getElementById("erroresValidacion");
    contenedorErrores.innerHTML = errores.length > 0
        ? "<ul><li>" + errores.join("</li><li>") + "</li></ul>"
        : "";

    if (errores.length > 0) {
        event.preventDefault();
    }
});
