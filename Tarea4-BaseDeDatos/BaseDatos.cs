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
