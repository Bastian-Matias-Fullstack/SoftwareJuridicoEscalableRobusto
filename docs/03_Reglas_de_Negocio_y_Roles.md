# 03 – Reglas de Negocio y Roles

## 🎯 Objetivo de este documento

Este documento describe **las reglas de negocio reales** de LegalApp y el **modelo de roles** que gobierna el sistema.

Es el documento más importante del proyecto porque demuestra:
- criterio de ingeniería
- entendimiento del dominio
- protección de datos
- decisiones conscientes de diseño

> En LegalApp, **las reglas viven en backend** y se validan siempre.

---

## 👥 Roles del sistema

LegalApp soporta tres roles principales:

### 🟢 Admin
- Acceso total al sistema
- Gestión de Casos
- Gestión de Usuarios
- Gestión de Roles

### 🟡 Abogado
- Gestión de Casos
- Acciones limitadas según estado del caso
- Sin acceso a Usuarios ni Roles

### 🔵 Soporte
- Gestión de Usuarios
- Sin acceso a Casos
- Sin acceso a Roles

> El rol efectivo se obtiene desde el JWT y es validado en backend.

---

## ⚖️ Entidad Caso – Estados

Un **Caso Jurídico** puede estar en uno de los siguientes estados:

- **Pendiente** (estado inicial automático)
- **EnProceso**
- **Cerrado**

### Regla fundamental
El **estado del caso no es editable libremente** desde formularios.

---

## 🧩 Reglas de negocio clave

### 1️⃣ Creación de Caso
- El estado **siempre inicia en `Pendiente`**
- El frontend **no puede enviar el estado**
- El backend fuerza el estado inicial

Violaciones:
- Contrato inválido → `400`
- Regla de negocio → `409`

---

### 2️⃣ Edición de Caso
- No se puede editar un caso **Cerrado**
- No se puede cambiar el estado desde este flujo
- Campos permitidos dependen del rol y estado

Violaciones:
- Caso no existe → `404`
- Caso cerrado → `409`

---

### 3️⃣ Cierre de Caso
- El cierre tiene endpoint dedicado
- Un caso **no puede cerrarse dos veces**
- Una vez cerrado, el caso es inmutable

Violaciones:
- Cierre repetido → `409`

---

### 4️⃣ Eliminación de Caso
- Un caso Cerrado **no puede eliminarse**
- El backend valida siempre el estado

Violaciones:
- Eliminación inválida → `409`

---

### 5️⃣ Regla Cliente – Caso Activo
- Un cliente **no puede tener más de un caso activo**
- Esta regla se valida en backend
- Previene duplicidad y conflictos

Violaciones:
- Cliente con caso activo → `409`

---

## 🔐 Reglas por rol (resumen)

### Abogado
- ❌ No puede editar cliente
- ❌ No puede modificar estado
- ❌ No puede eliminar casos cerrados
- ❌ No puede reabrir casos

### Admin
- Acceso más amplio
- Aún así, respeta reglas duras del dominio

### Soporte
- No interactúa con Casos

---

## 🚫 Qué NO se permite (explícito)

- Cambiar estado por formulario genérico
- Saltarse reglas desde frontend
- Confiar en visibilidad UI como seguridad
- Generar 500 para reglas esperadas

---

## 🧪 Traducción a errores HTTP

| Situación                       | Código |
|--------------------------------|--------|
| Contrato inválido              | 400    |
| Entidad no encontrada          | 404    |
| Regla de negocio violada       | 409    |
| No autenticado / no autorizado | 401/403 |
| Error inesperado               | 500    |

Estos códigos son **consistentes en toda la API**.

---

## 🧠 Decisiones clave

- Backend como autoridad absoluta
- Reglas expresadas como excepciones de dominio
- Frontend solo bloquea por UX
- Estados protegidos
- Errores previsibles y controlados

Estas decisiones **no son accidentales**.

---

## 🧾 Nota final

Este documento define **cómo se comporta LegalApp**.

Cualquier cambio futuro debe respetar estas reglas o ser explícitamente documentado.
