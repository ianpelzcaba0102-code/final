const formulario = document.getElementById("registroForm");

formulario.addEventListener("submit", function (event) {
    const campos = [
        { id: "NombreUsuario", etiqueta: "Nombre de usuario" },
        { id: "Contrasena", etiqueta: "Contraseña" },
        { id: "Nombre", etiqueta: "Nombre" },
        { id: "Apellido", etiqueta: "Apellido" }
    ];
    let errores = [];

    for (const campo of campos) {
        const valor = document.getElementById(campo.id).value;

        if (valor.trim() === "") {
            errores.push(campo.etiqueta + " es obligatorio.");
        }

        if (valor.length > 50) {
            errores.push(campo.etiqueta + " no puede superar los 50 caracteres.");
        }
    }

    const contenedorErrores = document.getElementById("erroresValidacion");
    contenedorErrores.innerHTML = errores.length > 0
        ? "<ul><li>" + errores.join("</li><li>") + "</li></ul>"
        : "";

    if (errores.length > 0) {
        event.preventDefault();
    }
});
