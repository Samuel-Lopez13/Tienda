using Microsoft.EntityFrameworkCore;
using Tienda.DBContext;
using Tienda.Mapping;
using Tienda.Repositorio;
using Tienda.Repositorio.IRepositorio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SistemaVentasContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionMaestra"));
});

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Agregar implementacion para las interfaces
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//Se agrega para poder usar el Patch
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();