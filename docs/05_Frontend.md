# 05 – Frontend (Login + Dashboard)

## 🎯 Objetivo de este documento

Este documento describe **la filosofía, arquitectura y criterios de UX del frontend de LegalApp**.
El frontend está diseñado para ser **defensivo**, coherente y alineado con reglas reales de backend.

Principio base:
> El frontend **no es la capa de seguridad**.  
> Su rol es **mejorar la experiencia de usuario**, no validar reglas de negocio finales.

---

## 🧠 Filosofía del Frontend

El frontend de LegalApp se rige por estos principios:

- UX clara y predecible
- Estados de carga visibles
- Prevención de acciones inválidas
- Mensajes no técnicos
- Reset correcto de estado UI
- Consistencia visual y funcional
- Backend como fuente de verdad

---

## 🧱 Stack Frontend

- HTML5
- CSS3 (Bootstrap 5 + estilos glassmorphism)
- JavaScript Vanilla (sin framework)
- SweetAlert2 (feedback global)
- Choices.js (mejora UX de selects)
- Fetch API con JWT

---

## 🔐 Login – Criterio Mid-Senior

### Responsabilidades del Login
- Autenticar credenciales
- Crear sesión válida (JWT)
- Redirigir correctamente al Dashboard
- Manejar errores y estados de carga

### Decisiones clave
- Validaciones inline (no popups)
- Mensajes genéricos para credenciales inválidas
- Bloqueo de submit durante request
- Manejo explícito de errores de red y servidor
- No exponer información sensible

### Qué NO hace el Login
- No autoriza por rol
- No decide permisos
- No valida reglas de negocio

---

## 🧭 Dashboard – Arquitectura General

El Dashboard funciona como una **SPA ligera**:

- Una sola vista principal
- Módulos renderizados dinámicamente
- Navegación por roles
- Control de visibilidad centralizado

### Módulos
- Casos
- Usuarios
- Roles

La visibilidad de módulos depende del **rol efectivo** obtenido desde el JWT.

---

## 👥 Visibilidad por Rol (UX)

- **Admin**: Casos, Usuarios y Roles
- **Abogado**: solo Casos
- **Soporte**: solo Usuarios

> Esta visibilidad es **UX**, no seguridad.

El backend valida siempre cada acción.

---

## ✏️ Formularios – Estándar aplicado

Todos los formularios siguen el mismo patrón:

### Validaciones
- Inline por campo
- Sin tooltips nativos HTML
- Mensajes claros y contextualizados

### Submit
- Botón deshabilitado durante envío
- Texto de estado (“Guardando…”)
- Prevención de doble submit

### Reset
- Formularios se abren siempre limpios
- No quedan estados inválidos pegados
- Botones y campos se restauran correctamente

---

## ⚠️ Manejo de errores Frontend

### Errores por campo
- Se muestran inline
- No usan SweetAlert

### Errores globales
Usan SweetAlert:
- 401 / 403 → sesión / permisos
- 409 → regla de negocio
- 500 → error inesperado
- Error de red

---

## 🧩 Integración con Backend

- JWT enviado en Authorization Header
- Endpoints consumidos por rol
- Status codes interpretados correctamente
- Mensajes de backend respetados (`detail`)

El frontend **no interpreta textos arbitrarios**.

---

## 🧪 Estado de QA Frontend

- ✅ Módulo Casos: QA cerrado
- ⏳ Módulo Usuarios: pendiente aplicar mismo estándar
- ⏳ Módulo Roles: pendiente QA

Los pendientes están documentados en `/docs/06_Estado_y_Pendientes.md`.

---

## 🧾 Nota final

Este documento define **cómo se piensa el frontend en LegalApp**.

Cualquier nuevo módulo debe respetar estos criterios
para mantener consistencia y calidad de UX.
