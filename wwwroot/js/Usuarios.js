function setFieldError(inputId, message) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.classList.add("is-invalid");
    input.classList.remove("is-valid");

    const feedback = input.parentElement?.querySelector(".invalid-feedback");
    if (feedback) feedback.textContent = message ?? "Campo inválido";
}

function clearFieldError(inputId) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.classList.remove("is-invalid");
    const feedback = input.parentElement?.querySelector(".invalid-feedback");
    if (feedback) feedback.textContent = "";
}

function clearUserFormErrors() {
    ["nombreUsuario", "emailUsuario", "passwordUsuario"].forEach(clearFieldError);
}


// usuarios.js
// Este módulo carga, crea, edita y elimina usuarios desde el backend usando JWT
// Punto de entrada del módulo, se llama desde dashboard.html
function initUsuariosModule() {
        document.getElementById("seccion-usuarios")?.classList.remove("d-none");
    if (!window.__usuariosInitialized) {
    window.__usuariosInitialized = true;
    configurarEventosUsuarios();
  }
    cargarUsuarios();
}
window.initUsuariosModule = initUsuariosModule;
// 🧠 Configura los listeners para botones y formularios
function configurarEventosUsuarios() {
    document.getElementById("btnNuevoUsuario").addEventListener("click", () => {
        abrirModalUsuario();   // Abre el modal vacío para crear usuario
    });
    document.getElementById("formUsuario").addEventListener("submit", async (e) => {
        e.preventDefault();
        await guardarUsuario(); // Enviar datos al backend (POST o PUT)
    });
     // ✅ UX pro: limpiar error al escribir
    ["nombreUsuario", "emailUsuario", "passwordUsuario"].forEach(id => {
        const input = document.getElementById(id);
        if (!input) return;

        input.addEventListener("input", () => {
            clearFieldError(id);
        });
    });
}
// 🔄 Fetch: obtener todos los usuarios
async function cargarUsuarios() {
    try {
        const response = await fetch("/api/usuarios", {
            headers: authHeader()
        });
        if (!response.ok) {
            console.error("Error HTTP:", response.status);
            return;
        }
        const text = await response.text();

        if (!text) {
            console.warn("Respuesta vacía del servidor");
            return;
        }
        const usuarios = JSON.parse(text);
        renderizarUsuarios(usuarios);
    } catch (error) {
        console.error("Error al cargar usuarios:", error);
    }
}
// 🧱 Renderiza usuarios en la tabla HTML
function renderizarUsuarios(usuarios) {
    const tbody = document.getElementById("usuariosBody");
    tbody.innerHTML = "";
    usuarios.forEach(usuario => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${usuario.id}</td>
            <td>${usuario.nombre}</td>
            <td>${usuario.email}</td>
            <td>${formatearRoles(usuario.roles)}</td>
            <td>
                <button class="btn btn-sm btn-outline-light me-1" onclick='abrirModalUsuario(${JSON.stringify(usuario)})'>✏️</button>
                <button class="btn btn-sm btn-outline-danger" onclick='eliminarUsuario(${usuario.id})'>🗑️</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}
// Convierte array de roles en badges bonitos
function formatearRoles(roles) {
    if (!roles || roles.length === 0) return "-";
    return roles.map(r => `<span class='badge text-bg-primary me-1'>${r}</span>`).join("");
}
// ✏ Abre el modal con datos (si hay) o limpio para nuevo
function abrirModalUsuario(usuario = null) {
        clearUserFormErrors();
    document.getElementById("formUsuario").reset();
    document.getElementById("usuarioId").value = usuario?.id || "";
    document.getElementById("nombreUsuario").value = usuario?.nombre || "";
    document.getElementById("emailUsuario").value = usuario?.email || "";
    // Si es edición, la contraseña no es requerida
    document.getElementById("passwordUsuario").required = !usuario;
    const modal = new bootstrap.Modal(document.getElementById("modalUsuario"));
    modal.show();
}
// 💾 Guarda o actualiza un usuario (POST o PUT)
async function guardarUsuario() {
    const id = (document.getElementById("usuarioId").value ?? "").trim();
    // 1) Leer + normalizar (trim)
    const nombre = (document.getElementById("nombreUsuario").value ?? "").trim();
    const email = (document.getElementById("emailUsuario").value ?? "").trim();
    const password = (document.getElementById("passwordUsuario").value ?? "").trim();
    const esNuevo = !id;

    // 2) Validaciones (inline, sin SweetAlert)
clearUserFormErrors();

const nombreRegex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/;
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

if (!nombre) {
    setFieldError("nombreUsuario", "El nombre es obligatorio.");
    document.getElementById("nombreUsuario").focus();
    return;
}
if (nombre.length < 3) {
    setFieldError("nombreUsuario", "El nombre debe tener al menos 3 caracteres.");
    document.getElementById("nombreUsuario").focus();
    return;
}
if (!nombreRegex.test(nombre)) {
    setFieldError("nombreUsuario", "El nombre solo puede contener letras y espacios.");
    document.getElementById("nombreUsuario").focus();
    return;
}

if (!email) {
    setFieldError("emailUsuario", "El email es obligatorio.");
    document.getElementById("emailUsuario").focus();
    return;
}
if (!emailRegex.test(email)) {
    setFieldError("emailUsuario", "Email inválido.");
    document.getElementById("emailUsuario").focus();
    return;
}

if (esNuevo) {
    if (!password) {
        setFieldError("passwordUsuario", "La contraseña es obligatoria.");
        document.getElementById("passwordUsuario").focus();
        return;
    }
    if (password.length < 6) {
        setFieldError("passwordUsuario", "La contraseña debe tener al menos 6 caracteres.");
        document.getElementById("passwordUsuario").focus();
        return;
    }
} else {
    if (password && password.length < 6) {
        setFieldError("passwordUsuario", "La contraseña debe tener al menos 6 caracteres.");
        document.getElementById("passwordUsuario").focus();
        return;
    }
}




    // ✅ Primero crea el objeto
    const payload = { nombre, email };
    // ✅ Luego agrega la contraseña solo si se escribió
    if (password && password.trim() !== "") {
        payload.password = password.trim();
    }
    const url = id ? `/api/usuarios/${id}` : "/api/usuarios";
    const method = id ? "PUT" : "POST";

    // 🔒 UX pro: deshabilitar botón Guardar y mostrar spinner
const form = document.getElementById("formUsuario");
const btnSubmit = document.querySelector('button[type="submit"][form="formUsuario"]');

if (!btnSubmit) {
    console.warn("No se encontró el botón submit del formulario formUsuario.");
    return;
}
    btnSubmit.disabled = true;
    const originalHtml = btnSubmit.innerHTML;
    btnSubmit.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Guardando...';

    try {
        const res = await fetch(url, {
            method,
            headers: {
                ...authHeader(),
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!res.ok) throw new Error("Error al guardar usuario");

        Swal.fire("Éxito", id ? "Usuario actualizado" : "Usuario creado", "success");
        bootstrap.Modal.getInstance(document.getElementById("modalUsuario")).hide();
        cargarUsuarios();
    } catch (error) {
        console.error(error);
        Swal.fire("Error", error.message, "error");
    } finally{{
         // 🔓 Restaurar botón siempre (éxito o error)
        btnSubmit.disabled = false;
        btnSubmit.innerHTML = originalHtml;
    }}
}
// ❌ Elimina un usuario después de confirmar con SweetAlert2
async function eliminarUsuario(id) {
    const confirm = await Swal.fire({
        title: "¿Eliminar usuario?",
        text: "Esta acción no se puede deshacer",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    });
    if (!confirm.isConfirmed) return;
    try {
        const res = await fetch(`/api/usuarios/${id}`, {
            method: "DELETE",
            headers: authHeader()
        });
        if (!res.ok) throw new Error("Error al eliminar usuario");
        Swal.fire("Eliminado", "El usuario fue eliminado", "success");
        cargarUsuarios();
    } catch (error) {
        console.error(error);
        Swal.fire("Error", error.message, "error");
    }
}
// 🔐 Adjunta el JWT al header Authorization
function authHeader() {
    const token = localStorage.getItem("jwt_token");
    return {
        "Authorization": `Bearer ${token}`
    };
}
// 🔄 Exportar función de arranque para dashboard.html
window.initUsuariosModule = initUsuariosModule;
