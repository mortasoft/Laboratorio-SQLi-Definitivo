# Laboratorio de SQL Injection: Explotación y Mitigación

Este repositorio contiene una comparativa práctica de una vulnerabilidad SQLi en .NET Core.

## 🚀 Escenarios
- **Puerto 8080 (Vulnerable):** Implementación con concatenación de strings. Permite bypass de autenticación y extracción de datos con `sqlmap`.
- **Puerto 8081 (Seguro):** Implementación con **Consultas Parametrizadas**. Inmune a inyecciones.

## 🛠️ Herramientas utilizadas
- Docker / Docker-Compose
- SQL Server 2022
- sqlmap
- .NET Core 6.0
