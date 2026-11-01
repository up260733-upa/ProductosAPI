using MySqlConnector;

namespace ProductosAPI.Data;

public class ConexionDb
{
    private readonly string _cadenaConexion;

    public ConexionDb(IConfiguration configuration)
    {
        //La cadena se lee de appsettings.json
        _cadenaConexion = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Falta la cadena de conexion 'MySql' en appsettings.json");
    }

    //Regresa una conexion ya abierta, quien la pide se encarga de cerrarla con using
    public MySqlConnection CrearConexion()
    {
        var conexion = new MySqlConnection(_cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
