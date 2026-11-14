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
