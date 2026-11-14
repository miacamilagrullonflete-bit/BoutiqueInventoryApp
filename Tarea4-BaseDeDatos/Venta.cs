public class Venta
{
    public int Id { get; }
    public int PrendaId { get; }
    public string NombrePrenda { get; }
    public int Cantidad { get; }
    public decimal PrecioUnitario { get; }
    public DateTime Fecha { get; }

    public decimal Total { get { return PrecioUnitario * Cantidad; } }

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