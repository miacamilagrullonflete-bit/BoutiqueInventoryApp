try
{
    
    Dictionary<int, string> categorias = new Dictionary<int, string>();
    Dictionary<int, string> tallas = new Dictionary<int, string>();

    List<int> prendaIds = new List<int>();
    List<string> prendaNombres = new List<string>();
    List<decimal> prendaPrecios = new List<decimal>();
    List<int> prendaExistencias = new List<int>();
    List<int> prendaCategoriaIds = new List<int>();
    List<int> prendaTallaIds = new List<int>();

    int siguientePrendaId = 1;

    categorias.Add(1, "Camisas");
    categorias.Add(2, "Pantalones");
    categorias.Add(3, "Vestidos");

    tallas.Add(1, "S");
    tallas.Add(2, "M");
    tallas.Add(3, "L");
    tallas.Add(4, "XL");

    string LeerTexto(string mensaje)
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

    int LeerEntero(string mensaje)
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

    decimal LeerDecimal(string mensaje)
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

    bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (S/N): ");
        string respuesta = Console.ReadLine() ?? "";

        return respuesta.Trim().Equals("S", StringComparison.OrdinalIgnoreCase);
    }

    void Pausa()
    {
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    int ElegirDeDiccionario(Dictionary<int, string> diccionario, string titulo)
    {
        Console.WriteLine("\n" + titulo);

        foreach (KeyValuePair<int, string> par in diccionario)
        {
            Console.WriteLine(par.Key + " - " + par.Value);
        }

        int id = LeerEntero("Escriba el Id: ");

        if (!diccionario.ContainsKey(id))
        {
            Console.WriteLine("Ese Id no existe.");
            return -1;
        }

        return id;
    }

    // >>> AQUÍ VAN LAS SIGUIENTES PARTES (pégalas ARRIBA de esta línea) <<<

    // ==============================
    // MENÚ PRINCIPAL
    // ==============================

    bool ejecutando = true;

    while (ejecutando)
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine("        REGISTRO DE ROPA - BOUTIQUE");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Categorías");
        Console.WriteLine("2. Tallas");
        Console.WriteLine("3. Prendas");
        Console.WriteLine("4. Existencias y reportes");
        Console.WriteLine("5. Salir");
        Console.WriteLine("======================================");

        int opcion = LeerEntero("\nSeleccione una opción: ");

        switch (opcion)
        {
            case 1: Console.WriteLine("En construcción."); Pausa(); break; // se reemplaza en la Parte 2
            case 2: Console.WriteLine("En construcción."); Pausa(); break; // se reemplaza en la Parte 2
            case 3: Console.WriteLine("En construcción."); Pausa(); break; // se reemplaza en la Parte 3
            case 4: Console.WriteLine("En construcción."); Pausa(); break; // se reemplaza en la Parte 6

            case 5:
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
catch (Exception error)
{
    Console.WriteLine("\nOcurrió un error inesperado: " + error.Message);
}