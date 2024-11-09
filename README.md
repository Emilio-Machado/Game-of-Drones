# Game of Drones

Este es un proyecto completo de [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) y [Angular](https://angular.io/), desarrollado como parte de una prueba como Desarrollador Full Stack Senior. El proyecto consiste en una API para manejar partidas, una base de datos SQLite integrada y un frontend en Angular.


## Requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (obligatorio)
- [Node.js](https://nodejs.org/) – versión 18.19.1 o más reciente (solo si deseas modificar el frontend)
- [Angular CLI 18](https://www.npmjs.com/package/@angular/cli/v/18.0.0) (solo si deseas modificar el frontend)


⚠️ **Nota Importante**: El proyecto está diseñado para clonarse y ejecutarse directamente en Visual Studio sin pasos adicionales. La base de datos SQLite se configura automáticamente y las migraciones se aplican en el primer inicio. Además, se incluye una compilación del frontend en la carpeta `dist`, lo que permite ejecutar el proyecto sin necesidad de instalar ni compilar Angular.

## Configuración del Proyecto

1.  Clona el repositorio:
    
    `git clone https://github.com/Emilio-Machado/Game-of-Drones.git` 
    
2.  Configuración de `appsettings.json`:
    
-   El archivo `appsettings.json` ya incluye una configuración básica con un secreto JWT y una cadena de conexión para SQLite.
    

> ⚠️ **Nota de Seguridad**:
> -   Para facilitar la prueba, la clave secreta JWT (`Jwt:Secret`) está en `appsettings.json`.
> -   **Esto no es seguro para producción**. Para un entorno de producción:
>     -   Mueve `Jwt:Secret` a una variable de entorno (`JWT_SECRET`).
>     -   Modifica `Program.cs` para cargar el secreto desde las variables de entorno.


## Ejecución del Proyecto

### Backend (.NET)

Abre la solución en Visual Studio y presiona **Iniciar** para que el proyecto restaure automáticamente las dependencias, aplique las migraciones y configure la base de datos SQLite. La API estará disponible en 

`http://localhost:1179`.

Alternativamente, puedes ejecutar el proyecto desde la línea de comandos:


`dotnet run` 

### Frontend (Angular)

⚠️ **Nota Importante**: El proyecto incluye una compilación del frontend en la carpeta `Frontend/Web/dist`, por lo que se sirve directamente sin necesidad de instalar ni compilar Angular.

Si deseas modificar el frontend, sigue estos pasos:

1.  Ve a la carpeta del frontend:
    
    `cd Frontend/Web` 
    
2.  Instala las dependencias:
    
    `npm install` 
    
3.  Compila el proyecto Angular:

    `npm run build` 
    
> **Nota**: Esto generará los nuevos archivos en `dist/web`, que se servirán automáticamente junto con el backend.


## Endpoints y Documentación de la API

Swagger está habilitado para la API en el entorno de desarrollo. Para ver la documentación interactiva, ejecuta el proyecto y ve a:


`http://localhost:1179/swagger` 


## Estructura del Proyecto

-   `Controllers/` - Controladores de la API para manejar los endpoints.
-   `DTOs/` - Objetos de transferencia de datos entre el backend y el frontend.
-   `DataAccess/` - Contiene el contexto de la base de datos y las migraciones.
-   `Frontend/Web/` - Código fuente del frontend en Angular.
-   `Migrations/` - Migraciones de la base de datos generadas por Entity Framework.
-   `Models/` - Modelos de datos que representan las entidades del sistema.
-   `Properties/` - Configuraciones adicionales para el proyecto .NET.
-   `Logs/` - Directorio de logs (excluido del repositorio).

## Licencia

Este proyecto es solo para evaluación y desarrollo interno. No está disponible para uso comercial o distribución pública sin autorización.
