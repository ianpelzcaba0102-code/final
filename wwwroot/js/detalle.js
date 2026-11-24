const radiosEncuentro = document.querySelectorAll(".encuentro-radio");
const resultadoLugares = document.getElementById("lugaresDisponibles");
const urlLugaresDisponibles = resultadoLugares.dataset.url;

for (const radio of radiosEncuentro) {
    radio.addEventListener("change", async function () {
        resultadoLugares.innerHTML = "Consultando lugares disponibles...";

        try {
            const respuesta = await fetch(
                `${urlLugaresDisponibles}?idEncuentro=${encodeURIComponent(radio.value)}`
            );

            if (!respuesta.ok) {
                throw new Error("No se pudieron consultar los lugares disponibles.");
            }

            const datos = await respuesta.json();
            resultadoLugares.innerHTML =
                `Lugares disponibles: <strong>${datos.lugaresDisponibles}</strong>`;
        } catch (error) {
            resultadoLugares.innerHTML = error.message;
        }
    });
}
