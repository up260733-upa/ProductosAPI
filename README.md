# ProductosAPI

Backend en ASP.NET Core (.NET 10) con MySQL para administrar productos: consultar, buscar, insertar, actualizar y eliminar. Se prueba desde Swagger.

**Autora:** Mia Lizeska Babun Pérez

## Cómo ejecutarlo

Necesitas el SDK de .NET 10 y un servidor MySQL encendido.

1. Crea la base de datos, la tabla y tres productos de ejemplo:

   ```bash
   mysql -u root -p < database/productos.sql
   ```

2. Si tu usuario `root` tiene contraseña, escríbela en `ConnectionStrings:MySql` dentro de `appsettings.json`.

3. Inicia la API:

   ```bash
   dotnet run
   ```

4. Abre Swagger en <http://localhost:5160>.

## Estructura

| Archivo | Para qué sirve |
| --- | --- |
| `Program.cs` | Arranca la app, registra los servicios y activa Swagger |
| `Endpoints/ProductosEndpoints.cs` | Define las rutas HTTP y valida los datos |
| `Repositories/IProductoRepository.cs` | La interface que pide la actividad |
| `Repositories/ProductoRepository.cs` | Implementa la interface con consultas SQL |
| `Data/ConexionDb.cs` | Lee la cadena de conexión y abre la conexión |
| `Models/Producto.cs` | Representa un producto |
| `database/productos.sql` | Crea la base de datos y la tabla |

## Puntos de la actividad

| Punto | Método | Ruta | SQL | Respuestas |
| --- | --- | --- | --- | --- |
| Conexión | `ConexionDb.CrearConexion()` | | | |
| Consultas | `ObtenerTodos()` | `GET /api/productos` | `SELECT` | 200 |
| Buscar | `ObtenerPorId(id)` | `GET /api/productos/{id}` | `SELECT ... WHERE id` | 200, 404 |
| Insert | `Insertar(producto)` | `POST /api/productos` | `INSERT` | 201, 400 |
| Actualizar | `Actualizar(producto)` | `PUT /api/productos/{id}` | `UPDATE` | 200, 400, 404 |
| Eliminar | `Eliminar(id)` | `DELETE /api/productos/{id}` | `DELETE` | 204, 404 |

Ejemplo de body para `POST` y `PUT`:

```json
{
  "nombre": "Bocina Bluetooth",
  "descripcion": "Bocina portatil",
  "precio": 499.90,
  "stock": 12
}
```

## Flujo del proyecto

Cuando la app arranca, `Program.cs` registra `ConexionDb` y le indica que, cada vez que alguien pida un `IProductoRepository`, entregue un `ProductoRepository`. Así los endpoints solo conocen la interface y no saben cómo se guardan los datos.

Una petición recorre estos pasos:

1. Swagger manda la petición HTTP, por ejemplo `POST /api/productos` con un JSON.
2. El endpoint que coincide con la ruta convierte el JSON en un objeto `Producto`.
3. En `POST` y `PUT`, el método `Validar` revisa el nombre, la descripción, el precio y el stock. Si algo está mal, responde 400 y ya no se toca la base de datos.
4. El endpoint llama al método del repositorio que corresponde.
5. El repositorio pide una conexión abierta a `ConexionDb`, arma el comando SQL con parámetros y lo ejecuta en MySQL.
6. Al terminar el método, `using` cierra la conexión.
7. El repositorio regresa el resultado y el endpoint lo convierte en una respuesta HTTP.

### Qué hace cada método del repositorio

- **`ObtenerTodos`** ejecuta un `SELECT`, recorre las filas con el lector y regresa una lista de productos.
- **`ObtenerPorId`** busca una sola fila por id. Si no la encuentra regresa `null`, y el endpoint responde 404.
- **`Insertar`** ejecuta el `INSERT` y regresa el id que MySQL generó, que el endpoint incluye en su respuesta 201.
- **`Actualizar`** ejecuta el `UPDATE` y regresa `true` si encontró el registro. Si no, el endpoint responde 404.
- **`Eliminar`** ejecuta el `DELETE` y regresa `true` si borró el registro. Si no, el endpoint responde 404.
- **`LeerProducto`** y **`AgregarParametros`** son métodos auxiliares para no repetir código. El primero convierte una fila en un `Producto`; el segundo pasa los valores como parámetros, lo que evita la inyección SQL.
