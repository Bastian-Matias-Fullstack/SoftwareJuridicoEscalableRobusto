# 02 – Arquitectura de LegalApp

## 🎯 Objetivo de la arquitectura

La arquitectura de **LegalApp** está diseñada para demostrar **criterio profesional nivel Mid-Senior**, priorizando:

- Separación clara de responsabilidades
- Mantenibilidad a largo plazo
- Testabilidad real
- Reglas de negocio protegidas
- Backend como fuente de verdad
- Preparación para crecimiento y despliegue

No se busca complejidad innecesaria, sino **estructura correcta**.

---

## 🧱 Estilo arquitectónico

LegalApp implementa **Clean Architecture**, separando el sistema en capas bien definidas:

```
API  →  Application  →  Domain
            ↓
     Infrastructure
```

Cada capa tiene responsabilidades claras y **no depende de detalles de capas inferiores**.

---

## 🧩 Capas del sistema

### 1️⃣ Domain (Dominio)

Responsabilidad:
- Entidades del negocio
- Reglas invariantes
- Enums y Value Objects
- Lógica que **no depende de frameworks**

Ejemplos:
- Entidad `Caso`
- Estados del Caso
- Reglas básicas de consistencia

Principio clave:
> El Dominio no conoce nada del mundo exterior.

---

### 2️⃣ Application (Aplicación)

Responsabilidad:
- Casos de uso
- Reglas de negocio
- Validaciones funcionales
- Orquestación de flujos

Componentes:
- Command / Query Handlers
- Servicios de dominio
- DTOs
- Excepciones de negocio

Ejemplos:
- Crear Caso
- Actualizar Caso
- Cerrar Caso
- Crear Usuario
- Asignar Rol

Principio clave:
> Aquí viven las reglas reales del sistema.

---

### 3️⃣ Infrastructure (Infraestructura)

Responsabilidad:
- Acceso a datos
- Implementaciones técnicas
- Integración con frameworks externos

Componentes:
- DbContext (EF Core)
- Repositorios
- Configuración de persistencia

Principio clave:
> Infrastructure implementa contratos definidos por Application.

---

### 4️⃣ API (Capa de entrada)

Responsabilidad:
- Exponer endpoints HTTP
- Autenticación y autorización
- Normalización de requests
- Traducción HTTP (status codes)

Características:
- Controllers delgados
- Sin lógica de negocio
- Delegación completa a Application

Principio clave:
> El controller **no decide reglas**, solo traduce.

---

## 🔐 Seguridad en la arquitectura

### Autenticación
- JWT Bearer
- Claims de identidad y rol
- Validación centralizada

### Autorización
- Policies por rol
- Validación doble:
  - Frontend (UX)
  - Backend (real)

Regla fundamental:
> **Nunca confiar en el frontend**.

---

## ⚠️ Manejo de errores

LegalApp utiliza un **Middleware Global de Errores**, responsable de:

- Capturar excepciones de Application
- Traducirlas a `ProblemDetails`
- Asignar status codes correctos

Ejemplos:
- Regla de negocio → `409 Conflict`
- Entidad no encontrada → `404 Not Found`
- Contrato inválido → `400 Bad Request`
- Error inesperado → `500 Internal Server Error`

Esto evita:
- try/catch duplicados
- parsing de strings
- respuestas inconsistentes

---

## 🧪 Arquitectura orientada a pruebas

La separación por capas permite:

- Tests unitarios en Application
- Tests de Dominio sin dependencias
- Mock de repositorios
- Validación de reglas sin HTTP

Principio clave:
> Si una regla no se puede testear, la arquitectura está mal.

---

## 🌐 Configuración por ambiente

La arquitectura separa **configuración del código**:

- Development
- Production

Ejemplos:
- JWT Key por variable de entorno
- CORS por configuración
- Swagger habilitado conscientemente

Esto permite:
- Simular producción en local
- Evitar dependencias del IDE
- Preparar despliegue cloud real

---

## 📌 Decisiones arquitectónicas clave

- Backend como autoridad
- Controllers sin lógica de negocio
- Excepciones tipadas para reglas
- Estado del Caso protegido
- Configuración explícita por ambiente
- Arquitectura preparada para crecer

Estas decisiones reflejan **experiencia real**, no patrones teóricos.

---

## 🧾 Nota final

Este documento describe **cómo está construido LegalApp** y **por qué**.

Los detalles específicos de reglas, QA, frontend y despliegue se documentan en los siguientes archivos dentro de `/docs`.
