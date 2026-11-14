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

