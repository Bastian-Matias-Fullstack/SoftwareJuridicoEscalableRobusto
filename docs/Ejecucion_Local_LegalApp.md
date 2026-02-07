# Ejecución Local – LegalApp

Este documento explica **cómo ejecutar la aplicación en local** cuando los archivos de configuración sensibles (`appsettings.json`) **no se versionan** por seguridad.

---

## ✅ Requisitos

- .NET SDK 8
- SQL Server (LocalDB o instancia local)
- (Opcional) Visual Studio 2022

---

## ⚙️ Configuración local (sin subir appsettings)

Este repositorio **no versiona** `appsettings.json`, `appsettings.Development.json` ni archivos de entorno.

Para ejecutar la aplicación en local debes crear tu propia configuración a partir de un template.

### 1️⃣ Crear appsettings local desde template

En la raíz del proyecto:

```powershell
Copy-Item .\appsettings.Template.json .\appsettings.Development.json
```

> `appsettings.Development.json` está ignorado por git y **no se sube al repositorio**.

---

### 2️⃣ Completar valores requeridos

Edita `appsettings.Development.json` y configura al menos:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- Configuración CORS (si aplica)

Ejemplo mínimo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LegalAppDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "CLAVE_LOCAL_DE_DESARROLLO",
    "Issuer": "LegalApp",
    "Audience": "LegalAppUsers"
  }
}
```

---

## 🗄️ Base de datos (migraciones)

el proyecto usa Entity Framework Core:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project Infraestructura --startup-project API
```

Ajusta los nombres de proyecto según tu solución si es necesario.

---

## ▶️ Ejecutar la aplicación

Desde la raíz del proyecto:

```bash
dotnet run --project API.csproj
```

La aplicación quedará disponible en:

- https://localhost:7266
- Swagger: https://localhost:7266/swagger

---

## 🔐 Roles y permisos

La aplicación trabaja con los siguientes roles:

- Admin
- Abogado
- Soporte

La visibilidad en frontend es una capa UX; **los permisos reales se validan en backend**.

Este archivo complementa el README principal del proyecto.
