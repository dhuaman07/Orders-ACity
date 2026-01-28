# 📦 Orders API

API REST para la gestión de órdenes, desarrollada con **.NET**, aplicando **Clean Architecture**, **CQRS**, y **Entity Framework Core**, siguiendo buenas prácticas de diseño y arquitectura de software.

---

## 🧠 Arquitectura

El proyecto está estructurado siguiendo los principios de **Clean Architecture**, manteniendo el dominio desacoplado de frameworks externos.

# Migracion Data Base

- Crear la base de datos OrdersACDB
- Ejecutar los comandos de migracion

 Add-Migration InitialCreate -Context AppDbContext -Verbose  (Si)
 Update-Database -Verbose -Context AppDbContext (Si)
 Remove-Migration -Context AppDbContext (No)
