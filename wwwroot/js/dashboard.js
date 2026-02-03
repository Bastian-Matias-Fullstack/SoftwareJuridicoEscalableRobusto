       function obtenerRolesDesdeJWT() {
        const token = localStorage.getItem("jwt_token");
        if (!token) return [];
        try {
            const payloadBase64 = token.split('.')[1];
            const payloadJson = atob(payloadBase64);
            const payload = JSON.parse(payloadJson);
            const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            return Array.isArray(roles) ? roles : [roles];
        } catch (e) {
            console.error("Error al decodificar el token:", e);
            return [];
        }
    }
            function obtenerRolEfectivo(roles) {
        if (roles.includes("Admin")) return "Admin";
        if (roles.includes("Abogado")) return "Abogado";
        if (roles.includes("Soporte")) return "Soporte";
        return null;
    }
        function aplicarVisibilidadPorRol(roles) {
        const gestionRoles = document.getElementById("seccion-gestion-roles");
        const btnNuevoCaso = document.getElementById("btnNuevoCaso");
        const tablaCasos = document.getElementById("tablaCasosWrapper");
        const seccionUsuarios = document.getElementById("seccion-usuarios");
        const btnNuevoUsuario = document.getElementById("btnNuevoUsuario");
        // 🔹 1️⃣ Determinar rol efectivo (UI = 1 solo estado)
        let rol = null;
        if (roles?.includes("Admin")) {
            rol = "Admin";
        } else if (roles?.includes("Abogado")) {
            rol = "Abogado";
        } else if (roles?.includes("Soporte")) {
            rol = "Soporte";
        }
        // 🔹 2️⃣ Reset visual TOTAL (estado limpio)
        [
            gestionRoles,
            btnNuevoCaso,
            tablaCasos,
            seccionUsuarios,
            btnNuevoUsuario
        ].forEach(el => el?.classList.add("d-none"));

        // 🔹 3️⃣ Aplicar reglas exclusivas por rol
        switch (rol) {
            case "Admin":
                gestionRoles?.classList.remove("d-none");
                btnNuevoCaso?.classList.remove("d-none");
                tablaCasos?.classList.remove("d-none");
                seccionUsuarios?.classList.remove("d-none");
                btnNuevoUsuario?.classList.remove("d-none");
                break;

            case "Abogado":
                btnNuevoCaso?.classList.remove("d-none");
                tablaCasos?.classList.remove("d-none");
                break;

            case "Soporte":
                seccionUsuarios?.classList.remove("d-none");
                btnNuevoUsuario?.classList.remove("d-none");
                break;

            default:
                // No autorizado o sin rol válido
                break;
        }
    }
    function navigate(moduloId) {
    // 1️⃣ Mostrar módulo
    mostrarModulo(moduloId);

    // 2️⃣ Estado activo del menú
    document.querySelectorAll(".sidebar-menu li")
        .forEach(li => li.classList.remove("active"));

    const navId = "nav-" + moduloId.replace("mod-", "");
    document.getElementById(navId)?.classList.add("active");
    // AQUÍ SE ENGANCHA EL PASO 1
    onModuloCargado(moduloId);
       aplicarVisibilidadPorRol(obtenerRolesDesdeJWT());
}
// ===============================
// Demo Context (SE2)
// ===============================
const demoContext = sessionStorage.getItem("demoContext"); 
function applyDemoContextVisibility(context) {
    const casos = document.getElementById("tablaCasosWrapper");
    const roles = document.getElementById("seccion-gestion-roles");
    const usuarios = document.getElementById("seccion-usuarios");
    // Ocultamos todo primero
    [casos, roles, usuarios].forEach(el => el?.classList.add("d-none"));

    switch (context) {
        case "casos":
            casos?.classList.remove("d-none");
            break;

        case "roles":
            roles?.classList.remove("d-none");
            break;

        case "usuarios":
            usuarios?.classList.remove("d-none");
            break;

        default:
            // sin demoContext → comportamiento normal
            [casos, roles, usuarios].forEach(el => el?.classList.remove("d-none"));
    }
}
document.addEventListener("DOMContentLoaded", () => {
    /*➡️ Espera que el DOM esté completamente cargado antes de ejecutar el código JS (buena práctica para manipular el DOM).
    */
    // 1️⃣ Obtener roles PRIMERO
    const roles = obtenerRolesDesdeJWT();
    aplicarVisibilidadPorRol(roles);
    const rol = obtenerRolEfectivo(roles);
    function configurarSidebarPorRol(rol) {
        // Admin: ve todo
        if (rol === "Admin") return;
        // No Admin → ocultar Roles
        document.getElementById("nav-roles")?.remove();        // Solo Admin y Soporte ven Usuarios
        if (rol !== "Soporte") {
            document.getElementById("nav-usuarios")?.remove();
        }
        if (rol === "Soporte") {
            document.getElementById("nav-casos")?.remove();
        }
    }

    // ===============================
    // 🔹 INICIALIZACIÓN DE LA UI
    // ===============================
    // 2️⃣ Configurar qué opciones del sidebar se muestran
    configurarSidebarPorRol(rol);
    aplicarVisibilidadPorRol(roles);
    // 3️⃣ Cargar módulo inicial según rol
    if (rol === "Admin") {
        navigate("mod-dashboard");
    }
    if (rol === "Abogado") {
        navigate("mod-casos");
    }
    if (rol === "Soporte") {
        navigate("mod-usuarios");
    }
    const apiUrl = "https://localhost:7266/api/Casos";
    /*➡️ Define la URL base para la API de casos.
    */
    const token = localStorage.getItem("jwt_token");
    /*➡️ Recupera el token JWT desde localStorage (para autorizar las peticiones).*/
    let usuario = JSON.parse(localStorage.getItem("usuario_actual"));
    if (!usuario) {
        // Si no existe usuario, asignamos un objeto vacío
        usuario = {};
    }
    /*➡️ Obtiene el usuario actual guardado (si existe).*/
    const saludo = document.getElementById("saludoUsuario");
    /*➡️ Elemento donde mostrarás “Hola, Usuario”.*/

    console.log("Rol del usuario:", roles);

    // 1️⃣ Siempre aplicar reglas de rol (base de la app)
    // 2️⃣ Solo si existe demoContext, se aplica encima (modo demo)
    if (demoContext) {
        applyDemoContextVisibility(demoContext);
    }
    // Aplicamos Choise.Js
    const filtroEstado = document.getElementById("filtroEstado");
    const choicesEstado = new Choices(filtroEstado, {
        searchEnabled: false,
        itemSelectText: '',
        shouldSort: false
    });

    const filtros = {
        estado: null,
        pagina: 1,
        tamanio: 10
    };

    /*➡️ Filtro inicial: sin estado aplicado, página 1, 10 resultados por página.*/


    if (!token) {
        alert("Token no encontrado. Redirigiendo al login...");
        window.location.href = "login.html";
        return;
    }

    if (saludo && usuario.nombre) {
        saludo.textContent = `Hola, ${usuario.nombre}`;
    }

    /*➡️ Personaliza el saludo si el usuario tiene nombre guardado.*/
    cargarCasosDesdeBackend();
    // ===============================
// 🧭 PAGINACIÓN: listener único (fix mid-senior)
// ===============================
const paginacion = document.getElementById("paginacion");

paginacion?.addEventListener("click", (e) => {
    const link = e.target.closest(".page-link");
    if (!link) return;

    e.preventDefault();

    const nuevaPagina = parseInt(link.dataset.page, 10);
    if (!Number.isInteger(nuevaPagina)) return;

    if (nuevaPagina !== filtros.pagina) {
        filtros.pagina = nuevaPagina;
        cargarCasosDesdeBackend();
    }
});

    /**➡️ Ejecuta carga inicial. */
    filtroEstado.addEventListener("change", () => {
        const estadoSeleccionado = filtroEstado.value?.trim();
        filtros.estado = estadoSeleccionado || null;
        filtros.pagina = 1;
        let casosFiltrados = [];

        cargarCasosDesdeBackend();
    });
    /**➡️ Actualiza el filtro de estado y recarga la tabla */
    function construirQueryString(filtros) {
        const params = new URLSearchParams();
        if (filtros.estado) params.append("estado", filtros.estado);
        if (filtros.pagina) params.append("pagina", filtros.pagina);
        if (filtros.tamanio) params.append("tamanio", filtros.tamanio);
        return "?" + params.toString();
    }
    /**➡️ Transforma tu objeto filtros en un query string para el fetch. */
    async function cargarCasosDesdeBackend() {
        const query = construirQueryString(filtros);

        const response = await fetch(`${apiUrl}${query}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });


        if (!response.ok) {
            if (response.status === 401) {
                alert("Sesión expirada. Redirigiendo...");
                localStorage.removeItem("jwt_token");
                localStorage.removeItem("usuario_actual");
                window.location.href = "login.html";
                return;
            }

            console.error("Error al obtener los casos:", response.status);
            return;
        }

        const data = await response.json();

        // ✅ Validación mínima para evitar errores si backend falla
        if (!data.items || !data.resumen) {
            console.warn("⚠️ La respuesta del backend no tiene el formato esperado:", data);
            document.getElementById("contadorResultados").textContent =
                "⚠️ No se pudieron cargar los datos correctamente.";
            return;
        }

        renderizarTabla(data.items);
        actualizarResumen(data.resumen);
        mostrarMensajeInformativo(data.items.length, data.totalRegistros);
        renderizarPaginacion(data.pagina, data.totalPaginas);


    }
    function renderizarTabla(lista) {
        const tbody = document.getElementById("casosBody");
        tbody.classList.remove("opacity-100");
        tbody.classList.add("opacity-0");
        tbody.innerHTML = "";

        lista.forEach(caso => {
            const estadoBadge = getEstadoBadge(caso.estado);
            const tipoIcono = getTipoIcono(caso.tipoCaso);
            const claseFila = `tr-${caso.estado.toLowerCase()}`;

            //validacion para mostrar mensaje de cerrar solo si no esta cerrado
            const puedeCerrar = caso.estado.toLowerCase() !== "cerrado";
            const puedeEditar = caso.estado.toLowerCase() !== "cerrado";
            const puedeEliminar = caso.estado.toLowerCase() !== "cerrado";
            const row = `
            <tr class="${claseFila}">
                <td>${caso.id}</td>
                <td>${caso.titulo}</td>
                <td>${estadoBadge}</td>
                <td>${tipoIcono}${caso.tipoCaso}</td>
                <td>${caso.nombreCliente || 'No Client'}</td>
                <td>${new Date(caso.fechaCreacion).toLocaleDateString()}</td>
                <td class="text-nowrap">
                   <button class="btn btn-sm btn-outline-light me-1 btn-ver" data-id="${caso.id}" title="Ver">
                   <i class="bi bi-eye-fill"></i>
                    </button>
                    ${puedeEditar ? `
                      <button class="btn btn-sm btn-outline-warning" title="Editar">
                         <i class="bi bi-pencil-fill"></i>
                      </button>` : ""}

                                  ${puedeEliminar ? `
                   <button class="btn btn-sm btn-outline-danger btn-eliminar" data-id="${caso.id}" title="Eliminar">
                       <i class="bi bi-trash-fill"></i>
                   </button>` : ""}

                      ${puedeCerrar ? `
                     <button class="btn btn-sm btn-outline-secondary btn-cerrar" data-id="${caso.id}" title="Cerrar">
                       <i class="bi bi-lock-fill"></i>
                     </button>` : ""}
                </td>
            </tr>
        `;
            tbody.innerHTML += row;
        });

        /**➡️ Limpia y vuelve a renderizar la tabla de casos con animación suave (opacity).
➡️ Inserta HTML dinámico fila por fila */



        setTimeout(() => {
            tbody.classList.remove("opacity-0");
            tbody.classList.add("opacity-100");
        }, 50);
    }
    function renderizarPaginacion(paginaActual, totalPaginas) {
        const paginacion = document.getElementById("paginacion");
        paginacion.innerHTML = "";

        if (totalPaginas <= 1) return;

        const crearItem = (label, page, disabled = false, active = false) => {
            const li = document.createElement("li");
            li.className = `page-item ${disabled ? "disabled" : ""} ${active ? "active" : ""}`;
            li.innerHTML = `<a class="page-link" href="#" data-page="${page}">${label}</a>`;
            return li;
        };

        paginacion.appendChild(crearItem("Anterior", paginaActual - 1, paginaActual === 1));

        for (let i = 1; i <= totalPaginas; i++) {
            paginacion.appendChild(crearItem(i, i, false, i === paginaActual));
        }
        paginacion.appendChild(crearItem("Siguiente", paginaActual + 1, paginaActual === totalPaginas));
    }
    function actualizarResumen(resumen) {
        document.getElementById("totalCasos").textContent = resumen.total;
        document.getElementById("casosPendientes").textContent = resumen.pendientes;
        document.getElementById("casosResueltos").textContent = resumen.resueltos;
    }
    /**➡️ Muestra totales y contadores en la parte superior del dashboard.
     * 
 */
    function mostrarMensajeInformativo(mostrados, total) {
        const estadoTabla = document.getElementById("contadorResultados");
        if (!estadoTabla) return;
        estadoTabla.textContent = `Mostrando ${mostrados} de ${total} casos`;
    }

    /** ℹ️ Mensaje inferior: “Mostrando X de Y casos”*/
    function mostrarDetalleCaso(id) {
        fetch(`${apiUrl}/${id}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(res => {
                if (!res.ok) throw new Error("Error al obtener detalle");
                return res.json();
            })
            .then(data => {
                document.getElementById("detalle-titulo").innerText = data.titulo;
                document.getElementById("detalle-descripcion").innerText = data.descripcion;
                document.getElementById("detalle-estado").innerText = data.estado;
                document.getElementById("detalle-tipo").innerText = data.tipoCaso;
                document.getElementById("detalle-cliente").innerText = data.nombreCliente;
                document.getElementById("detalle-fecha").innerText = new Date(data.fechaCreacion).toLocaleDateString();

                 const grupoMotivo = document.getElementById("grupo-motivo-cierre");
                const motivoEl = document.getElementById("detalle-motivo-cierre");
                if (data.estado.toLowerCase() === "cerrado" && data.motivoCierre) {
                motivoEl.innerText = data.motivoCierre;
                grupoMotivo.classList.remove("d-none");
            } else {
                grupoMotivo.classList.add("d-none");
            }


                const modal = new bootstrap.Modal(document.getElementById('modalDetalle'));
                modal.show();
            })
            .catch(error => {
                console.error("Error al cargar detalle:", error);
                alert("No se pudo cargar el detalle del caso.");
            });
    }

    /**➡️ Trae los datos de un caso por ID.
➡️ Rellena un modal Bootstrap para mostrar info detallada */

    function getEstadoBadge(estado) {
        switch (estado.toLowerCase()) {
            case "pendiente":
                return '<span class="badge estado-pendiente">Pendiente</span>';
            case "enproceso":
                return '<span class="badge estado-enproceso">En Proceso</span>';
            case "cerrado":
                return '<span class="badge estado-cerrado">Cerrado</span>';
            default:
                return '<span class="badge bg-secondary badge-estado">' + estado + '</span>';
        }
    }

    /**➡️ Devuelven HTML para mostrar el estado y tipo con íconos y colores personalizados. */
    function getTipoIcono(tipo) {
        switch (tipo.toLowerCase()) {
            case "laboral":
                return '<i class="bi bi-people text-primary me-2"></i>';
            case "familia":
                return '<i class="bi bi-house-heart text-success me-2"></i>';
            case "civil":
                return '<i class="bi bi-bank text-info me-2"></i>';
            case "penal":
                return '<i class="bi bi-shield-exclamation text-danger me-2"></i>';
            default:
                return '<i class="bi bi-folder2-open text-secondary me-2"></i>';
        }
    }

    // 🔁 Función reutilizable para mostrar errores claros al usuario
    async function mostrarErrorDesdeResponse(respuesta, mensajePorDefecto) {
        try {
            const errorJson = await respuesta.json();
            const mensaje = errorJson.detail || mensajePorDefecto;

            Swal.fire({
                icon: 'warning',
                title: 'Error',
                text: mensaje,
            });
        } catch {
            Swal.fire({
                icon: 'error',
                title: 'Error inesperado',
                text: mensajePorDefecto,
            });
        }
    }
    // ===============================
// ✅ VALIDACIÓN INLINE (helpers)
// ===============================
function clearInvalid(form) {
  if (!form) return;
  form.querySelectorAll(".is-invalid").forEach(el => el.classList.remove("is-invalid"));
  form.querySelectorAll(".invalid-feedback").forEach(fb => (fb.textContent = ""));
}

function setInvalid(inputEl, message) {
  if (!inputEl) return;
  inputEl.classList.add("is-invalid");
  const feedback = inputEl.closest(".mb-3")?.querySelector(".invalid-feedback");
  if (feedback) feedback.textContent = message;
}

function validateCasoForm() {
  const form = document.getElementById("formGestionCaso");
  const tituloEl = document.getElementById("form-titulo");
  const descEl = document.getElementById("form-descripcion");
  const tipoEl = document.getElementById("form-tipo");
  const clienteEl = document.getElementById("form-cliente");

  clearInvalid(form);

  const titulo = tituloEl.value.trim();
  const descripcion = descEl.value.trim();
  const tipoCaso = tipoEl.value;
  const clienteId = parseInt(clienteEl.value, 10);

  let ok = true;

  if (!titulo) { setInvalid(tituloEl, "Debes ingresar un título."); ok = false; }
  else if (titulo.length < 5 || titulo.length > 150) { setInvalid(tituloEl, "Debe tener entre 5 y 150 caracteres."); ok = false; }

  if (!descripcion) { setInvalid(descEl, "Debes ingresar una descripción."); ok = false; }
  else if (descripcion.length > 5000) { setInvalid(descEl, "No puede superar 5000 caracteres."); ok = false; }

  if (!tipoCaso) { setInvalid(tipoEl, "Debes seleccionar un tipo de caso."); ok = false; }

  if (!Number.isInteger(clienteId) || clienteId < 1) { setInvalid(clienteEl, "Debes seleccionar un cliente válido."); ok = false; }

  return ok;
}


    document.getElementById("logoutBtn")?.addEventListener("click", () => {
        localStorage.removeItem("jwt_token");
        localStorage.removeItem("usuario_actual");
        sessionStorage.removeItem("demoContext"); 
        window.location.href = "login.html";
    });

    // Limpia el token y usuario del almacenamiento, redirige al login. **/
    document.addEventListener("click", async (e) => {
        // 👁 Ver detalle

        if (e.target.closest(".btn-ver")) {
            const btn = e.target.closest(".btn-ver");
            const id = btn.dataset.id;
            mostrarDetalleCaso(id);
        }

        // ✏️ EDITAR
    if (e.target.closest(".btn-outline-warning")) {
        const row = e.target.closest("tr");
        const id = row.children[0].textContent;
        const estado = row.children[2].innerText.trim().toLowerCase();

        if (estado === "cerrado") {
            Swal.fire({
                icon: "info",
                title: "No editable",
                text: "Este caso está cerrado y no puede ser modificado.",
            });
            return;
        }

          try {
    const res = await fetch(`${apiUrl}/${id}`, {
        headers: { "Authorization": `Bearer ${token}` }
    });

    if (!res.ok) {
        throw new Error("No se pudo obtener el caso");
    }

    const data = await res.json();
    // Inputs del formulario (OBLIGATORIO)
const tituloInput = document.getElementById("form-titulo");
const tipoSelect = document.getElementById("form-tipo");
const clienteSelect = document.getElementById("form-cliente");
const descripcionInput = document.getElementById("form-descripcion");

// Reset previo (muy importante)
[tituloInput, tipoSelect, clienteSelect, descripcionInput].forEach(el => {
    el.disabled = false;
});

   const rol = obtenerRolEfectivo(obtenerRolesDesdeJWT());

// Reset previo (importante)
[tituloInput, tipoSelect, clienteSelect, descripcionInput].forEach(el => {
    el.disabled = false;
});

//REGLAS SEGÚN ESTADO + ROL
if (rol === "Abogado") {

    // Cliente nunca editable por abogado
    clienteSelect.disabled = true;

    if (data.estado === "EnProceso") {
        tipoSelect.disabled = true;
        tituloInput.disabled = true; //NUEVO: título no editable en EnProceso
    }

    if (data.estado === "Cerrado") {
        tituloInput.disabled = true;
        tipoSelect.disabled = true;
        clienteSelect.disabled = true;
        descripcionInput.disabled = true;

        // Ocultar guardar
        document.querySelector("#formGestionCaso button[type='submit']")
            .style.display = "none";
    }
}
    document.getElementById("form-id").value = data.id;
    document.getElementById("form-titulo").value = data.titulo;
    document.getElementById("form-descripcion").value = data.descripcion;
    //cargar clientes ANTES de asignar valor
    await cargarClientes();
    //asignar SOLO el ID
    clienteSelect.value = data.clienteId;
    //no editable
    document.getElementById("grupo-cliente").style.display = "block";
// ESTADO (solo informativo)
const estadoSpan = document.getElementById("form-estado");
// Si viene Pendiente, visualmente pasa a EnProceso
const estadoVisual =
    data.estado === "Pendiente"
        ? "EnProceso"
        : data.estado;

estadoSpan.textContent = estadoVisual;
estadoSpan.className = "badge";

if (estadoVisual === "Pendiente") {
    estadoSpan.classList.add("estado-pendiente");
} else if (estadoVisual === "EnProceso") {
    estadoSpan.classList.add("estado-enproceso");
} else if (estadoVisual === "Cerrado") {
    estadoSpan.classList.add("estado-cerrado");
}

    // -------- TIPO --------
    document.getElementById("form-tipo").value = data.tipoCaso;
    document.getElementById("modalGestionCasoLabel").textContent = "✏️ Editar Caso";
    new bootstrap.Modal(document.getElementById("modalGestionCaso")).show();

} catch (err) {
    console.error("❌ Error al cargar caso:", err);
    Swal.fire("Error", "No se pudo cargar el caso", "error");
}

        }
     
        // 🗑️ Eliminar con SweetAlert (esto va *fuera* del bloque de editar)
        if (e.target.closest(".btn-eliminar")) {
            const btn = e.target.closest(".btn-eliminar");
            const id = btn.dataset.id;

            // 🔐 Validación local: no permitir eliminar si ya está cerrado
            const fila = btn.closest("tr");
            const estado = fila.children[2].innerText.trim().toLowerCase(); // Columna de estado

            if (estado === "cerrado") {
                Swal.fire({
                    icon: "warning",
                    title: "No se puede eliminar",
                    text: "Este caso está cerrado y no puede ser eliminado.",
                });
                return; // ⚠️ No seguimos con el fetch
            }

            // ✅ Confirmación visual
            Swal.fire({
                title: '¿Estás seguro?',
                text: "Esta acción eliminará el caso permanentemente.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then(async (result) => {
                if (result.isConfirmed) {
                    try {
                        Swal.fire({
                            title: 'Eliminando...',
                            allowOutsideClick: false,
                            didOpen: () => {
                                Swal.showLoading();
                            }
                        });

                        const res = await fetch(`${apiUrl}/${id}`, {
                            method: "DELETE",
                            headers: { "Authorization": `Bearer ${token}` }
                        });

                        if (!res.ok) {
                            const errorJson = await res.json();
                            const mensajeError = errorJson.detail || "Error inesperado";
                            // ⚠️ Mostrar mensaje personalizado según tipo de error
                            if (res.status === 400 || res.status === 404 || res.status === 409) {
                                Swal.fire({
                                    icon: 'warning',
                                    title: 'No se pudo eliminar',
                                    text: mensajeError,
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error inesperado',
                                    text: 'Ocurrió un problema al intentar eliminar el caso.',
                                });
                            }

                            return; // detenemos el flujo
                        }
                        Swal.fire({
                            toast: true,
                            position: 'top-end',
                            icon: 'success',
                            title: 'Caso eliminado exitosamente',
                            showConfirmButton: false,
                            timer: 2000,
                            timerProgressBar: true
                        });

                        await cargarCasosDesdeBackend();
                    } catch (error) {
                        Swal.fire('Error', error.message, 'error');
                    }
                }
            });
        }
        // 🔒 Cerrar caso
        if (e.target.closest(".btn-cerrar")) {
            const btn = e.target.closest(".btn-cerrar");
            const id = btn.dataset.id;
            // 🔄 Preguntar motivo de cierre
            const { value: motivo } = await Swal.fire({
                title: "Cerrar caso",
                input: "textarea",
                inputLabel: "Motivo del cierre (opcional)",
                inputPlaceholder: "Escribe el motivo si corresponde...",
                inputAttributes: {
                    "aria-label": "Motivo de cierre"
                },
                showCancelButton: true,
                confirmButtonText: "Cerrar caso",
                cancelButtonText: "Cancelar"
            });
            if (motivo === undefined) return; // Usuario canceló
            try {
                Swal.fire({
                    title: "Cerrando caso...",
                    allowOutsideClick: false,
                    didOpen: () => Swal.showLoading()
                });

                const res = await fetch(`${apiUrl}/${id}/cerrar`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify({ motivoCierre: motivo })
                });

                if (!res.ok) {
                    const errorJson = await res.json();
                    throw new Error(errorJson.detail || "Error inesperado");
                }

                await cargarCasosDesdeBackend();

                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Caso cerrado con éxito',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });

            } catch (error) {
                console.error("Error al cerrar caso:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error al cerrar",
                    text: error.message || "No se pudo cerrar el caso."
                });
            }
        }
    });
      
    document.getElementById("btnNuevoCaso")?.addEventListener("click",async () => {

        // Limpiar el formulario antes de abrir
        // Limpiar opciones previas por si viene de modo edición

        document.getElementById("formGestionCaso").reset();
        // ✅ Reset visual de validaciones inline (paso 1)
clearInvalid(document.getElementById("formGestionCaso"));

// ✅ Asegurar que el botón Guardar siempre vuelva (fix submit oculto en edición Cerrado)
const submitBtn = document.querySelector('button[form="formGestionCaso"][type="submit"]');
if (submitBtn) {
    submitBtn.style.display = ""; // vuelve a mostrarse si estaba oculto
    submitBtn.disabled = false;   // por si quedó bloqueado
    submitBtn.innerHTML = '<i class="bi bi-save me-1"></i> Guardar Cambios';
}

// ✅ Asegurar que los campos vuelvan habilitados (nuevo caso = editable)
["form-titulo", "form-descripcion", "form-tipo", "form-cliente"].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.disabled = false;
});

        document.getElementById("form-id").value = ""; // dejar vacío para saber que es nuevo
        // 🆕 Limpiar campo cliente (solo visual)
        document.getElementById("form-cliente").value = "";
              
   
            const clienteSelect = document.getElementById("form-cliente");
            clienteSelect.disabled = false;   
            clienteSelect.innerHTML = `<option value="">-- Seleccione cliente --</option>`;
            await cargarClientes();                   // ✅ CLAVE// ✅ select, no readonly
        
        document.getElementById("grupo-cliente").style.display = "block"; // ✅ Mostrar campo
            // 🔧 ESTADO VISUAL POR DEFECTO (NUEVO CASO)
        const estadoSpan = document.getElementById("form-estado");
        estadoSpan.textContent = "Pendiente";
        estadoSpan.className = "badge estado-pendiente";


        // Actualizar el título del modal
        document.getElementById("modalGestionCasoLabel").textContent = "📝 Nuevo Caso";
        // Mostrar el modal
        const modal = new bootstrap.Modal(document.getElementById("modalGestionCaso"));
        modal.show();
    });

    document.getElementById("formGestionCaso")?.addEventListener("submit", async (e) => {
        e.preventDefault();
        // ===============================
        // ⏳ Loading state (Paso 4)
        // ===============================
        const submitBtn = document.querySelector(
          'button[form="formGestionCaso"][type="submit"]'
        );

      if (submitBtn) {
    submitBtn.disabled = true;
    submitBtn.innerHTML = "Guardando...";
}
        // VALIDACIÓN INLINE
    if (!validateCasoForm()) {
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="bi bi-save me-1"></i> Guardar Cambios';
        return;
    }



        const id = document.getElementById("form-id").value.trim();
        const titulo = document.getElementById("form-titulo").value.trim();
        const descripcion = document.getElementById("form-descripcion").value.trim();
        const tipoCaso = document.getElementById("form-tipo").value;

        const clienteId = parseInt(
            document.getElementById("form-cliente").value,
            10
        );  

       
        const caso = {
            Titulo: titulo,
            Descripcion: descripcion,
            TipoCaso: tipoCaso,
            ClienteId: clienteId
        };

        const esNuevo = id === "";

        const url = esNuevo ? "https://localhost:7266/api/Casos" : `https://localhost:7266/api/Casos/${id}`;
        const metodo = esNuevo ? "POST" : "PUT";

            let response; 
            try {
    const response = await fetch(url, {
        method: metodo,
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(caso)
    });

    if (!response.ok) {
        throw response; // ⬅️ NO lo parses aquí
    }

    // ✅ ÉXITO
    bootstrap.Modal.getInstance(document.getElementById("modalGestionCaso")).hide();

    // ✅ Recarga la tabla
    await cargarCasosDesdeBackend();

    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: 'success',
        title: esNuevo ? "Caso creado con éxito" : "Caso actualizado con éxito",
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true
    });

} catch (error) {
    console.error("❌ Error al guardar caso:", error);

    // ✅ Error backend (viene como Response)
    if (error instanceof Response) {
        const msg = await mostrarErrorDesdeResponse(
            error,
            esNuevo
                ? "No se pudo crear el caso."
                : "No se pudo actualizar el caso."
        );

        // 🎯 REGLA DE NEGOCIO ESPECÍFICA (si el backend lo dice)
        if (msg && msg.toLowerCase().includes("cliente ya tiene")) {
            Swal.fire({
                icon: 'warning',
                title: 'No se puede cambiar el cliente',
                text: 'Este cliente ya tiene un caso activo. Para modificar el cliente debes ingresar como Administrador.',
            });
        }

    } else {
        // ✅ Error de conexión / JS
        Swal.fire({
            icon: 'error',
            title: 'Error de conexión',
            text: error.message || 'No se pudo conectar con el servidor.',
        });
    }

} finally {
    // 🔚 SIEMPRE restaurar botón (éxito o error)
    submitBtn.disabled = false;
    submitBtn.innerHTML = '<i class="bi bi-save me-1"></i> Guardar Cambios';
}
        

    });
});
// ===============================
// 🔹 CONTROLADOR DE MÓDULOS (NUEVO)
// ===============================
const MODULOS = [
    //'mod-dashboard',
    'mod-casos',
    'mod-usuarios',
    'mod-roles'
];
function mostrarModulo(id) {
    MODULOS.forEach(m => {
        const el = document.getElementById(m);
        if (el) el.classList.add('d-none');
    });

    const activo = document.getElementById(id);
    if (activo) activo.classList.remove('d-none');
}
// ===============================
// 🔹 LOADER POR MÓDULO (PASO 1)
// ===============================
function onModuloCargado(moduloId) {
  switch (moduloId) {
    case "mod-casos":
      if (typeof cargarCasosDesdeBackend === "function") {
        cargarCasosDesdeBackend();
      }
      break;

    case "mod-usuarios":
      if (typeof cargarUsuariosDesdeBackend === "function") {
        cargarUsuariosDesdeBackend();
      }
      break;
  }
}

// ===============================
// 🔹 SIDEBAR (UX / NAVEGACIÓN)
// ===============================
function toggleSidebar() {
    document.getElementById("sidebar")
        ?.classList.toggle("collapsed");
}


async function cargarClientes() {
    const response = await fetch("https://localhost:7266/api/Clientes", {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    const clientes = await response.json();
    const select = document.getElementById("form-cliente");

    clientes.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;          // 👈 Id de la BD
        option.textContent = c.nombre; // 👈 Nombre visible
        select.appendChild(option);
    });
}


