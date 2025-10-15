# ApiClientes: El Backend (API REST con .NET Core)

Esta es la API que maneja toda la lógica del negocio para la gestión de clientes (el ABM). Está construida usando "ASP.NET Core Minimal APIs", que es la forma más moderna y rápida de montar *endpoints* en .NET.

### 1. La Arquitectura
Elegí Minimal APIs porque son súper livianas. No hay controladores grandes; solo el archivo `Program.cs` mapeando las rutas directamente a la lógica. Esto mantiene la API sencilla y muy fácil de mantener.

### 2. La Base de Datos
Use PostgreSQL con Entity Framework Core (EF Core). Esto nos permite trabajar con objetos C# en lugar de escribir SQL a mano, lo cual agiliza el desarrollo y las migraciones. La base de datos se crea y actualiza automáticamente al iniciar la aplicación.

### 3. Reglas de Negocio y Errores
La API es la responsable de proteger los datos. Implementé dos cosas clave:
* Validación de campos: Revisa que el cliente envíe todos los datos obligatorios. Si falla, devuelve un error "400 Bad Request".
* Unicidad del CUIT: Es una regla de oro: cada cliente debe tener un CUIT único. Si intentas crear o editar un cliente con un CUIT que ya existe, la API devuelve un error "409 Conflict". Esto es importante para que el Frontend sepa exactamente qué salió mal.

### 4. La Búsqueda
Para que el Frontend pudiera filtrar clientes fácilmente, creé un *endpoint* de búsqueda. En lugar de complicar la URL, le pasamos el texto a buscar como un parámetro de consulta por ejemplo:/clientes/search?query=Juan. Esto hace que la ruta sea simple y flexible.

### 5. Acceso al Frontend
Configuré CORS. Esto es vital porque el Frontend (Angular) corre en un puerto diferente al de la API. Con el CORS activado, el navegador permite que la aplicación Angular se comunique con esta API sin que la seguridad del navegador la bloquee.

## Rutas de la API

* Ver todos: GET /clientes
* Ver uno: GET /clientes/{id}
* Buscar: GET /clientes/search?query={texto}
* Crear: POST /clientes
* Editar: PUT /clientes/{id}
* Borrar: DELETE /clientes/{id}
