const botonVerMas = document.getElementById("verMas");

if (botonVerMas) {
    const contenedorTalleres = document.getElementById("talleres");
    const mensajeError = document.getElementById("errorVerMas");
    const urlVerMas = botonVerMas.dataset.urlVerMas;
    const urlDetalle = botonVerMas.dataset.urlDetalle;
    const rutaImagenes = botonVerMas.dataset.rutaImagenes;

    botonVerMas.addEventListener("click", async function () {
        botonVerMas.disabled = true;
        mensajeError.textContent = "";

        try {
            const cantidadMostrada = contenedorTalleres.querySelectorAll(".taller-card").length;
            const respuesta = await fetch(`${urlVerMas}?cantidadMostrada=${cantidadMostrada}`);

            if (!respuesta.ok) {
                throw new Error("No se pudieron cargar más talleres.");
            }

            const resultado = await respuesta.json();

            for (const taller of resultado.talleres) {
                const tarjeta = document.createElement("a");
                const detalleUrl = new URL(urlDetalle, window.location.origin);
                detalleUrl.searchParams.set("id", taller.id);
                tarjeta.className = "taller-card";
                tarjeta.href = detalleUrl.toString();

                const imagen = document.createElement("img");
                imagen.src = rutaImagenes + taller.imagen;
                imagen.alt = taller.nombre;

                const contenido = document.createElement("div");
                contenido.className = "taller-card-content";

                const nombre = document.createElement("h2");
                nombre.textContent = taller.nombre;

                const categoria = document.createElement("p");
                categoria.className = "taller-category";
                categoria.textContent = taller.nombreCategoria;

                contenido.append(nombre, categoria);
                tarjeta.append(imagen, contenido);
                contenedorTalleres.appendChild(tarjeta);
            }

            if (!resultado.hayMas) {
                botonVerMas.hidden = true;
            }
        } catch (error) {
            mensajeError.textContent = error.message;
        } finally {
            botonVerMas.disabled = false;
        }
    });
}
