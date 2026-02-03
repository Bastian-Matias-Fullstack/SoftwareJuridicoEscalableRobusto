function mostrarError(texto) {
    const mensajeError = document.getElementById("mensajeError");
    mensajeError.innerText = texto;
    mensajeError.style.display = "block";
    mensajeError.classList.remove("shake"); // Reiniciar animación
    void mensajeError.offsetWidth;          // Trigger reflow
    mensajeError.classList.add("shake");    // Aplicar animación
}
// ===============================
// Demo Context Handling (SE2)
// ===============================
function getDemoContextFromUrl() {
    const params = new URLSearchParams(window.location.search);
    const demo = (params.get("demo") || "").toLowerCase();

    const allowed = ["casos", "roles", "usuarios"];
    return allowed.includes(demo) ? demo : null;
}

const demoContext = getDemoContextFromUrl();

if (demoContext) {
    sessionStorage.setItem("demoContext", demoContext);
} else {
    sessionStorage.removeItem("demoContext");
}


document.getElementById("loginForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const mensajeError = document.getElementById("mensajeError");

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!email || !password) {
        mostrarError("Completa todos los campos.");

        return;
    }

    if (!emailRegex.test(email)) {
        mostrarError("Ingresa un correo válido.");

        return;
    }

    const loginData = {
        email: email,
        password: password
    };

    try {
        const response = await fetch("https://localhost:7266/api/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(loginData)
        });

        if (!response.ok) {
            throw new Error("Login failed: " + response.status);
        }

      /*  const data = await response.json();*/
        const data = await response.json();
        const token = data.token;
        localStorage.setItem("jwt_token", token);
       
        // ✅ Decodificar manualmente
        const payloadBase64 = token.split('.')[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);


        const usuario = {
            email: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || "",
            nombre: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "Usuario",
            rol: payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || []
        };

        localStorage.setItem("usuario_actual", JSON.stringify(usuario));

        //// Guarda datos del usuario (puedes expandir esto según lo que devuelva tu backend)
        //const usuario = {
        //    email: data.email,
        //    nombre: data.nombre || "Usuario",
        //    rol: data.rol
        //};
        //localStorage.setItem("usuario_actual", JSON.stringify(usuario));

        // Redirigir inmediatamente

        window.location.href = "dashboard.html";



    } catch (error) {
        console.error("Login error:", error);
        mostrarError("Correo o contraseña inválidos.");


    }
});
// Ocultar mensaje de error al escribir nuevamente
document.getElementById("email").addEventListener("input", () => {
    document.getElementById("mensajeError").style.display = "none";
});
document.getElementById("password").addEventListener("input", () => {
    document.getElementById("mensajeError").style.display = "none";
});

