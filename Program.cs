using ProductosAPI.Data;
using ProductosAPI.Endpoints;
using ProductosAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Nuestros Servicios
builder.Services.AddSingleton<ConexionDb>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    //Swagger se muestra directo en http://localhost:5160
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductosAPI v1");
        options.RoutePrefix = string.Empty;
    });
}

//Endpoints
app.MapProductoEndpoints();

app.Run();
