using Microsoft.EntityFrameworkCore;
using Tienda;
using Tienda.DBContext;
using Tienda.Repositorio;
using Tienda.Repositorio.IRepositorio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SistemaVentasContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionMaestra"));
});

builder.Services.AddAutoMapper(typeof(MappingConfig));

//Agregar implementacion para las interfaces
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
