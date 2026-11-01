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

El proyecto está dividido en partes y cada una hace una sola cosa: los endpoints reciben las peticiones, el repositorio habla con la base de datos y `ConexionDb` abre la conexión.

Al arrancar, `Program.cs` prepara todo y deja listo Swagger, que es la página desde donde se prueba la API. También indica que, cuando se pida un `IProductoRepository`, se use `ProductoRepository`. Por eso los endpoints trabajan con la interface y no necesitan saber cómo se guardan los datos.

Por ejemplo, esto pasa al agregar un producto:

1. En Swagger se llena el JSON del producto y se presiona **Execute**. Eso manda un `POST /api/productos`.
2. El endpoint recibe la petición y convierte el JSON en un objeto `Producto`.
3. El método `Validar` revisa que los datos estén bien, por ejemplo que haya nombre y que el precio no sea negativo. Si algo está mal, responde 400 y no llega a la base de datos.
4. Si todo está bien, el endpoint llama a `Insertar` del repositorio.
5. El repositorio le pide a `ConexionDb` una conexión abierta y ejecuta el `INSERT` en MySQL. La conexión se cierra sola al terminar, gracias a `using`.
6. MySQL genera el id del producto y el repositorio se lo regresa al endpoint.
7. El endpoint responde 201 con el producto creado, y Swagger muestra esa respuesta.

Las demás operaciones siguen el mismo camino y solo cambian el método del repositorio y la consulta SQL. Si el producto que se busca, actualiza o elimina no existe, el endpoint responde 404.

### Qué hace cada método del repositorio

- **`ObtenerTodos`** ejecuta un `SELECT`, recorre las filas con el lector y regresa una lista de productos.
- **`ObtenerPorId`** busca una sola fila por id. Si no la encuentra regresa `null`, y el endpoint responde 404.
- **`Insertar`** ejecuta el `INSERT` y regresa el id que MySQL generó, que el endpoint incluye en su respuesta 201.
- **`Actualizar`** ejecuta el `UPDATE` y regresa `true` si encontró el registro. Si no, el endpoint responde 404.
- **`Eliminar`** ejecuta el `DELETE` y regresa `true` si borró el registro. Si no, el endpoint responde 404.
- **`LeerProducto`** y **`AgregarParametros`** son métodos auxiliares para no repetir código. El primero convierte una fila en un `Producto`; el segundo pasa los valores como parámetros, lo que evita la inyección SQL.
