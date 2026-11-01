using MySqlConnector;
using ProductosAPI.Data;
using ProductosAPI.Models;

namespace ProductosAPI.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly ConexionDb _conexionDb;

    public ProductoRepository(ConexionDb conexionDb)
    {
        _conexionDb = conexionDb;
    }

    //Consulta: SELECT de todos los productos
    public List<Producto> ObtenerTodos()
    {
        const string sql = "SELECT id, nombre, descripcion, precio, stock FROM productos ORDER BY id";

        using var conexion = _conexionDb.CrearConexion();
        using var comando = new MySqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        var productos = new List<Producto>();
        while (lector.Read())
        {
            productos.Add(LeerProducto(lector));
        }
        return productos;
    }

    //Buscar: SELECT de un producto por su id, null si no existe
    public Producto? ObtenerPorId(int id)
    {
        const string sql = "SELECT id, nombre, descripcion, precio, stock FROM productos WHERE id = @id";

        using var conexion = _conexionDb.CrearConexion();
        using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        using var lector = comando.ExecuteReader();

        return lector.Read() ? LeerProducto(lector) : null;
    }

    //Insert: regresa el id que MySQL le asigno al nuevo registro
    public int Insertar(Producto producto)
    {
        const string sql = @"INSERT INTO productos (nombre, descripcion, precio, stock)
                             VALUES (@nombre, @descripcion, @precio, @stock)";

        using var conexion = _conexionDb.CrearConexion();
        using var comando = new MySqlCommand(sql, conexion);
        AgregarParametros(comando, producto);
        comando.ExecuteNonQuery();

        return (int)comando.LastInsertedId;
    }

    //Actualizar: true si encontro el registro
    public bool Actualizar(Producto producto)
    {
        const string sql = @"UPDATE productos
                             SET nombre = @nombre, descripcion = @descripcion, precio = @precio, stock = @stock
                             WHERE id = @id";

        using var conexion = _conexionDb.CrearConexion();
        using var comando = new MySqlCommand(sql, conexion);
        AgregarParametros(comando, producto);
        comando.Parameters.AddWithValue("@id", producto.Id);

        return comando.ExecuteNonQuery() > 0;
    }

    //Eliminar: true si borro el registro
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM productos WHERE id = @id";

        using var conexion = _conexionDb.CrearConexion();
        using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    //Convierte la fila actual del lector en un Producto
    private static Producto LeerProducto(MySqlDataReader lector)
    {
        return new Producto
        {
            Id = lector.GetInt32("id"),
            Nombre = lector.GetString("nombre"),
            Descripcion = lector.GetString("descripcion"),
            Precio = lector.GetDecimal("precio"),
            Stock = lector.GetInt32("stock")
        };
    }

    //Parametros compartidos por Insertar y Actualizar, evitan la inyeccion SQL
    private static void AgregarParametros(MySqlCommand comando, Producto producto)
    {
        comando.Parameters.AddWithValue("@nombre", producto.Nombre);
        comando.Parameters.AddWithValue("@descripcion", producto.Descripcion);
        comando.Parameters.AddWithValue("@precio", producto.Precio);
        comando.Parameters.AddWithValue("@stock", producto.Stock);
    }
}
