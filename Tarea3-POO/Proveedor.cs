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