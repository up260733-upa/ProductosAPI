using ProductosAPI.Models;
using ProductosAPI.Repositories;

namespace ProductosAPI.Endpoints;

public static class ProductosEndpoints
{
    public static void MapProductoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/productos").WithTags("Productos");

        //Get /api/productos
        group.MapGet("/", (IProductoRepository repositorio) => Results.Ok(repositorio.ObtenerTodos()))
            .Produces<List<Producto>>();

        //Get /api/productos/{id}
        group.MapGet("/{id:int}", (IProductoRepository repositorio, int id) =>
        {
            var producto = repositorio.ObtenerPorId(id);
            return producto is null
                ? Results.NotFound(new { mensaje = "Producto no encontrado" })
                : Results.Ok(producto);
        })
            .Produces<Producto>()
            .Produces(StatusCodes.Status404NotFound);

        //Post /api/productos
        group.MapPost("/", (IProductoRepository repositorio, Producto nuevo) =>
        {
            var error = Validar(nuevo);
            if (error is not null)
            {
                return Results.BadRequest(new { mensaje = error }); //400
            }

            nuevo.Id = repositorio.Insertar(nuevo);
            return Results.Created($"/api/productos/{nuevo.Id}", nuevo); //201
        })
            .Produces<Producto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        //Put /api/productos/{id}
        group.MapPut("/{id:int}", (IProductoRepository repositorio, int id, Producto producto) =>
        {
            var error = Validar(producto);
            if (error is not null)
            {
                return Results.BadRequest(new { mensaje = error }); //400
            }

            //El id que manda es el de la ruta, no el del body
            producto.Id = id;
            return repositorio.Actualizar(producto)
                ? Results.Ok(producto)
                : Results.NotFound(new { mensaje = "Producto no encontrado" }); //404
        })
            .Produces<Producto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        //Delete /api/productos/{id}
        group.MapDelete("/{id:int}", (IProductoRepository repositorio, int id) =>
        {
            return repositorio.Eliminar(id)
                ? Results.NoContent() //204
                : Results.NotFound(new { mensaje = "Producto no encontrado" }); //404
        })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    //Los limites coinciden con las columnas de la tabla productos
    private static string? Validar(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre) || producto.Nombre.Length > 100)
        {
            return "El nombre es obligatorio y admite hasta 100 caracteres";
        }
        if (producto.Descripcion.Length > 255)
        {
            return "La descripcion admite hasta 255 caracteres";
        }
        if (producto.Precio < 0 || producto.Precio > 99999999.99m)
        {
            return "El precio debe estar entre 0 y 99999999.99";
        }
        if (producto.Stock < 0)
        {
            return "El stock no puede ser negativo";
        }
        return null;
    }
}
