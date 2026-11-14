// =====================================================
// REGISTRO DE ROPA PARA BOUTIQUE - TAREA 4
// Programación orientada a objetos + SQL Server
// Primero va el programa principal y después las clases.
// =====================================================

try
{
    BaseDatos baseDatos = new BaseDatos("Server=localhost\\SQLEXPRESS01;Database=BoutiqueDB;Trusted_Connection=True;TrustServerCertificate=True;");
    Inventario inventario = new Inventario(baseDatos);
    MenuBoutique menu = new MenuBoutique(inventario);
    menu.Iniciar();
}
catch (Exception error)
{
    Console.WriteLine("No se pudo iniciar el programa: " + error.Message);
    Console.WriteLine("Revise que SQL Server esté encendido y que la base BoutiqueDB exista.");
    Console.WriteLine("\nPresione una tecla para salir...");
    Console.ReadKey();
}

// =====================================================
// MODELO: clases del negocio
// =====================================================

// Clase base: todo lo que tiene Id y Nombre (categorías, tallas y proveedores) hereda de aquí
public class ElementoCatalogo
{
    private string nombre = "";

    public int Id { get; }

    public string Nombre
    {
        get { return nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre no puede estar vacío.");
            }

            nombre = value.Trim();
        }
    }

    public ElementoCatalogo(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }

    public override string ToString()
    {
        return Id + " - " + Nombre;
    }
}

public class Categoria : ElementoCatalogo
{
    public Categoria(int id, string nombre) : base(id, nombre)
    {
    }
}

public class Talla : ElementoCatalogo
{
    public Talla(int id, string nombre) : base(id, nombre)
    {
    }
}

public class Proveedor : ElementoCatalogo
{
    private string email = "";

    public string Telefono { get; set; }

    public string Email
    {
        get { return email; }
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
            {
                throw new ArgumentException("El email no es válido.");
            }

            email = value.Trim();
        }
    }

    public Proveedor(int id, string nombre, string telefono, string email) : base(id, nombre)
    {
        Telefono = telefono;
        Email = email;
    }

    public override string ToString()
    {
        return base.ToString() + " | Tel: " + Telefono + " | Email: " + Email;
    }
}

public class Prenda
{
    private string nombre = "";
    private decimal precio;
    private int existencias;
    private decimal descuento;

    public int Id { get; }
    public Categoria Categoria { get; set; }
    public Talla Talla { get; set; }
    public Proveedor Proveedor { get; set; }

    public string Nombre
    {
        get { return nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre de la prenda no puede estar vacío.");
            }

            nombre = value.Trim();
        }
    }

    public decimal Precio
    {
        get { return precio; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor que cero.");
            }

            precio = value;
        }
    }

    public int Existencias
    {
        get { return existencias; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Las existencias no pueden ser negativas.");
            }

            existencias = value;
        }
    }

    // Descuento en porcentaje, de 0 a 100
    public decimal Descuento
    {
        get { return descuento; }
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("El descuento debe estar entre 0 y 100.");
            }

            descuento = value;
        }
    }

    // Se calcula solo, no se guarda
    public decimal PrecioFinal
    {
        get { return Precio - (Precio * Descuento / 100); }
    }

    public Prenda(int id, string nombre, decimal precio, int existencias,
                  Categoria categoria, Talla talla, Proveedor proveedor)
    {
        Id = id;
        Nombre = nombre;
        Precio = precio;
        Existencias = existencias;
        Categoria = categoria;
        Talla = talla;
        Proveedor = proveedor;
        Descuento = 0;
    }

    public void AgregarExistencias(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        }

        Existencias += cantidad;
    }

    public void RetirarExistencias(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        }

        if (cantidad > Existencias)
        {
            throw new InvalidOperationException("No hay suficientes existencias.");
        }

        Existencias -= cantidad;
    }

    public override string ToString()
    {
        return "[" + Id + "] " + Nombre +
               " | " + Categoria.Nombre +
               " | Talla " + Talla.Nombre +
               " | Prov: " + Proveedor.Nombre +
               " | Precio: $" + Precio.ToString("N2") +
               " | Desc: " + Descuento + "%" +
               " | Final: $" + PrecioFinal.ToString("N2") +
               " | Stock: " + Existencias;
    }
}

// Guarda los datos tal como estaban al momento de vender,
// así el historial no cambia aunque después cambie el precio o se borre la prenda
public class Venta
{
    public int Id { get; }
    public int PrendaId { get; }
    public string NombrePrenda { get; }
    public int Cantidad { get; }
    public decimal PrecioUnitario { get; }
    public DateTime Fecha { get; }

    public decimal Total
    {
        get { return PrecioUnitario * Cantidad; }
    }

    // Se usa cuando la fecha ya existe (por ejemplo, al cargar desde la base de datos)
    public Venta(int id, int prendaId, string nombrePrenda, int cantidad, decimal precioUnitario, DateTime fecha)
    {
        Id = id;
        PrendaId = prendaId;
        NombrePrenda = nombrePrenda;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Fecha = fecha;
    }

    public override string ToString()
    {
        return "[" + Id + "] " + Fecha.ToString("dd/MM/yyyy HH:mm") +
               " | " + NombrePrenda +
               " | Cant: " + Cantidad +
               " | P. unitario: $" + PrecioUnitario.ToString("N2") +
               " | Total: $" + Total.ToString("N2");
    }
}


// =====================================================
// ACCESO A DATOS: único lugar que habla con SQL Server
// =====================================================

// Esta clase es la única que habla con SQL Server.
// Inventario le pide que guarde, cambie, borre o cargue datos.
public class BaseDatos
{
    private string cadenaConexion;

    public BaseDatos(string cadenaConexion)
    {
        this.cadenaConexion = cadenaConexion;
    }

    // ==============================
    // AYUDA: ejecutan SQL y siempre cierran la conexión
    // ==============================

