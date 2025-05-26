User Management API

API RESTful construida con ASP.NET Core 8, ASP.NET Identity y JWT para gestionar usuarios con roles.

🚀 Características

Autenticación: JWT (Bearer tokens).

Autorización: Roles Admin y User.

CRUD de usuarios: Endpoints protegidos con [Authorize].

Identity: Gestión segura de contraseñas, stamps y tokens.

Base de datos: SQL Server (configurable en appsettings.json).

Swagger para documentación interactiva.

Seed de datos: Al arrancar crea roles (Admin, User) y un usuario admin por defecto.

Pruebas:

Unitarias con xUnit + EF Core InMemory.

Integración con WebApplicationFactory<Program>.

🛠️ Requisitos

.NET 8 SDK

SQL Server (LocalDB / SQLEXPRESS / remoto)

(Opcional) Visual Studio 2022 o Rider

🔧 Configuración

Clona el repositorio y entra en la carpeta del API:

git clone https://github.com/tuUsuario/user-management-system.git
cd user-management-system/UserManagement.Api

Ajusta la cadena en appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=UserDb;Trusted_Connection=True;Encrypt=False;"
}

Define la clave JWT en el mismo archivo:

"JwtSettings": {
  "Issuer": "TuEmpresa.Api",
  "Audience": "TuEmpresa.Client",
  "Key": "UnaClaveSuperSecretaYLarga123!"
}

📦 Migraciones y despliegue

Instala EF Core CLI:

dotnet tool install --global dotnet-ef

Crea la primera migración:

dotnet ef migrations add InitialCreate

Aplica migraciones (crea la BD):

dotnet ef database update

Ejecuta la API:

dotnet run

Por defecto levantará en https://localhost:7111 y http://localhost:5062.

Abre Swagger en https://localhost:7111/swagger para probar manualmente.

🔑 Endpoints principales

Ruta

Método

Descripción

Roles

/api/auth/register

POST

Registra un nuevo usuario

Anónimo

/api/auth/login

POST

Devuelve JWT

Anónimo

/api/user

GET

Lista todos los usuarios

Admin

/api/user/{id}

GET

Obtiene un usuario por ID

Admin

/api/user

POST

Crea usuario

Admin

/api/user/{id}

PUT

Actualiza usuario

Admin

/api/user/{id}

DELETE

Elimina usuario

Admin

🧪 Pruebas

Desde la carpeta de tests:

cd UserManagement.Api.Tests
dotnet test

📄 Licencia

Proyecto de ejemplo para fines educativos.
