using DocumentosAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Controladores ──────────────────────────────────────────
builder.Services.AddControllers();

// ── Base de datos MySQL con Laragon ────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// ── CORS para permitir Angular en localhost:4200 ────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ── Límite de tamaño de archivos subidos (100 MB) ──────────
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104_857_600; // 100 MB
});

var app = builder.Build();

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();