    // Para UPDATE y DELETE
    private void EjecutarComando(string sql, params SqlParameter[] parametros)
    {
        SqlConnection conexion = new SqlConnection(cadenaConexion);

        try
        {
            conexion.Open();
            SqlCommand comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);
            comando.ExecuteNonQuery();
        }
        finally
        {
            conexion.Close();
        }
    }

    // Para INSERT: devuelve el Id que SQL Server le asignó al registro nuevo
    private int EjecutarInsert(string sql, params SqlParameter[] parametros)
    {
        SqlConnection conexion = new SqlConnection(cadenaConexion);

        try
        {
            conexion.Open();
            SqlCommand comando = new SqlCommand(sql + "; SELECT CAST(SCOPE_IDENTITY() AS INT);", conexion);
            comando.Parameters.AddRange(parametros);
            return Convert.ToInt32(comando.ExecuteScalar());
        }
        finally
        {
            conexion.Close();
        }
    }

    // Para SELECT: ejecuta "alLeerFila" una vez por cada fila que devuelve la consulta
    private void Consultar(string sql, Action<SqlDataReader> alLeerFila)
    {
        SqlConnection conexion = new SqlConnection(cadenaConexion);

        try
        {
            conexion.Open();
            SqlCommand comando = new SqlCommand(sql, conexion);
            SqlDataReader lector = comando.ExecuteReader();

            while (lector.Read())
            {
                alLeerFila(lector);
            }

            lector.Close();
        }
        finally
        {
            conexion.Close();
        }
    }

    // ==============================
    // CATEGORÍAS
    // ==============================

    public List<Categoria> CargarCategorias()
    {
        List<Categoria> lista = new List<Categoria>();

        Consultar("SELECT Id, Nombre FROM Categorias ORDER BY Id", fila =>
        {
            lista.Add(new Categoria(fila.GetInt32(0), fila.GetString(1)));
        });

        return lista;
    }

    public int InsertarCategoria(string nombre)
    {
        return EjecutarInsert("INSERT INTO Categorias (Nombre) VALUES (@Nombre)",
            new SqlParameter("@Nombre", nombre));
    }

    public void ActualizarCategoria(int id, string nombre)
    {
        EjecutarComando("UPDATE Categorias SET Nombre = @Nombre WHERE Id = @Id",
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Id", id));
    }

    public void EliminarCategoria(int id)
    {
        EjecutarComando("DELETE FROM Categorias WHERE Id = @Id",
            new SqlParameter("@Id", id));
    }

    // ==============================
    // TALLAS
    // ==============================

    public List<Talla> CargarTallas()
    {
        List<Talla> lista = new List<Talla>();

        Consultar("SELECT Id, Nombre FROM Tallas ORDER BY Id", fila =>
        {
            lista.Add(new Talla(fila.GetInt32(0), fila.GetString(1)));
        });

        return lista;
    }

    public int InsertarTalla(string nombre)
    {
        return EjecutarInsert("INSERT INTO Tallas (Nombre) VALUES (@Nombre)",
            new SqlParameter("@Nombre", nombre));
    }

    public void ActualizarTalla(int id, string nombre)
    {
        EjecutarComando("UPDATE Tallas SET Nombre = @Nombre WHERE Id = @Id",
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Id", id));
    }

    public void EliminarTalla(int id)
    {
        EjecutarComando("DELETE FROM Tallas WHERE Id = @Id",
            new SqlParameter("@Id", id));
    }

    // ==============================
    // PROVEEDORES
    // ==============================

    public List<Proveedor> CargarProveedores()
    {
        List<Proveedor> lista = new List<Proveedor>();

        Consultar("SELECT Id, Nombre, Telefono, Email FROM Proveedores ORDER BY Id", fila =>
        {
            lista.Add(new Proveedor(fila.GetInt32(0), fila.GetString(1), fila.GetString(2), fila.GetString(3)));
        });

        return lista;
    }

    public int InsertarProveedor(string nombre, string telefono, string email)
    {
        return EjecutarInsert("INSERT INTO Proveedores (Nombre, Telefono, Email) VALUES (@Nombre, @Telefono, @Email)",
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Telefono", telefono),
            new SqlParameter("@Email", email));
    }

    public void ActualizarProveedor(int id, string nombre, string telefono, string email)
    {
        EjecutarComando("UPDATE Proveedores SET Nombre = @Nombre, Telefono = @Telefono, Email = @Email WHERE Id = @Id",
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Telefono", telefono),
            new SqlParameter("@Email", email),
            new SqlParameter("@Id", id));
    }

    public void EliminarProveedor(int id)
    {
        EjecutarComando("DELETE FROM Proveedores WHERE Id = @Id",
            new SqlParameter("@Id", id));
    }

    // ==============================
    // PRENDAS
    // ==============================

    // Recibe las categorías, tallas y proveedores ya cargados para armar cada Prenda con sus objetos
    public List<Prenda> CargarPrendas(Dictionary<int, Categoria> categorias,
                                      Dictionary<int, Talla> tallas,
                                      Dictionary<int, Proveedor> proveedores)
    {
        List<Prenda> lista = new List<Prenda>();

        Consultar("SELECT Id, Nombre, Precio, Existencias, Descuento, CategoriaId, TallaId, ProveedorId " +
                  "FROM Prendas ORDER BY Id", fila =>
        {
            Prenda prenda = new Prenda(
                fila.GetInt32(0),
                fila.GetString(1),
                fila.GetDecimal(2),
                fila.GetInt32(3),
                categorias[fila.GetInt32(5)],
                tallas[fila.GetInt32(6)],
                proveedores[fila.GetInt32(7)]);

            prenda.Descuento = fila.GetDecimal(4);
            lista.Add(prenda);
        });

        return lista;
    }

    public int InsertarPrenda(Prenda prenda)
    {
        return EjecutarInsert(
            "INSERT INTO Prendas (Nombre, Precio, Existencias, Descuento, CategoriaId, TallaId, ProveedorId) " +
            "VALUES (@Nombre, @Precio, @Existencias, @Descuento, @CategoriaId, @TallaId, @ProveedorId)",
            new SqlParameter("@Nombre", prenda.Nombre),
            new SqlParameter("@Precio", prenda.Precio),
            new SqlParameter("@Existencias", prenda.Existencias),
            new SqlParameter("@Descuento", prenda.Descuento),
            new SqlParameter("@CategoriaId", prenda.Categoria.Id),
            new SqlParameter("@TallaId", prenda.Talla.Id),
            new SqlParameter("@ProveedorId", prenda.Proveedor.Id));
    }

    public void ActualizarPrenda(Prenda prenda)
    {
        EjecutarComando(
            "UPDATE Prendas SET Nombre = @Nombre, Precio = @Precio, Existencias = @Existencias, " +
            "Descuento = @Descuento, CategoriaId = @CategoriaId, TallaId = @TallaId, ProveedorId = @ProveedorId " +
            "WHERE Id = @Id",
            new SqlParameter("@Nombre", prenda.Nombre),
            new SqlParameter("@Precio", prenda.Precio),
            new SqlParameter("@Existencias", prenda.Existencias),
            new SqlParameter("@Descuento", prenda.Descuento),
            new SqlParameter("@CategoriaId", prenda.Categoria.Id),
            new SqlParameter("@TallaId", prenda.Talla.Id),
            new SqlParameter("@ProveedorId", prenda.Proveedor.Id),
            new SqlParameter("@Id", prenda.Id));
    }

    public void EliminarPrenda(int id)
    {
        EjecutarComando("DELETE FROM Prendas WHERE Id = @Id",
            new SqlParameter("@Id", id));
    }

    // ==============================
    // VENTAS
    // ==============================

    public List<Venta> CargarVentas()
    {
        List<Venta> lista = new List<Venta>();

        Consultar("SELECT Id, PrendaId, NombrePrenda, Cantidad, PrecioUnitario, Fecha FROM Ventas ORDER BY Id", fila =>
        {
            lista.Add(new Venta(
                fila.GetInt32(0),
                fila.GetInt32(1),
                fila.GetString(2),
                fila.GetInt32(3),
                fila.GetDecimal(4),
                fila.GetDateTime(5)));
        });

        return lista;
    }

    // Guarda la venta y baja el stock en una sola transacción:
    // o se guardan las dos cosas, o no se guarda ninguna
    public int RegistrarVenta(int prendaId, string nombrePrenda, int cantidad,
                              decimal precioUnitario, int nuevasExistencias, DateTime fecha)
    {
        return EjecutarInsert(
            "SET XACT_ABORT ON; BEGIN TRANSACTION; " +
            "UPDATE Prendas SET Existencias = @Existencias WHERE Id = @PrendaId; " +
            "INSERT INTO Ventas (PrendaId, NombrePrenda, Cantidad, PrecioUnitario, Fecha) " +
            "VALUES (@PrendaId, @NombrePrenda, @Cantidad, @PrecioUnitario, @Fecha); " +
            "COMMIT TRANSACTION",
            new SqlParameter("@Existencias", nuevasExistencias),
            new SqlParameter("@PrendaId", prendaId),
            new SqlParameter("@NombrePrenda", nombrePrenda),
            new SqlParameter("@Cantidad", cantidad),
            new SqlParameter("@PrecioUnitario", precioUnitario),
            new SqlParameter("@Fecha", fecha));
    }
}


// =====================================================
// LÓGICA: inventario conectado a la base de datos
// =====================================================

// Es el mismo Inventario de la Tarea 3, pero ahora conectado a SQL Server:
// al iniciar carga todo desde la base de datos y cada cambio también se guarda en ella.
// Esta clase NO usa Console: recibe datos, hace el trabajo y devuelve resultados o lanza errores.
public class Inventario
{
    private BaseDatos baseDatos;

    private Dictionary<int, Categoria> categorias = new Dictionary<int, Categoria>();
    private Dictionary<int, Talla> tallas = new Dictionary<int, Talla>();
    private Dictionary<int, Proveedor> proveedores = new Dictionary<int, Proveedor>();
    private List<Prenda> prendas = new List<Prenda>();
    private List<Venta> ventas = new List<Venta>();

    public Inventario(BaseDatos baseDatos)
    {
        this.baseDatos = baseDatos;

        foreach (Categoria categoria in baseDatos.CargarCategorias())
        {
            categorias.Add(categoria.Id, categoria);
        }

        foreach (Talla talla in baseDatos.CargarTallas())
        {
            tallas.Add(talla.Id, talla);
        }

        foreach (Proveedor proveedor in baseDatos.CargarProveedores())
        {
            proveedores.Add(proveedor.Id, proveedor);
        }

        prendas = baseDatos.CargarPrendas(categorias, tallas, proveedores);
        ventas = baseDatos.CargarVentas();
    }

    // ==============================
    // LECTURA DE DATOS
    // ==============================

    public IEnumerable<Categoria> Categorias
    {
        get { return categorias.Values; }
    }

    public IEnumerable<Talla> Tallas
    {
        get { return tallas.Values; }
    }

    public IEnumerable<Proveedor> Proveedores
    {
        get { return proveedores.Values; }
    }

    public IEnumerable<Prenda> Prendas
    {
        get { return prendas; }
    }

    public IEnumerable<Venta> Ventas
    {
        get { return ventas; }
    }

    // ==============================
    // CATEGORÍAS
    // ==============================

    public Categoria AgregarCategoria(string nombre)
    {
        // Se crea una copia temporal solo para validar el nombre
        Categoria validada = new Categoria(0, nombre);

        if (categorias.Values.Any(c => c.Nombre.Equals(validada.Nombre, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        int id = baseDatos.InsertarCategoria(validada.Nombre);
        Categoria categoria = new Categoria(id, validada.Nombre);
        categorias.Add(id, categoria);

        return categoria;
    }

    public Categoria? ObtenerCategoria(int id)
    {
        categorias.TryGetValue(id, out Categoria? categoria);
        return categoria;
    }

    public void ModificarCategoria(int id, string nuevoNombre)
    {
        Categoria? categoria = ObtenerCategoria(id);

        if (categoria == null)
        {
            throw new InvalidOperationException("La categoría no existe.");
        }

        if (categorias.Values.Any(c => c.Id != id &&
            c.Nombre.Equals(nuevoNombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");
        }

        categoria.Nombre = nuevoNombre;
        baseDatos.ActualizarCategoria(id, categoria.Nombre);
    }

    public void EliminarCategoria(int id)
    {
        if (!categorias.ContainsKey(id))
        {
            throw new InvalidOperationException("La categoría no existe.");
        }

        if (prendas.Any(p => p.Categoria.Id == id))
        {
            throw new InvalidOperationException("No se puede eliminar: hay prendas que usan esta categoría.");
        }

        baseDatos.EliminarCategoria(id);
        categorias.Remove(id);
    }

    // ==============================
    // TALLAS
    // ==============================

    public Talla AgregarTalla(string nombre)
    {
        Talla validada = new Talla(0, nombre);

        if (tallas.Values.Any(t => t.Nombre.Equals(validada.Nombre, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe una talla con ese nombre.");
        }

        int id = baseDatos.InsertarTalla(validada.Nombre);
        Talla talla = new Talla(id, validada.Nombre);
        tallas.Add(id, talla);

        return talla;
    }

    public Talla? ObtenerTalla(int id)
    {
        tallas.TryGetValue(id, out Talla? talla);
        return talla;
    }

    public void ModificarTalla(int id, string nuevoNombre)
    {
        Talla? talla = ObtenerTalla(id);

        if (talla == null)
        {
            throw new InvalidOperationException("La talla no existe.");
        }

        if (tallas.Values.Any(t => t.Id != id &&
            t.Nombre.Equals(nuevoNombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe otra talla con ese nombre.");
        }

        talla.Nombre = nuevoNombre;
        baseDatos.ActualizarTalla(id, talla.Nombre);
    }

    public void EliminarTalla(int id)
    {
        if (!tallas.ContainsKey(id))
        {
            throw new InvalidOperationException("La talla no existe.");
        }

        if (prendas.Any(p => p.Talla.Id == id))
        {
            throw new InvalidOperationException("No se puede eliminar: hay prendas que usan esta talla.");
        }

        baseDatos.EliminarTalla(id);
        tallas.Remove(id);
    }

    // ==============================
    // PROVEEDORES
    // ==============================

    public Proveedor AgregarProveedor(string nombre, string telefono, string email)
    {
        Proveedor validado = new Proveedor(0, nombre, telefono, email);

        if (proveedores.Values.Any(p => p.Nombre.Equals(validado.Nombre, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese nombre.");
        }

        int id = baseDatos.InsertarProveedor(validado.Nombre, validado.Telefono, validado.Email);
        Proveedor proveedor = new Proveedor(id, validado.Nombre, validado.Telefono, validado.Email);
        proveedores.Add(id, proveedor);

        return proveedor;
    }

    public Proveedor? ObtenerProveedor(int id)
    {
        proveedores.TryGetValue(id, out Proveedor? proveedor);
        return proveedor;
    }

    public void ModificarProveedor(int id, string nuevoNombre, string nuevoTelefono, string nuevoEmail)
    {
        Proveedor? proveedor = ObtenerProveedor(id);

        if (proveedor == null)
        {
            throw new InvalidOperationException("El proveedor no existe.");
        }

        if (proveedores.Values.Any(p => p.Id != id &&
            p.Nombre.Equals(nuevoNombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe otro proveedor con ese nombre.");
        }

        proveedor.Nombre = nuevoNombre;
        proveedor.Telefono = nuevoTelefono;
        proveedor.Email = nuevoEmail;
        baseDatos.ActualizarProveedor(id, proveedor.Nombre, proveedor.Telefono, proveedor.Email);
    }

    public void EliminarProveedor(int id)
    {
        if (!proveedores.ContainsKey(id))
        {
            throw new InvalidOperationException("El proveedor no existe.");
        }

        if (prendas.Any(p => p.Proveedor.Id == id))
        {
            throw new InvalidOperationException("No se puede eliminar: hay prendas de este proveedor.");
        }

        baseDatos.EliminarProveedor(id);
        proveedores.Remove(id);
    }

    // ==============================
    // PRENDAS
    // ==============================

    public Prenda AgregarPrenda(string nombre, decimal precio, int existencias,
                                Categoria categoria, Talla talla, Proveedor proveedor)
    {
        // Copia temporal solo para validar nombre, precio y existencias
        Prenda validada = new Prenda(0, nombre, precio, existencias, categoria, talla, proveedor);

        if (prendas.Any(p => p.Nombre.Equals(validada.Nombre, StringComparison.OrdinalIgnoreCase) &&
                             p.Categoria.Id == categoria.Id &&
                             p.Talla.Id == talla.Id))
        {
            throw new InvalidOperationException("Ya existe esa prenda con la misma categoría y talla.");
        }

        int id = baseDatos.InsertarPrenda(validada);
        Prenda prenda = new Prenda(id, validada.Nombre, validada.Precio, validada.Existencias,
                                   categoria, talla, proveedor);
        prendas.Add(prenda);

        return prenda;
    }

    public Prenda? ObtenerPrenda(int id)
    {
        return prendas.FirstOrDefault(p => p.Id == id);
    }

    // Guarda en la base de datos los cambios hechos a una prenda (precio, stock, descuento...)
    public void GuardarPrenda(Prenda prenda)
    {
        baseDatos.ActualizarPrenda(prenda);
    }

    public List<Prenda> BuscarPorNombre(string texto)
    {
        return prendas.Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Prenda> BuscarPorCategoria(Categoria categoria)
    {
        return prendas.Where(p => p.Categoria.Id == categoria.Id).ToList();
    }

    public List<Prenda> BuscarPorTalla(Talla talla)
    {
        return prendas.Where(p => p.Talla.Id == talla.Id).ToList();
    }

    public List<Prenda> BuscarPorProveedor(Proveedor proveedor)
    {
        return prendas.Where(p => p.Proveedor.Id == proveedor.Id).ToList();
    }

    // Busca según el precio final (con descuento)
    public List<Prenda> BuscarPorRangoPrecio(decimal minimo, decimal maximo)
    {
        return prendas.Where(p => p.PrecioFinal >= minimo && p.PrecioFinal <= maximo).ToList();
    }

    public void EliminarPrenda(int id)
    {
        Prenda? prenda = ObtenerPrenda(id);

        if (prenda == null)
        {
            throw new InvalidOperationException("La prenda no existe.");
        }

        baseDatos.EliminarPrenda(id);
        prendas.Remove(prenda);
    }

    // Aplica el mismo descuento a todas las prendas de una categoría (una oferta)
    public int AplicarDescuentoACategoria(Categoria categoria, decimal porcentaje)
    {
        int contador = 0;

        foreach (Prenda prenda in prendas)
        {
            if (prenda.Categoria.Id == categoria.Id)
            {
                prenda.Descuento = porcentaje;
                baseDatos.ActualizarPrenda(prenda);
                contador++;
            }
        }

        return contador;
    }

    // ==============================
    // VENTAS
    // ==============================

    public Venta RegistrarVenta(int prendaId, int cantidad)
    {
        Prenda? prenda = ObtenerPrenda(prendaId);

        if (prenda == null)
        {
            throw new InvalidOperationException("La prenda no existe.");
        }

        prenda.RetirarExistencias(cantidad);
        DateTime fecha = DateTime.Now;

        try
        {
            int id = baseDatos.RegistrarVenta(prenda.Id, prenda.Nombre, cantidad,
                                              prenda.PrecioFinal, prenda.Existencias, fecha);

            Venta venta = new Venta(id, prenda.Id, prenda.Nombre, cantidad, prenda.PrecioFinal, fecha);
            ventas.Add(venta);

            return venta;
        }
        catch
        {
            // Si la base de datos falla, se devuelve el stock para que todo quede igual que antes
            prenda.AgregarExistencias(cantidad);
            throw;
        }
    }

    // ==============================
    // REPORTES
    // ==============================

    public List<Prenda> PrendasConStockBajo(int limite)
    {
        return prendas.Where(p => p.Existencias <= limite).ToList();
    }

    public int UnidadesTotales()
    {
        return prendas.Sum(p => p.Existencias);
    }

    public decimal ValorInventario()
    {
        return prendas.Sum(p => p.PrecioFinal * p.Existencias);
    }

    public decimal TotalVendido()
    {
        return ventas.Sum(v => v.Total);
    }
}


// =====================================================
// INTERFAZ: menús de consola
// =====================================================

// Esta clase se encarga de TODO lo que el usuario ve y escribe (Console).
// La lógica de verdad está en la clase Inventario.
public class MenuBoutique
{
    private Inventario inventario;

    public MenuBoutique(Inventario inventario)
    {
        this.inventario = inventario;
    }

    // ==============================
    // FUNCIONES DE AYUDA
    // ==============================

    private string LeerTexto(string mensaje)
    {
        string valor;

        do
        {
            Console.Write(mensaje);
            valor = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(valor))
            {
                Console.WriteLine("Este campo no puede estar vacío.");
            }

        } while (string.IsNullOrWhiteSpace(valor));

        return valor.Trim();
    }

    private int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (int.TryParse(Console.ReadLine(), out int numero))
            {
                return numero;
            }

            Console.WriteLine("Entrada inválida. Escriba un número entero.");
        }
    }

    private decimal LeerDecimal(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (decimal.TryParse(Console.ReadLine(), out decimal numero) && numero >= 0)
            {
                return numero;
            }

            Console.WriteLine("Entrada inválida. Escriba un número válido (no negativo).");
        }
    }

    private bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (S/N): ");
        string respuesta = Console.ReadLine() ?? "";

        return respuesta.Trim().Equals("S", StringComparison.OrdinalIgnoreCase);
    }

    private void Pausa()
    {
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    // Ejecuta una acción y, si algo falla, muestra el error en vez de cerrar el programa
    private void Ejecutar(Action accion)
    {
        try
        {
            accion();
        }
        catch (Exception error)
        {
            Console.WriteLine("\nError: " + error.Message);
            Pausa();
        }
    }

    private void MostrarElementos(IEnumerable<ElementoCatalogo> elementos)
    {
        if (!elementos.Any())
        {
            Console.WriteLine("No hay registros.");
            return;
        }

        foreach (ElementoCatalogo elemento in elementos)
        {
            Console.WriteLine(elemento);
        }
    }

    private void MostrarPrendas(IEnumerable<Prenda> lista)
    {
        if (!lista.Any())
        {
            Console.WriteLine("No hay prendas para mostrar.");
            return;
        }

        foreach (Prenda prenda in lista)
        {
            Console.WriteLine(prenda);
        }

        Console.WriteLine("\nTotal: " + lista.Count());
    }

    private Categoria ElegirCategoria()
    {
        Console.WriteLine("\nCategorías disponibles:");
        MostrarElementos(inventario.Categorias);

        int id = LeerEntero("Id de la categoría: ");
        Categoria? categoria = inventario.ObtenerCategoria(id);

        if (categoria == null)
        {
            throw new InvalidOperationException("Esa categoría no existe.");
        }

        return categoria;
    }

    private Talla ElegirTalla()
    {
        Console.WriteLine("\nTallas disponibles:");
        MostrarElementos(inventario.Tallas);

        int id = LeerEntero("Id de la talla: ");
        Talla? talla = inventario.ObtenerTalla(id);

        if (talla == null)
        {
            throw new InvalidOperationException("Esa talla no existe.");
        }

        return talla;
    }

    private Proveedor ElegirProveedor()
    {
        Console.WriteLine("\nProveedores disponibles:");
        MostrarElementos(inventario.Proveedores);

        int id = LeerEntero("Id del proveedor: ");
        Proveedor? proveedor = inventario.ObtenerProveedor(id);

        if (proveedor == null)
        {
            throw new InvalidOperationException("Ese proveedor no existe.");
        }

        return proveedor;
    }

    private Prenda ElegirPrenda()
    {
        Console.WriteLine("\nPrendas registradas:");
        MostrarPrendas(inventario.Prendas);

        int id = LeerEntero("\nId de la prenda: ");
        Prenda? prenda = inventario.ObtenerPrenda(id);

        if (prenda == null)
        {
            throw new InvalidOperationException("Esa prenda no existe.");
        }

        return prenda;
    }

    // Menú reutilizable para categorías, tallas y proveedores
    private void MenuCatalogo(string titulo, Action agregar, Action listar, Action modificar, Action eliminar)
    {
        bool seguirEnMenu = true;

        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== " + titulo + " =====");
            Console.WriteLine("1. Agregar");
            Console.WriteLine("2. Listar");
            Console.WriteLine("3. Modificar");
            Console.WriteLine("4. Eliminar");
            Console.WriteLine("5. Volver al menú principal");

            int opcion = LeerEntero("\nSeleccione una opción: ");

            switch (opcion)
            {
                case 1: Ejecutar(agregar); break;
                case 2: Ejecutar(listar); break;
                case 3: Ejecutar(modificar); break;
                case 4: Ejecutar(eliminar); break;
                case 5: seguirEnMenu = false; break;

                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }

    // ==============================
    // CATEGORÍAS
    // ==============================

    private void AgregarCategoria()
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR CATEGORÍA =====");

        string nombre = LeerTexto("Nombre: ");
        Categoria categoria = inventario.AgregarCategoria(nombre);

        Console.WriteLine("\nCategoría agregada. Id: " + categoria.Id);
        Pausa();
    }

    private void ListarCategorias()
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE CATEGORÍAS =====");
        MostrarElementos(inventario.Categorias);
        Pausa();
    }

    private void ModificarCategoria()
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR CATEGORÍA =====");
        MostrarElementos(inventario.Categorias);

        int id = LeerEntero("\nId a modificar: ");
        string nuevoNombre = LeerTexto("Nuevo nombre: ");
        inventario.ModificarCategoria(id, nuevoNombre);

        Console.WriteLine("\nCategoría modificada.");
        Pausa();
    }

    private void EliminarCategoria()
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR CATEGORÍA =====");
        MostrarElementos(inventario.Categorias);

        int id = LeerEntero("\nId a eliminar: ");

        if (Confirmar("¿Confirma la eliminación?"))
        {
            inventario.EliminarCategoria(id);
            Console.WriteLine("Categoría eliminada.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }

        Pausa();
    }

    // ==============================
    // TALLAS
    // ==============================

    private void AgregarTalla()
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR TALLA =====");

        string nombre = LeerTexto("Nombre (por ejemplo S, M, 38): ");
        Talla talla = inventario.AgregarTalla(nombre);

        Console.WriteLine("\nTalla agregada. Id: " + talla.Id);
        Pausa();
    }

    private void ListarTallas()
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE TALLAS =====");
        MostrarElementos(inventario.Tallas);
        Pausa();
    }

    private void ModificarTalla()
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR TALLA =====");
        MostrarElementos(inventario.Tallas);

        int id = LeerEntero("\nId a modificar: ");
        string nuevoNombre = LeerTexto("Nuevo nombre: ");
        inventario.ModificarTalla(id, nuevoNombre);

        Console.WriteLine("\nTalla modificada.");
        Pausa();
    }

    private void EliminarTalla()
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR TALLA =====");
        MostrarElementos(inventario.Tallas);

        int id = LeerEntero("\nId a eliminar: ");

        if (Confirmar("¿Confirma la eliminación?"))
        {
            inventario.EliminarTalla(id);
            Console.WriteLine("Talla eliminada.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }

        Pausa();
    }

    // ==============================
    // PROVEEDORES
    // ==============================

    private void AgregarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR PROVEEDOR =====");

        string nombre = LeerTexto("Nombre: ");
        string telefono = LeerTexto("Teléfono: ");
        string email = LeerTexto("Email: ");
        Proveedor proveedor = inventario.AgregarProveedor(nombre, telefono, email);

        Console.WriteLine("\nProveedor agregado. Id: " + proveedor.Id);
        Pausa();
    }

    private void ListarProveedores()
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE PROVEEDORES =====");
        MostrarElementos(inventario.Proveedores);
        Pausa();
    }

    private void ModificarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR PROVEEDOR =====");
        MostrarElementos(inventario.Proveedores);

        int id = LeerEntero("\nId a modificar: ");
        string nuevoNombre = LeerTexto("Nuevo nombre: ");
        string nuevoTelefono = LeerTexto("Nuevo teléfono: ");
        string nuevoEmail = LeerTexto("Nuevo email: ");
        inventario.ModificarProveedor(id, nuevoNombre, nuevoTelefono, nuevoEmail);

        Console.WriteLine("\nProveedor modificado.");
        Pausa();
    }

    private void EliminarProveedor()
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR PROVEEDOR =====");
        MostrarElementos(inventario.Proveedores);

        int id = LeerEntero("\nId a eliminar: ");

        if (Confirmar("¿Confirma la eliminación?"))
        {
            inventario.EliminarProveedor(id);
            Console.WriteLine("Proveedor eliminado.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }

        Pausa();
    }

    // ==============================
    // PRENDAS
    // ==============================

    private void AgregarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR PRENDA =====");

        string nombre = LeerTexto("Nombre de la prenda: ");
        decimal precio = LeerDecimal("Precio: ");
        int existencias = LeerEntero("Existencias: ");
        Categoria categoria = ElegirCategoria();
        Talla talla = ElegirTalla();
        Proveedor proveedor = ElegirProveedor();

        Prenda prenda = inventario.AgregarPrenda(nombre, precio, existencias, categoria, talla, proveedor);

        Console.WriteLine("\nPrenda agregada. Id: " + prenda.Id);
        Pausa();
    }

    private void ListarPrendas()
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE PRENDAS =====");
        MostrarPrendas(inventario.Prendas);
        Pausa();
    }

    private void BuscarPrendas()
    {
        Console.Clear();
        Console.WriteLine("===== BUSCAR PRENDAS =====");
        Console.WriteLine("1. Por Id");
        Console.WriteLine("2. Por nombre");
        Console.WriteLine("3. Por categoría");
        Console.WriteLine("4. Por talla");
        Console.WriteLine("5. Por proveedor");
        Console.WriteLine("6. Por rango de precio");

        int criterio = LeerEntero("\nSeleccione el criterio: ");
        List<Prenda> resultado;

        switch (criterio)
        {
            case 1:
            {
                int id = LeerEntero("Id de la prenda: ");
                Prenda? prenda = inventario.ObtenerPrenda(id);
                resultado = new List<Prenda>();

                if (prenda != null)
                {
                    resultado.Add(prenda);
                }

                break;
            }

            case 2:
                resultado = inventario.BuscarPorNombre(LeerTexto("Nombre o parte del nombre: "));
                break;

            case 3:
                resultado = inventario.BuscarPorCategoria(ElegirCategoria());
                break;

            case 4:
                resultado = inventario.BuscarPorTalla(ElegirTalla());
                break;

            case 5:
                resultado = inventario.BuscarPorProveedor(ElegirProveedor());
                break;

            case 6:
            {
                decimal minimo = LeerDecimal("Precio mínimo: ");
                decimal maximo = LeerDecimal("Precio máximo: ");

                if (maximo < minimo)
                {
                    throw new InvalidOperationException("El precio máximo no puede ser menor que el mínimo.");
                }

                resultado = inventario.BuscarPorRangoPrecio(minimo, maximo);
                break;
            }

            default:
                Console.WriteLine("Criterio inválido.");
                Pausa();
                return;
        }

        Console.WriteLine();
        MostrarPrendas(resultado);
        Pausa();
    }

    private void ModificarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR PRENDA =====");

        Prenda prenda = ElegirPrenda();

        Console.WriteLine("\n¿Qué desea modificar?");
        Console.WriteLine("1. Nombre");
        Console.WriteLine("2. Precio");
        Console.WriteLine("3. Existencias");
        Console.WriteLine("4. Categoría");
        Console.WriteLine("5. Talla");
        Console.WriteLine("6. Proveedor");

        int campo = LeerEntero("\nSeleccione una opción: ");

        switch (campo)
        {
            case 1: prenda.Nombre = LeerTexto("Nuevo nombre: "); break;
            case 2: prenda.Precio = LeerDecimal("Nuevo precio: "); break;
            case 3: prenda.Existencias = LeerEntero("Nuevas existencias: "); break;
            case 4: prenda.Categoria = ElegirCategoria(); break;
            case 5: prenda.Talla = ElegirTalla(); break;
            case 6: prenda.Proveedor = ElegirProveedor(); break;

            default:
                Console.WriteLine("Opción inválida.");
                Pausa();
                return;
        }

        inventario.GuardarPrenda(prenda);
        Console.WriteLine("\nPrenda modificada.");
        Pausa();
    }

    private void EliminarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR PRENDA =====");

        Prenda prenda = ElegirPrenda();

        if (Confirmar("¿Confirma la eliminación?"))
        {
            inventario.EliminarPrenda(prenda.Id);
            Console.WriteLine("Prenda eliminada.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }

        Pausa();
    }

    private void AplicarDescuentoPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== DESCUENTO A UNA PRENDA =====");

        Prenda prenda = ElegirPrenda();
        prenda.Descuento = LeerDecimal("Descuento en % (de 0 a 100): ");
        inventario.GuardarPrenda(prenda);

        Console.WriteLine("\nNuevo precio final: $" + prenda.PrecioFinal.ToString("N2"));
        Pausa();
    }

    private void AplicarDescuentoCategoria()
    {
        Console.Clear();
        Console.WriteLine("===== OFERTA A UNA CATEGORÍA =====");

        Categoria categoria = ElegirCategoria();
        decimal porcentaje = LeerDecimal("Descuento en % (de 0 a 100): ");
        int cantidad = inventario.AplicarDescuentoACategoria(categoria, porcentaje);

        Console.WriteLine("\nSe actualizaron " + cantidad + " prenda(s).");
        Pausa();
    }

    private void MenuPrendas()
    {
        bool seguirEnMenu = true;

        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== PRENDAS =====");
            Console.WriteLine("1. Agregar prenda");
            Console.WriteLine("2. Listar prendas");
            Console.WriteLine("3. Buscar prendas");
            Console.WriteLine("4. Modificar prenda");
            Console.WriteLine("5. Eliminar prenda");
            Console.WriteLine("6. Descuento a una prenda");
            Console.WriteLine("7. Oferta a una categoría");
            Console.WriteLine("8. Volver al menú principal");

            int opcion = LeerEntero("\nSeleccione una opción: ");

            switch (opcion)
            {
                case 1: Ejecutar(AgregarPrenda); break;
                case 2: Ejecutar(ListarPrendas); break;
                case 3: Ejecutar(BuscarPrendas); break;
                case 4: Ejecutar(ModificarPrenda); break;
                case 5: Ejecutar(EliminarPrenda); break;
                case 6: Ejecutar(AplicarDescuentoPrenda); break;
                case 7: Ejecutar(AplicarDescuentoCategoria); break;
                case 8: seguirEnMenu = false; break;

                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }

    // ==============================
    // VENTAS Y EXISTENCIAS
    // ==============================

    private void RegistrarVenta()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR VENTA =====");

        Prenda prenda = ElegirPrenda();

        Console.WriteLine("\nPrecio final: $" + prenda.PrecioFinal.ToString("N2"));
        Console.WriteLine("Existencias disponibles: " + prenda.Existencias);

        int cantidad = LeerEntero("Cantidad a vender: ");
        Venta venta = inventario.RegistrarVenta(prenda.Id, cantidad);

        Console.WriteLine("\nVenta registrada. Total: $" + venta.Total.ToString("N2"));
        Pausa();
    }

    private void RegistrarEntrada()
    {
        Console.Clear();
        Console.WriteLine("===== ENTRADA DE MERCANCÍA =====");

        Prenda prenda = ElegirPrenda();
        int cantidad = LeerEntero("Cantidad que ingresa: ");
        prenda.AgregarExistencias(cantidad);
        inventario.GuardarPrenda(prenda);

        Console.WriteLine("\nExistencias actuales: " + prenda.Existencias);
        Pausa();
    }

    private void MostrarHistorialVentas()
    {
        Console.Clear();
        Console.WriteLine("===== HISTORIAL DE VENTAS =====");

        if (!inventario.Ventas.Any())
        {
            Console.WriteLine("Todavía no hay ventas.");
        }
        else
        {
            foreach (Venta venta in inventario.Ventas)
            {
                Console.WriteLine(venta);
            }

            Console.WriteLine("\nTotal vendido: $" + inventario.TotalVendido().ToString("N2"));
        }

        Pausa();
    }

    private void MenuVentas()
    {
        bool seguirEnMenu = true;

        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== VENTAS Y EXISTENCIAS =====");
            Console.WriteLine("1. Registrar venta");
            Console.WriteLine("2. Registrar entrada de mercancía");
            Console.WriteLine("3. Historial de ventas");
            Console.WriteLine("4. Volver al menú principal");

            int opcion = LeerEntero("\nSeleccione una opción: ");

            switch (opcion)
            {
                case 1: Ejecutar(RegistrarVenta); break;
                case 2: Ejecutar(RegistrarEntrada); break;
                case 3: Ejecutar(MostrarHistorialVentas); break;
                case 4: seguirEnMenu = false; break;

                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }

    // ==============================
    // REPORTES
    // ==============================

    private void MostrarStockBajo()
    {
        Console.Clear();
        Console.WriteLine("===== PRENDAS CON STOCK BAJO =====");

        int limite = LeerEntero("Mostrar prendas con existencias menores o iguales a: ");
        MostrarPrendas(inventario.PrendasConStockBajo(limite));
        Pausa();
    }

    private void MostrarValorInventario()
    {
        Console.Clear();
        Console.WriteLine("===== VALOR DEL INVENTARIO =====");
        Console.WriteLine("Prendas distintas: " + inventario.Prendas.Count());
        Console.WriteLine("Unidades totales: " + inventario.UnidadesTotales());
        Console.WriteLine("Valor total: $" + inventario.ValorInventario().ToString("N2"));
        Pausa();
    }

    private void MostrarTotalVendido()
    {
        Console.Clear();
        Console.WriteLine("===== TOTAL VENDIDO =====");
        Console.WriteLine("Ventas registradas: " + inventario.Ventas.Count());
        Console.WriteLine("Total vendido: $" + inventario.TotalVendido().ToString("N2"));
        Pausa();
    }

    private void MenuReportes()
    {
        bool seguirEnMenu = true;

        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== REPORTES =====");
            Console.WriteLine("1. Prendas con stock bajo");
            Console.WriteLine("2. Valor del inventario");
            Console.WriteLine("3. Total vendido");
            Console.WriteLine("4. Volver al menú principal");

            int opcion = LeerEntero("\nSeleccione una opción: ");

            switch (opcion)
            {
                case 1: Ejecutar(MostrarStockBajo); break;
                case 2: Ejecutar(MostrarValorInventario); break;
                case 3: Ejecutar(MostrarTotalVendido); break;
                case 4: seguirEnMenu = false; break;

                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }

    // ==============================
    // MENÚ PRINCIPAL
    // ==============================

    public void Iniciar()
    {
        bool ejecutando = true;

        while (ejecutando)
        {
            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine("        REGISTRO DE ROPA - BOUTIQUE");
            Console.WriteLine("======================================");
            Console.WriteLine("1. Categorías");
            Console.WriteLine("2. Tallas");
            Console.WriteLine("3. Proveedores");
            Console.WriteLine("4. Prendas");
            Console.WriteLine("5. Ventas y existencias");
            Console.WriteLine("6. Reportes");
            Console.WriteLine("7. Salir");
            Console.WriteLine("======================================");

            int opcion = LeerEntero("\nSeleccione una opción: ");

            switch (opcion)
            {
                case 1:
                    MenuCatalogo("CATEGORÍAS", AgregarCategoria, ListarCategorias, ModificarCategoria, EliminarCategoria);
                    break;

                case 2:
                    MenuCatalogo("TALLAS", AgregarTalla, ListarTallas, ModificarTalla, EliminarTalla);
                    break;

                case 3:
                    MenuCatalogo("PROVEEDORES", AgregarProveedor, ListarProveedores, ModificarProveedor, EliminarProveedor);
                    break;

                case 4:
                    MenuPrendas();
                    break;

                case 5:
                    MenuVentas();
                    break;

                case 6:
                    MenuReportes();
                    break;

                case 7:
                    if (Confirmar("¿Seguro que desea salir?"))
                    {
                        ejecutando = false;
                        Console.WriteLine("\nGracias por usar el sistema. ¡Hasta luego!");
                    }
                    break;

                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    Pausa();
                    break;
            }
        }
    }
}
