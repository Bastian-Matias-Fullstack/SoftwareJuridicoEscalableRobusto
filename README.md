# ⚖️ LegalApp – Sistema de Gestión de Casos Jurídicos

**LegalApp** es una aplicación web **fullstack** orientada a la gestión de casos jurídicos, desarrollada como **proyecto de portafolio profesional nivel Mid-Senior Fullstack .NET**.

El foco del proyecto no es solo “que funcione”, sino demostrar:
- criterio de arquitectura
- reglas de negocio claras
- seguridad y control de roles
- manejo consistente de errores
- QA consciente
- preparación para despliegue y demos públicas

---

## 🧱 Stack Tecnológico

### Backend
- **ASP.NET Core 8** – Web API REST
- **Entity Framework Core**
- **SQL Server**
- **Clean Architecture** (Domain, Application, Infrastructure)
- **FluentValidation**
- **JWT Authentication (Bearer)**
- **Swagger / OpenAPI**
- **Middleware global de errores (`ProblemDetails`)**

### Frontend
- **HTML5 + CSS3**
- **Bootstrap 5 (glassmorphism)**
- **JavaScript Vanilla (Fetch API)**
- **SweetAlert2**
- Dashboard desacoplado consumiendo API con JWT

---

## 🚀 Funcionalidades principales

- CRUD completo de **Casos Jurídicos**
- Control de **estados del Caso** (Pendiente / En Proceso / Cerrado)
- Reglas de negocio aplicadas en backend
- Autenticación con JWT
- Autorización por roles (**Admin / Abogado / Soporte**)
- Filtros combinados, ordenamiento y paginación real
- Manejo consistente de errores HTTP (400 / 404 / 409 / 401 / 403)
- Swagger habilitado para pruebas y demos

---

## 🔐 Seguridad y Roles

Roles soportados:
- **Admin**: gestión completa
- **Abogado**: gestión de Casos
- **Soporte**: gestión de Usuarios

> ⚠️ Importante  
> El frontend **no es la capa de seguridad**.  
> Todas las reglas y permisos se validan nuevamente en backend.

---

## 🧪 Calidad y QA

- Tests automatizados en **Application** y **Domain**
- QA manual por módulo y rol
- Eliminación de errores 500 para escenarios esperados
- Uso de excepciones de dominio + middleware global
- Criterio claro de “QA-ready”

---

## 📂 Estructura del proyecto

```
LegalApp/
├── API.csproj
├── Aplicacion/
├── Dominio/
├── Infraestructura/
├── wwwroot/              # Frontend (HTML / CSS / JS)
├── LegalApp.Tests/
│
├── docs/                 # Documentación técnica y funcional
│   ├── 01_Contexto_General.md
│   ├── 02_Arquitectura.md
│   ├── 03_Reglas_de_Negocio_y_Roles.md
│   ├── 04_QA_y_Pruebas.md
│   ├── 05_Frontend.md
│   ├── 06_Estado_y_Pendientes.md
│   └── 07_Despliegue_y_Portafolio.md
│
├── appsettings.example.json
├── README.md
└── .gitignore
```

---

## 📚 Documentación del proyecto

La documentación completa del sistema se encuentra en la carpeta **`/docs`**.

Orden recomendado de lectura:
1. Contexto General  
2. Arquitectura  
3. Reglas de Negocio y Roles  
4. QA y Pruebas  
5. Frontend  
6. Estado y Pendientes  
7. Despliegue y Portafolio  

Esta documentación refleja **el estado real del proyecto** y las decisiones técnicas tomadas.

---

## ▶️ Ejecución local (desarrollo)

```bash
dotnet run --project API.csproj
```

Swagger:
```
https://localhost:7266/swagger
```

---

## 🌐 Preparación para producción

- Configuración por ambientes (`Development / Production`)
- Variables sensibles por **variables de entorno**
- CORS configurable
- Swagger controlado por configuración
- Backend listo para despliegue cloud

Detalles completos en `/docs/07_Despliegue_y_Portafolio.md`.

---

## 📌 Estado del proyecto

- ✅ Backend estable
- ✅ Tests pasando
- ✅ Arquitectura definida
- ✅ Módulo Casos cerrado
- ⏳ QA pendiente en Usuarios y Roles (documentado)

---

## 🧾 Nota final

LegalApp forma parte de un **portafolio profesional** y está pensado para:
- demos técnicas
- entrevistas
- evaluación de criterio de ingeniería

**Autor:** @Derox1
