using Serilog;
using Game_of_Drones.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/exception.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog(); // Configurar Serilog como el logger predeterminado

// Agregar servicios
builder.Services.AddControllers();

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GameDatabase")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migraciones y crear la base de datos automáticamente si no existe
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    dbContext.Database.Migrate();
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Sirve archivos estáticos desde "Frontend/Web/dist/web/browser"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Frontend", "Web", "dist", "web", "browser")),
    RequestPath = ""
});

// Middleware para manejar el enrutamiento de Angular y redirigir a index.html si no es un archivo estático o una API
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 &&
    !(context.Request.Path.Value?.StartsWith("/api") ?? false) &&
    !Path.HasExtension(context.Request.Path.Value))
    {
        context.Response.StatusCode = 200;
        await context.Response.SendFileAsync(Path.Combine("Frontend", "Web", "dist", "web", "browser", "index.html"));
    }
});

app.MapControllers();
app.Run();
