# 01 – Contexto General de LegalApp

## 🎯 Propósito del proyecto

**LegalApp** es una aplicación web de gestión de casos jurídicos diseñada como un **proyecto de portafolio profesional nivel Mid‑Senior Fullstack .NET**.

El objetivo principal **no es construir un CRUD académico**, sino demostrar capacidad real para:

- Diseñar y aplicar **arquitectura limpia**
- Implementar **reglas de negocio reales**
- Controlar **roles y permisos**
- Manejar errores de forma consistente (sin 500 indebidos)
- Pensar el sistema con **criterio de producción**
- Preparar una aplicación para **demos públicas y despliegue cloud**

LegalApp está pensada para **ser explicada, defendida y evaluada en entrevistas técnicas**.

---

## 🧠 Enfoque profesional

Este proyecto fue desarrollado bajo los siguientes principios:

- **El backend es la fuente de verdad**
- El frontend **no es seguridad**, solo UX
- Las reglas de negocio viven en la capa Application
- Los controllers son delgados
- Los errores esperados **no generan 500**
- La configuración depende del ambiente (no del IDE)
- La documentación refleja el **estado real del proyecto**, no un ideal

Este enfoque replica el trabajo esperado de un desarrollador **Mid‑Senior en entornos reales**.

---

## 🧱 Alcance funcional

### Módulos principales

- **Casos Jurídicos**
- **Usuarios**
- **Roles**

### Funcionalidades clave

- CRUD completo de Casos
- Control de estados del Caso
- Reglas de negocio por rol
- Autenticación JWT
- Autorización por roles
- Manejo global de errores
- Dashboard frontend por rol

El módulo **Casos** se encuentra **cerrado y validado**.  
Los módulos **Usuarios** y **Roles** están funcionales, con QA pendiente documentado.

---

## 👥 Roles del sistema

El sistema soporta los siguientes roles:

- **Admin**
  - Acceso completo
  - Gestión de Casos, Usuarios y Roles

- **Abogado**
  - Gestión de Casos
  - Restricciones según estado del caso

- **Soporte**
  - Gestión de Usuarios
  - Sin acceso a Casos ni Roles

> ⚠️ Importante:  
> La visibilidad en frontend es solo UX.  
> **El backend valida siempre los permisos.**

---

## 🧩 Reglas de negocio (visión general)

Algunas reglas clave que definen el sistema:

- Un Caso inicia siempre en estado **Pendiente**
- El estado **no se puede modificar desde formularios genéricos**
- Los Casos **Cerrados son inmutables**
- Un Cliente no puede tener más de un Caso activo
- Las violaciones de reglas devuelven **409 Conflict**
- Entidades inexistentes devuelven **404 Not Found**
- Contratos inválidos devuelven **400 Bad Request**

Estas reglas están implementadas **en backend**, no en frontend.

---

## 🧪 Calidad y QA

La calidad del sistema se asegura mediante:

- Tests automatizados (Application + Domain)
- QA manual por módulo y rol
- Validación explícita de status codes
- Eliminación de try/catch innecesarios en controllers
- Uso de excepciones de dominio
- Middleware global de manejo de errores (`ProblemDetails`)

El objetivo es **previsibilidad**, no solo funcionamiento.

---

## 🌐 Relación con el portafolio

LegalApp forma parte de un **ecosistema de portafolio profesional**, donde:

- El backend se despliega de forma independiente
- El frontend Angular del portafolio consume la API
- Se exponen demos reales por rol
- Se evita cualquier dependencia a `localhost`

Esto permite:
- Demos públicas
- Evaluación técnica real
- Simulación de entorno productivo

---

## 🚀 Estado actual del proyecto

- ✅ Backend estable
- ✅ Tests pasando
- ✅ Arquitectura definida
- ✅ Reglas de negocio claras
- ✅ Módulo Casos cerrado
- ⏳ QA pendiente en Usuarios y Roles (documentado)
- ⏳ Despliegue cloud en progreso

---

## 📌 Nota final

Este documento define **el contexto base de LegalApp**.

Los detalles técnicos específicos (arquitectura, QA, frontend, despliegue) se desarrollan en los documentos siguientes dentro de la carpeta `/docs`.

Este archivo debe leerse **antes de revisar cualquier otro documento**.
