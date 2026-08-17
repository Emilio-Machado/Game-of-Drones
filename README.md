# Game of Drones

Este es un proyecto completo de [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) y [Angular](https://angular.io/), desarrollado como parte de una prueba como Desarrollador Full Stack Senior. El proyecto consiste en una API para manejar partidas, una base de datos SQLite integrada y un frontend en Angular.


## Requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (obligatorio)
- [Node.js](https://nodejs.org/) 20.11.1 o más reciente, con npm (obligatorio)


⚠️ **Nota Importante**: El proyecto está diseñado para clonarse y ejecutarse directamente desde Visual Studio. Al presionar **Iniciar**, MSBuild comprueba Node.js y npm, restaura las dependencias con `npm ci` cuando es necesario y compila Angular antes de iniciar ASP.NET Core. La base de datos SQLite se configura automáticamente y las migraciones se aplican en el primer inicio.

La distribución compilada de Angular no se almacena en Git. Si Node.js o npm no están disponibles, la compilación se detiene con un mensaje que indica cómo resolver el requisito faltante.

## Configuración del Proyecto

1.  Clona el repositorio:
    
    `git clone https://github.com/Emilio-Machado/Game-of-Drones.git` 
    
2.  No se requiere configurar secretos para ejecutar el proyecto localmente. En el entorno `Development`, la aplicación genera una clave JWT criptográficamente segura y temporal al iniciar.

> **Configuración segura**:
> - La clave de desarrollo se mantiene únicamente en memoria y cambia con cada reinicio.
> - Fuera de `Development`, la aplicación no inicia si no se configura `Jwt:Secret`.
> - En variables de entorno, usa `Jwt__Secret` con una clave Base64 de al menos 256 bits.
> - En despliegues externos, configura `AllowedHosts` con los dominios permitidos, separados por punto y coma (por ejemplo, `app.example.com;api.example.com`).
> - Las credenciales y claves reales nunca deben almacenarse en archivos versionados.


## Ejecución del Proyecto

### Backend (.NET)

Abre la solución en Visual Studio y presiona **Iniciar**. El proyecto restaura las dependencias de .NET y Angular, compila ambos componentes, aplica las migraciones y configura la base de datos SQLite. La API y el frontend estarán disponibles en

`http://localhost:1179`.

Alternativamente, puedes ejecutar el proyecto desde la línea de comandos:


`dotnet run` 

### Frontend (Angular)

No se requiere ejecutar comandos manuales para iniciar el frontend. Cuando se inicia o compila el proyecto .NET:

- `npm ci` se ejecuta en el primer build y cuando cambia `package.json` o `package-lock.json`.
- `npm run build` genera `Frontend/Web/dist/web/browser` antes de iniciar ASP.NET Core.
- Visual Studio considera los archivos fuente de Angular en su comprobación de proyecto actualizado, por lo que un cambio en el frontend provoca una nueva compilación al volver a presionar **Iniciar**.
- `dotnet publish` incluye automáticamente la distribución Angular generada en el artefacto publicado.

Angular CLI está incluido como dependencia local del proyecto; no es necesario instalarlo globalmente.


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
