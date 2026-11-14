public class Inventario
{
    private Dictionary<int, Categoria> categorias = new Dictionary<int, Categoria>();
    private Dictionary<int, Talla> tallas = new Dictionary<int, Talla>();
    private Dictionary<int, Proveedor> proveedores = new Dictionary<int, Proveedor>();
    private List<Prenda> prendas = new List<Prenda>();
    private List<Venta> ventas = new List<Venta>();

    private int siguienteCategoriaId = 1;
    private int siguienteTallaId = 1;
    private int siguienteProveedorId = 1;
    private int siguientePrendaId = 1;
    private int siguienteVentaId = 1;

    public Inventario()
    {
        AgregarCategoria("Camisas");
        AgregarCategoria("Pantalones");
        AgregarCategoria("Vestidos");
        AgregarCategoria("Faldas");
        AgregarCategoria("Zapatos");

        AgregarTalla("S");
        AgregarTalla("M");
        AgregarTalla("L");
        AgregarTalla("XL");
        AgregarTalla("36");
        AgregarTalla("38");
        AgregarTalla("40");

        AgregarProveedor("Moda Express", "809-555-0101", "ventas@modaexpress.com");
    }

   
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


    public Categoria AgregarCategoria(string nombre)
    {
        if (categorias.Values.Any(c => c.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        Categoria categoria = new Categoria(siguienteCategoriaId, nombre);
        categorias.Add(categoria.Id, categoria);
        siguienteCategoriaId++;

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

        categorias.Remove(id);
    }


    public Talla AgregarTalla(string nombre)
    {
        if (tallas.Values.Any(t => t.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe una talla con ese nombre.");
        }

        Talla talla = new Talla(siguienteTallaId, nombre);
        tallas.Add(talla.Id, talla);
        siguienteTallaId++;

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

        tallas.Remove(id);
    }


    public Proveedor AgregarProveedor(string nombre, string telefono, string email)
    {
        if (proveedores.Values.Any(p => p.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese nombre.");
        }

        Proveedor proveedor = new Proveedor(siguienteProveedorId, nombre, telefono, email);
        proveedores.Add(proveedor.Id, proveedor);
        siguienteProveedorId++;

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

        proveedores.Remove(id);
    }


    public Prenda AgregarPrenda(string nombre, decimal precio, int existencias,
    Categoria categoria, Talla talla, Proveedor proveedor)
    {
        if (prendas.Any(p => p.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
        p.Categoria.Id == categoria.Id &&
        p.Talla.Id == talla.Id))
        {
            throw new InvalidOperationException("Ya existe esa prenda con la misma categoría y talla.");
        }

        Prenda prenda = new Prenda(siguientePrendaId, nombre, precio, existencias,
        categoria, talla, proveedor);
        prendas.Add(prenda);
        siguientePrendaId++;

        return prenda;
    }

    public Prenda? ObtenerPrenda(int id)
    {
        return prendas.FirstOrDefault(p => p.Id == id);
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

        prendas.Remove(prenda);
    }

    public int AplicarDescuentoACategoria(Categoria categoria, decimal porcentaje)
    {
        int contador = 0;

        foreach (Prenda prenda in prendas)
        {
            if (prenda.Categoria.Id == categoria.Id)
            {
                prenda.Descuento = porcentaje;
                contador++;
            }
        }

        return contador;
    }

    public Venta RegistrarVenta(int prendaId, int cantidad)
    {
        Prenda? prenda = ObtenerPrenda(prendaId);

        if (prenda == null)
        {
            throw new InvalidOperationException("La prenda no existe.");
        }

        prenda.RetirarExistencias(cantidad);

        Venta venta = new Venta(siguienteVentaId, prenda.Id, prenda.Nombre, cantidad, prenda.PrecioFinal);
        ventas.Add(venta);
        siguienteVentaId++;

        return venta;
    }

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
