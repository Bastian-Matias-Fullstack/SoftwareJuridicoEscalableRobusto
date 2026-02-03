// ===========================
// 🔁 FUNCIONES AUXILIARES DE CARGA
// ===========================
let choicesUsuarios;
let choicesRoles;
const apiBase = "/api";

async function cargarUsuarios() {
    const res = await fetch(`${apiBase}/usuarios`, {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("jwt_token")}`
        }
    });

    const usuarios = await res.json();
    // Usa Choices.js para setear opciones
    choicesUsuarios.clearChoices();
    choicesUsuarios.setChoices(
        usuarios.map(u => ({ value: u.id, label: u.nombre })),
        'value',
        'label',
        true
    );
}


async function cargarRoles() {
    const res = await fetch(`${apiBase}/roles`, {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("jwt_token")}`
        }
    });

    const roles = await res.json();

    choicesRoles.clearChoices();
    choicesRoles.setChoices(
        roles.map(r => ({ value: r.nombre, label: r.nombre })),
        'value',
        'label',
        true
    );
}


async function cargarRolesAsignados(usuarioId) {
    const res = await fetch(`${apiBase}/roles/${usuarioId}`, {

        headers: {
            Authorization: `Bearer ${localStorage.getItem("jwt_token")}`
        }
    });

    const roles = await res.json();
    const lista = document.getElementById("listaRolesAsignados");
    lista.innerHTML = "";

    if (roles.length === 0) {
        lista.innerHTML = `<li class="list-group-item bg-transparent text-white">Sin roles asignados</li>`;
        return;
    }

    roles.forEach(rol => {
        lista.innerHTML += `
        <li class="list-group-item bg-transparent text-white d-flex justify-content-between align-items-center">
            ${rol.nombre}
            <button class="btn btn-sm btn-outline-danger btn-quitar-rol" data-rol="${rol.nombre}" title="Quitar rol">
                <i class="bi bi-x-circle"></i>
            </button>
        </li>
    `;
    });
}

// ===========================
// 🔐 MÓDULO DE GESTIÓN DE ROLES
// ===========================

// ⏳ Cargar roles asignados al cambiar usuario

document.getElementById("selectUsuarios")?.addEventListener("change", (e) => {
    const usuarioId = e.target.value;
    if (usuarioId) {
        cargarRolesAsignados(usuarioId);
    } else {
        document.getElementById("listaRolesAsignados").innerHTML = "";
    }
});


// ➕ Asignar rol
document.getElementById("btnAsignarRol")?.addEventListener("click", async () => {
    const selectUsuarios = document.getElementById("selectUsuarios");
    const selectRoles = document.getElementById("selectRoles");
    const usuarioId = selectUsuarios?.value;
    const nombreRol = selectRoles?.value;

    if (!usuarioId || !nombreRol) {
        Swal.fire({
            icon: "warning",
            title: "Campos requeridos",
            text: "Debes seleccionar un usuario y un rol.",
        });
        return;
    }

    try {
        const res = await fetch(`${apiBase}/roles/${usuarioId}/${nombreRol}`, {

            method: "POST",
            headers: {
                "Authorization": `Bearer ${localStorage.getItem("jwt_token")}`
            }
        });

        if (!res.ok) {
            let err = "No se pudo asignar el rol.";
            try {
                const json = await res.json();
                err = json.detail || err;
            } catch { }
            throw new Error(err);
        }

        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: 'Rol asignado correctamente',
            showConfirmButton: false,
            timer: 2000
        });

        cargarRolesAsignados(usuarioId);

    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Error",
            text: error.message || "Error inesperado",
        });
    }
});


// ❌ Quitar rol (debe ir FUERA del bloque de asignar)

    document.addEventListener("click", async (e) => {

        if (e.target.closest(".btn-quitar-rol")) {
            const btn = e.target.closest(".btn-quitar-rol");
            const rol = btn.dataset.rol;
            const usuarioId = document.getElementById("selectUsuarios").value;

            if (!rol || !usuarioId) return;

            const confirmacion = await Swal.fire({
                title: `¿Quitar rol "${rol}"?`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Sí, quitar",
                cancelButtonText: "Cancelar"
            });

            if (!confirmacion.isConfirmed) return;

            try {
                const res = await fetch(`${apiBase}/roles/${usuarioId}/${rol}`, {
                    method: "DELETE",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("jwt_token")}`
                    }
                });

                if (!res.ok) {
                    let errorMsg = "Error al quitar rol";
                    try {
                        const json = await res.json();
                        errorMsg = json.detail || errorMsg;
                    } catch {
                        // no hacer nada, ya tenemos un mensaje por defecto
                    }
                    throw new Error(errorMsg);           }

                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Rol eliminado correctamente',
                    showConfirmButton: false,
                    timer: 2000
                });

                cargarRolesAsignados(usuarioId);

            } catch (error) {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: error.message || "No se pudo quitar el rol"
                
                });
            }
        }
    });



    /* VALIDAMOS si el usuario tiene multiples roles a la vez para saber que mostrar */
function verificarAccesoPorRol(rolesPermitidos = []) {
    const token = localStorage.getItem("jwt_token");
    if (!token) {
        Swal.fire({
            icon: "error",
            title: "Sesión inválida",
            text: "Debes iniciar sesión.",
        }).then(() => {
            window.location.href = "login.html";
        });
        return false;
    }

    try {
        const payload = JSON.parse(atob(token.split(".")[1]));
        const rol = payload["role"] || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        const roles = Array.isArray(rol) ? rol : [rol];
        const tieneAcceso = roles.some(r => rolesPermitidos.includes(r));

        if (!tieneAcceso) {
            Swal.fire({
                icon: "error",
                title: "Acceso denegado",
                text: "No tienes permisos para acceder a esta sección.",
            }).then(() => {
                // 🔐 Limpieza para evitar loop infinito
                localStorage.removeItem("jwt_token");
                localStorage.removeItem("usuario_actual");
                window.location.href = "login.html";
            });
            return false;
        }

        return true;
    } catch (error) {
        console.error("Error al verificar el token:", error);
        Swal.fire({
            icon: "error",
            title: "Token inválido",
            text: "No se pudo verificar tu sesión.",
        }).then(() => {
            localStorage.clear();
            window.location.href = "login.html";
        });
        return false;
    }
}
// ===========================
// 🚀 INICIALIZACIÓN AL CARGAR
// ===========================
window.initRolesModule = function () {
    if (!verificarAccesoPorRol(["Admin"])) return;
    document.getElementById("seccion-gestion-roles")?.classList.remove("d-none");

    // Aquí va TODO lo que ya tenías en tu roles.js
    choicesUsuarios = new Choices("#selectUsuarios", {
        searchEnabled: false,
        shouldSort: false,
        itemSelectText: "",
    });

    choicesRoles = new Choices("#selectRoles", {
        searchEnabled: false,
        shouldSort: false,
        itemSelectText: "",
    });

    document.querySelector("#selectUsuarios").closest(".choices").classList.add("input-glass");
    document.querySelector("#selectRoles").closest(".choices").classList.add("input-glass");

    cargarUsuarios();
    cargarRoles();
};