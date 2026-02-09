# 🔐 Ejecución en Producción Local (Production-like)

Este proyecto está preparado para ejecutarse en **modo Producción local**, replicando el comportamiento real de un entorno productivo (configuración, seguridad, roles y manejo de errores), **sin exponer secretos en el repositorio**.

## 🎯 Objetivo
Permitir a evaluadores técnicos, reclutadores o equipos levantar la aplicación de forma segura y realista, usando variables de entorno tal como se hace en entornos empresariales.

---

## ⚙️ Variables de Entorno Requeridas

### 1️⃣ Connection String (Base de Datos)

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=LegalAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

### 2️⃣ JWT Key (Autenticación)

```powershell
$env:Jwt__Key="UNA_LLAVE_LARGA_Y_SEGURA_DE_AL_MENOS_32_CARACTERES"
```

---

### 3️⃣ Ambiente de Ejecución

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
```

---

## ▶️ Ejecución de la Aplicación

```powershell
dotnet run --project API.csproj --no-launch-profile
```

La aplicación quedará disponible en:

```
http://localhost:5000/login.html
```

---

## 🔒 Seguridad y Buenas Prácticas Aplicadas

- ❌ No se versionan secretos
- ✅ Secrets vía variables de entorno
- ✅ Configuración por ambiente
- ✅ Roles y permisos validados en backend
- ✅ UI solo refleja permisos
- ✅ Preparado para Linux / Cloud

---

## 🧪 Credenciales de Demo (Local)

| Usuario        | Rol     |
|---------------|---------|
| admin@demo.com| Admin   |
| roles@demo.com| Abogado |
| user@demo.com | Soporte |

---

## 🏁 Resultado Esperado

- Login funcional en Production local
- Endpoints protegidos (401 / 403)
- Swagger deshabilitado en Production
- Comportamiento idéntico a producción real
