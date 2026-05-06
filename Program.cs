using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GestionDocumental.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GestionDocumentalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GestionDocumentalContext") ?? throw new InvalidOperationException("Connection string 'GestionDocumentalContext' not found.")));

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<GestionDocumental.Soporte.CONSTANTES>();


// ===== CONFIGURACIÓN CORS ACTUALIZADA PARA PRODUCCIÓN =====
// Configurar CORS para permitir acceso desde otros proyectos locales y producción en Render
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalNetwork", policy =>
    {
        // Orígenes permitidos para desarrollo local
        policy.WithOrigins("http://localhost:*", "http://192.168.*:*")
        // CAMBIO: Agregar URL de producción en Render (reemplazar con tu URL real)
              .WithOrigins("https://tu-frontend-de-render.onrender.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "GestionDocumental API", 
        Version = "v1",
        Description = "API para la gestión documental"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

// ===== HABILITAR CORS PARA PERMITIR ACCESO DESDE OTROS PROYECTOS LOCALES Y PRODUCCIÓN =====
// Habilitar CORS para permitir acceso desde otros proyectos locales y producción
app.UseCors("AllowLocalNetwork");

app.UseAuthorization();

app.MapControllers();

app.Run();