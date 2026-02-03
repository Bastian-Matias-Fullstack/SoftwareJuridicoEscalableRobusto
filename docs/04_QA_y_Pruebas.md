# 04 – QA y Pruebas

## 🎯 Objetivo de este documento

Este documento describe **cómo se asegura la calidad en LegalApp**, tanto a nivel backend como frontend,
y define **qué se considera un comportamiento correcto del sistema**.

El foco no es solo que el sistema funcione,
sino que **falle de forma controlada y predecible**.

---

## 🧠 Enfoque de QA

El enfoque de QA en LegalApp sigue estos principios:

- QA valida **reglas de negocio**, no solo flujos felices
- Los errores esperados **no generan 500**
- El backend siempre responde con status codes coherentes
- El frontend interpreta errores, no los inventa
- La validación existe tanto en backend como en frontend (con distintos objetivos)

---

## 🧪 Tipos de pruebas aplicadas

### 1️⃣ Tests automatizados (Backend)

Se implementan tests unitarios principalmente en:

- **Application**
- **Domain**

Características:
- Uso de mocks para repositorios
- Validación directa de reglas de negocio
- Sin dependencias de HTTP ni base de datos real

Ejemplos de pruebas:
- Crear usuario con email duplicado → excepción de negocio
- Actualizar entidad inexistente → 404
- Violación de regla de dominio → 409

> Si una regla no está cubierta por tests o QA manual, se considera incompleta.

---

### 2️⃣ QA manual Backend

Se realizan pruebas manuales directamente contra la API (Swagger / HTTP):

- POST / PUT con contratos inválidos → 400
- Acciones sobre entidades inexistentes → 404
- Violaciones de reglas → 409
- Tokens inválidos / expirados → 401
- Roles sin permiso → 403

El objetivo es validar que **la API nunca devuelva 500 para escenarios esperados**.

---

### 3️⃣ QA manual Frontend

El frontend se prueba con enfoque **defensivo**:

- Validaciones inline por campo
- Estados de carga (disable + texto)
- Prevención de doble submit
- Reset correcto de formularios
- Mensajes claros y no técnicos

SweetAlert se utiliza **solo para eventos globales**, no para validaciones de formulario.

---

## 🔍 Casos cubiertos (resumen)

### Módulo Casos (QA cerrado)
- Crear
- Editar
- Eliminar
- Cerrar
- Ver detalle
- Filtrar y paginar

Resultado:
> Módulo Casos **cerrado y validado**.

---

### Módulo Usuarios (QA parcial)
- Crear usuario
- Actualizar usuario
- Manejo de email duplicado
- Manejo de ID inexistente

Pendiente:
- Alinear frontend al estándar aplicado en Casos.

---

### Módulo Roles (QA pendiente)
- Asignar roles
- Evitar duplicados
- Manejo de errores backend

Pendiente:
- QA frontend.

---

## ⚠️ Manejo de errores (criterio)

LegalApp utiliza un patrón consistente:

| Tipo de error                | Código |
|-----------------------------|--------|
| Contrato inválido           | 400    |
| Entidad no encontrada       | 404    |
| Regla de negocio violada    | 409    |
| No autenticado              | 401    |
| No autorizado               | 403    |
| Error inesperado            | 500    |

> Los errores 500 se consideran **bugs**, no casos de uso.

---

## 🧩 Herramientas utilizadas

- xUnit (tests automatizados)
- Moq (mocks)
- Swagger UI (QA manual backend)
- DevTools (Network / Console)
- SweetAlert2 (feedback global)

---

## 🧠 Criterio de aceptación

Una funcionalidad se considera **QA-ready** cuando:

- Cumple reglas de negocio
- No genera errores 500 en escenarios esperados
- Devuelve status codes coherentes
- Maneja correctamente estados de carga
- Muestra mensajes claros al usuario
- Está documentada si tiene pendientes

---

## 🧾 Nota final

Este documento define **cómo se valida la calidad en LegalApp**.

Si una funcionalidad no cumple estos criterios,
no se considera lista para cierre ni despliegue.
