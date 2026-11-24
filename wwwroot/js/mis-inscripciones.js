const datosCancelacion = document.getElementById("datosCancelacion");
const urlCancelar = datosCancelacion.dataset.urlCancelar;
const tokenAntiforgery = datosCancelacion.querySelector(
    'input[name="__RequestVerificationToken"]'
).value;

document.querySelectorAll(".btn-cancelar").forEach(function (boton) {
    boton.addEventListener("click", async function () {
        const fila = boton.closest("tr");
        const mensaje = fila.querySelector(".mensaje-cancelacion");
        const parametros = new URLSearchParams();
        parametros.set("idInscripcion", boton.dataset.idInscripcion);
        boton.disabled = true;
        mensaje.textContent = "";

        try {
            const respuesta = await fetch(urlCancelar, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "RequestVerificationToken": tokenAntiforgery
                },
                body: parametros
            });
            const resultado = await respuesta.json();

            if (resultado.ok) {
                fila.remove();
            } else {
                mensaje.textContent = resultado.mensaje;
            }
        } catch {
            mensaje.textContent = "No se pudo completar la cancelación. Intentá nuevamente.";
        } finally {
            boton.disabled = false;
        }
    });
});
