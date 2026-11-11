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
    
    void AgregarCatalogo(Dictionary<int, string> catalogo, string singular)
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR " + singular.ToUpper() + " =====");
        string nombre = LeerTexto("Nombre: ");
        if (catalogo.Values.Contains(nombre, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Ya existe una " + singular + " con ese nombre.");
            Pausa();

            return;
        }
        int id = catalogo.Count == 0 ? 1 : catalogo.Keys.Max() + 1;
        catalogo.Add(id, nombre);
        Console.WriteLine("\nRegistro agregado correctamente. Id: " + id);
        Pausa();
    }
    void BuscarCatalogo(Dictionary<int, string> catalogo, string singular)
    {
        Console.Clear();
        Console.WriteLine("===== BUSCAR " + singular.ToUpper() + " POR NOMBRE =====");
        string texto = LeerTexto("Nombre o parte del nombre: ");
        bool encontrado = false;
        foreach (KeyValuePair<int, string> par in catalogo)
        {
            if (par.Value.Contains(texto, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(par.Key + " - " + par.Value);
                encontrado = true;
            }
        }
        if (!encontrado)
        {
            Console.WriteLine("No se encontraron resultados.");
        }
        Pausa();
    }
    void ModificarCatalogo(Dictionary<int, string> catalogo, string singular)
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR " + singular.ToUpper() + " =====");
        if (catalogo.Count == 0)
        {
            Console.WriteLine("No hay registros.");
            Pausa();
            return;
        }

        foreach (KeyValuePair<int, string> par in catalogo)
        {
            Console.WriteLine(par.Key + " - " + par.Value);
        }
        int id = LeerEntero("\nId a modificar: ");
        if (!catalogo.ContainsKey(id))
        {
            Console.WriteLine("Ese Id no existe.");
            Pausa();
            return;
        }
        string nuevoNombre = LeerTexto("Nuevo nombre: ");
        foreach (KeyValuePair<int, string> otro in catalogo)
        {
            if (otro.Key != id &&
            otro.Value.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Ya existe otro registro con ese nombre.");
                Pausa();
                return;
            }
        }
        catalogo[id] = nuevoNombre;
        Console.WriteLine("\nRegistro modificado correctamente.");
        Pausa();
    }
    void EliminarCatalogo(Dictionary<int, string> catalogo, string singular, List<int> idsEnUso)
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR " + singular.ToUpper() + " =====");
        if (catalogo.Count == 0)
        {
            Console.WriteLine("No hay registros.");
            Pausa();
            return;
        }
        foreach (KeyValuePair<int, string> par in catalogo)
        {

            Console.WriteLine(par.Key + " - " + par.Value);
        }
        int id = LeerEntero("\nId a eliminar: ");
        if (!catalogo.ContainsKey(id))
        {
            Console.WriteLine("Ese Id no existe.");
            Pausa();
            return;
        }
        if (idsEnUso.Contains(id))
        {
            Console.WriteLine("No se puede eliminar porque hay prendas que la usan.");
            Pausa();
            return;
        }
        if (Confirmar("¿Confirma la eliminación?"))
        {
            catalogo.Remove(id);
            Console.WriteLine("Registro eliminado correctamente.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }
        Pausa();
    }
    void ListarCatalogo(Dictionary<int, string> catalogo, string singular)
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE " + singular.ToUpper() + "S =====");
        if (catalogo.Count == 0)
        {
            Console.WriteLine("No hay registros.");
        }
        else
        {
            foreach (KeyValuePair<int, string> par in catalogo)
            {
                Console.WriteLine(par.Key + " - " + par.Value);
            }

            Console.WriteLine("\nTotal: " + catalogo.Count);
        }
        Pausa();
    }
    void MenuCatalogo(Dictionary<int, string> catalogo, string singular, List<int> idsEnUso)
    {
        bool seguirEnMenu = true;
        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== MENÚ DE " + singular.ToUpper() + "S =====");
            Console.WriteLine("1. Agregar");
            Console.WriteLine("2. Buscar por nombre");
            Console.WriteLine("3. Modificar");
            Console.WriteLine("4. Eliminar");
            Console.WriteLine("5. Listar");
            Console.WriteLine("6. Volver al menú principal");
            int opcion = LeerEntero("\nSeleccione una opción: ");
            switch (opcion)
            {
                case 1: AgregarCatalogo(catalogo, singular); break;
                case 2: BuscarCatalogo(catalogo, singular); break;
                case 3: ModificarCatalogo(catalogo, singular); break;
                case 4: EliminarCatalogo(catalogo, singular, idsEnUso); break;
                case 5: ListarCatalogo(catalogo, singular); break;
                case 6: seguirEnMenu = false; break;
                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }
    
    void MostrarPrenda(int posicion)

    {
        Console.WriteLine("\nId: " + prendaIds[posicion]);
        Console.WriteLine("Nombre: " + prendaNombres[posicion]);
        Console.WriteLine("Categoría: " + categorias[prendaCategoriaIds[posicion]]);
        Console.WriteLine("Talla: " + tallas[prendaTallaIds[posicion]]);
        Console.WriteLine("Precio: $" + prendaPrecios[posicion].ToString("N2"));
        Console.WriteLine("Existencias: " + prendaExistencias[posicion]);
        Console.WriteLine("------------------------------");
    }
    void AgregarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== AGREGAR PRENDA =====");
        if (categorias.Count == 0 || tallas.Count == 0)
        {
            Console.WriteLine("Primero debe registrar al menos una categoría y una talla.");
            Pausa();
            return;
        }
        string nombre = LeerTexto("Nombre de la prenda: ");
        decimal precio;
        do
        {
            precio = LeerDecimal("Precio: ");
            if (precio <= 0)
            {
                Console.WriteLine("El precio debe ser mayor que cero.");
            }
        } while (precio <= 0);
        int cantidad = LeerEntero("Existencias: ");
        while (cantidad < 0)
        {
            Console.WriteLine("Las existencias no pueden ser negativas.");
            cantidad = LeerEntero("Existencias: ");
        }
        int categoriaId = ElegirDeDiccionario(categorias, "Categorías disponibles:");

        if (categoriaId == -1)
        {
            Pausa();
            return;
        }
        int tallaId = ElegirDeDiccionario(tallas, "Tallas disponibles:");
        if (tallaId == -1)
        {
            Pausa();
            return;
        }
        
        for (int i = 0; i < prendaIds.Count; i++)
        {
            if (prendaNombres[i].Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
            prendaCategoriaIds[i] == categoriaId &&
            prendaTallaIds[i] == tallaId)
            {
                Console.WriteLine("Ya existe esa prenda con la misma categoría y talla.");
                Pausa();
                return;
            }
        }
        prendaIds.Add(siguientePrendaId);
        prendaNombres.Add(nombre);
        prendaPrecios.Add(precio);
        prendaExistencias.Add(cantidad);
        prendaCategoriaIds.Add(categoriaId);
        prendaTallaIds.Add(tallaId);
        Console.WriteLine("\nPrenda agregada correctamente. Id: " + siguientePrendaId);
        siguientePrendaId++;
        Pausa();
    }
    void BuscarPrendas()
    {
        Console.Clear();
        Console.WriteLine("===== BUSCAR PRENDAS =====");
        Console.WriteLine("1. Por Id");
        Console.WriteLine("2. Por nombre");

        Console.WriteLine("3. Por categoría");
        Console.WriteLine("4. Por talla");
        Console.WriteLine("5. Por rango de precio");
        int criterio = LeerEntero("\nSeleccione el criterio: ");
        bool encontrada = false;
        switch (criterio)
        {
            case 1:
                {
                    int id = LeerEntero("Id de la prenda: ");
                    int posicion = prendaIds.IndexOf(id);
                    if (posicion != -1)
                    {
                        MostrarPrenda(posicion);
                        encontrada = true;
                    }
                    break;
                }
            case 2:
                {
                    string texto = LeerTexto("Nombre o parte del nombre: ");
                    for (int i = 0; i < prendaIds.Count; i++)
                    {
                        if (prendaNombres[i].Contains(texto, StringComparison.OrdinalIgnoreCase))
                        {
                            MostrarPrenda(i);
                            encontrada = true;
                        }
                    }
                    break;
                }
            case 3:
                {
                    int categoriaId = ElegirDeDiccionario(categorias, "Categorías disponibles:");
                    if (categoriaId == -1)
                    {
                        Pausa();
                        return;

                    }
                    for (int i = 0; i < prendaIds.Count; i++)
                    {
                        if (prendaCategoriaIds[i] == categoriaId)
                        {
                            MostrarPrenda(i);
                            encontrada = true;
                        }
                    }
                    break;
                }
            case 4:
                {
                    int tallaId = ElegirDeDiccionario(tallas, "Tallas disponibles:");
                    if (tallaId == -1)
                    {
                        Pausa();
                        return;
                    }
                    for (int i = 0; i < prendaIds.Count; i++)
                    {
                        if (prendaTallaIds[i] == tallaId)
                        {
                            MostrarPrenda(i);
                            encontrada = true;
                        }
                    }
                    break;
                }
            case 5:
                {
                    decimal minimo = LeerDecimal("Precio mínimo: ");
                    decimal maximo = LeerDecimal("Precio máximo: ");
                    if (maximo < minimo)
                    {
                        Console.WriteLine("El precio máximo no puede ser menor que el mínimo.");
                        Pausa();
                        return;
                    }

                    for (int i = 0; i < prendaIds.Count; i++)
                    {
                        if (prendaPrecios[i] >= minimo && prendaPrecios[i] <= maximo)
                        {
                            MostrarPrenda(i);
                            encontrada = true;
                        }
                    }
                    break;
                }
            default:
                Console.WriteLine("Criterio inválido.");
                Pausa();
                return;
        }
        if (!encontrada)
        {
            Console.WriteLine("No se encontraron prendas.");
        }
        Pausa();
    }
    void ModificarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== MODIFICAR PRENDA =====");
        int id = LeerEntero("Id de la prenda: ");
        int posicion = prendaIds.IndexOf(id);
        if (posicion == -1)
        {
            Console.WriteLine("Prenda no encontrada.");
            Pausa();
            return;
        }
        MostrarPrenda(posicion);
        Console.WriteLine("\n¿Qué desea modificar?");
        Console.WriteLine("1. Nombre");
        Console.WriteLine("2. Precio");

        Console.WriteLine("3. Existencias");
        Console.WriteLine("4. Categoría");
        Console.WriteLine("5. Talla");
        int campo = LeerEntero("\nSeleccione una opción: ");
        switch (campo)
        {
            case 1:
                {
                    prendaNombres[posicion] = LeerTexto("Nuevo nombre: ");
                    break;
                }
            case 2:
                {
                    decimal nuevoPrecio;
                    do
                    {
                        nuevoPrecio = LeerDecimal("Nuevo precio: ");
                        if (nuevoPrecio <= 0)
                        {
                            Console.WriteLine("El precio debe ser mayor que cero.");
                        }
                    } while (nuevoPrecio <= 0);
                    prendaPrecios[posicion] = nuevoPrecio;
                    break;
                }
            case 3:
                {
                    int nuevasExistencias = LeerEntero("Nuevas existencias: ");
                    while (nuevasExistencias < 0)
                    {
                        Console.WriteLine("Las existencias no pueden ser negativas.");
                        nuevasExistencias = LeerEntero("Nuevas existencias: ");
                    }
                    prendaExistencias[posicion] = nuevasExistencias;
                    break;
                }

            case 4:
                {
                    int nuevaCategoria = ElegirDeDiccionario(categorias, "Categorías disponibles:");
                    if (nuevaCategoria == -1)
                    {
                        Pausa();
                        return;
                    }
                    prendaCategoriaIds[posicion] = nuevaCategoria;
                    break;
                }
            case 5:
                {
                    int nuevaTalla = ElegirDeDiccionario(tallas, "Tallas disponibles:");
                    if (nuevaTalla == -1)
                    {
                        Pausa();
                        return;
                    }
                    prendaTallaIds[posicion] = nuevaTalla;
                    break;
                }
            default:
                Console.WriteLine("Opción inválida.");
                Pausa();
                return;
        }
        Console.WriteLine("\nPrenda modificada correctamente.");
        Pausa();
    }
    void EliminarPrenda()
    {
        Console.Clear();
        Console.WriteLine("===== ELIMINAR PRENDA =====");
        int id = LeerEntero("Id de la prenda: ");
        int posicion = prendaIds.IndexOf(id);
        if (posicion == -1)

        {
            Console.WriteLine("Prenda no encontrada.");
            Pausa();
            return;
        }
        MostrarPrenda(posicion);
        if (Confirmar("¿Confirma la eliminación?"))
        {
            prendaIds.RemoveAt(posicion);
            prendaNombres.RemoveAt(posicion);
            prendaPrecios.RemoveAt(posicion);
            prendaExistencias.RemoveAt(posicion);
            prendaCategoriaIds.RemoveAt(posicion);
            prendaTallaIds.RemoveAt(posicion);
            Console.WriteLine("Prenda eliminada correctamente.");
        }
        else
        {
            Console.WriteLine("Eliminación cancelada.");
        }
        Pausa();
    }
    void ListarPrendas()
    {
        Console.Clear();
        Console.WriteLine("===== LISTA DE PRENDAS =====");
        if (prendaIds.Count == 0)
        {
            Console.WriteLine("No hay prendas registradas.");
        }
        else
        {
            for (int i = 0; i < prendaIds.Count; i++)
            {
                MostrarPrenda(i);
            }
            Console.WriteLine("Total de prendas: " + prendaIds.Count);
        }
        Pausa();

    }
    void MenuPrendas()
    {
        bool seguirEnMenu = true;
        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== MENÚ DE PRENDAS =====");
            Console.WriteLine("1. Agregar prenda");
            Console.WriteLine("2. Buscar prendas");
            Console.WriteLine("3. Modificar prenda");
            Console.WriteLine("4. Eliminar prenda");
            Console.WriteLine("5. Listar prendas");
            Console.WriteLine("6. Volver al menú principal");
            int opcion = LeerEntero("\nSeleccione una opción: ");
            switch (opcion)
            {
                case 1: AgregarPrenda(); break;
                case 2: BuscarPrendas(); break;
                case 3: ModificarPrenda(); break;
                case 4: EliminarPrenda(); break;
                case 5: ListarPrendas(); break;
                case 6: seguirEnMenu = false; break;
                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }
   
    void RegistrarEntrada()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR ENTRADA DE MERCANCÍA =====");
        int id = LeerEntero("Id de la prenda: ");
        int posicion = prendaIds.IndexOf(id);

        if (posicion == -1)
        {
            Console.WriteLine("Prenda no encontrada.");
            Pausa();
            return;
        }
        int cantidad = LeerEntero("Cantidad que ingresa: ");
        if (cantidad <= 0)
        {
            Console.WriteLine("La cantidad debe ser mayor que cero.");
            Pausa();
            return;
        }
        prendaExistencias[posicion] += cantidad;
        Console.WriteLine("\nEntrada registrada. Existencias actuales: " + prendaExistencias[posicion]);
        Pausa();
    }
    void RegistrarVenta()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR VENTA =====");
        int id = LeerEntero("Id de la prenda: ");
        int posicion = prendaIds.IndexOf(id);
        if (posicion == -1)
        {
            Console.WriteLine("Prenda no encontrada.");
            Pausa();
            return;
        }
        Console.WriteLine("Existencias disponibles: " + prendaExistencias[posicion]);
        int cantidad = LeerEntero("Cantidad a vender: ");
        if (cantidad <= 0)
        {
            Console.WriteLine("La cantidad debe ser mayor que cero.");
            Pausa();
            return;

        }
        if (cantidad > prendaExistencias[posicion])
        {
            Console.WriteLine("No hay suficientes existencias.");
            Pausa();
            return;
        }
        prendaExistencias[posicion] -= cantidad;
        decimal total = prendaPrecios[posicion] * cantidad;
        Console.WriteLine("\nVenta registrada. Total: $" + total.ToString("N2"));
        Console.WriteLine("Existencias restantes: " + prendaExistencias[posicion]);
        Pausa();
    }
    void MostrarStockBajo()
    {
        Console.Clear();
        Console.WriteLine("===== PRENDAS CON STOCK BAJO =====");
        int limite = LeerEntero("Mostrar prendas con existencias menores o iguales a: ");
        bool encontrada = false;
        for (int i = 0; i < prendaIds.Count; i++)
        {
            if (prendaExistencias[i] <= limite)
            {
                MostrarPrenda(i);
                encontrada = true;
            }
        }
        if (!encontrada)
        {
            Console.WriteLine("No hay prendas con ese nivel de existencias.");
        }
        Pausa();
    }
    void MostrarValorInventario()
    {
        Console.Clear();
        Console.WriteLine("===== VALOR DEL INVENTARIO =====");

        decimal valorTotal = 0;
        int unidadesTotales = 0;
        for (int i = 0; i < prendaIds.Count; i++)
        {
            valorTotal += prendaPrecios[i] * prendaExistencias[i];
            unidadesTotales += prendaExistencias[i];
        }
        Console.WriteLine("Prendas distintas: " + prendaIds.Count);
        Console.WriteLine("Unidades totales: " + unidadesTotales);
        Console.WriteLine("Valor total: $" + valorTotal.ToString("N2"));
        Pausa();
    }
    void MostrarExistenciasPorCategoria()
    {
        Console.Clear();
        Console.WriteLine("===== EXISTENCIAS POR CATEGORÍA =====");
        if (categorias.Count == 0)
        {
            Console.WriteLine("No hay categorías registradas.");
        }
        else
        {
            foreach (KeyValuePair<int, string> par in categorias)
            {
                int total = 0;
                for (int i = 0; i < prendaIds.Count; i++)
                {
                    if (prendaCategoriaIds[i] == par.Key)
                    {
                        total += prendaExistencias[i];
                    }
                }
                Console.WriteLine(par.Value + ": " + total + " unidades");
            }
        }
        Pausa();
    }
    void MenuExistencias()

    {
        bool seguirEnMenu = true;
        while (seguirEnMenu)
        {
            Console.Clear();
            Console.WriteLine("===== EXISTENCIAS Y REPORTES =====");
            Console.WriteLine("1. Registrar entrada de mercancía");
            Console.WriteLine("2. Registrar venta");
            Console.WriteLine("3. Prendas con stock bajo");
            Console.WriteLine("4. Valor del inventario");
            Console.WriteLine("5. Existencias por categoría");
            Console.WriteLine("6. Volver al menú principal");
            int opcion = LeerEntero("\nSeleccione una opción: ");
            switch (opcion)
            {
                case 1: RegistrarEntrada(); break;
                case 2: RegistrarVenta(); break;
                case 3: MostrarStockBajo(); break;
                case 4: MostrarValorInventario(); break;
                case 5: MostrarExistenciasPorCategoria(); break;
                case 6: seguirEnMenu = false; break;
                default:
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                    break;
            }
        }
    }
  
    bool ejecutando = true;
    while (ejecutando)
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine(" REGISTRO DE ROPA - BOUTIQUE");
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
            case 1:
                MenuCatalogo(categorias, "categoría", prendaCategoriaIds);
                break;
            case 2:
                MenuCatalogo(tallas, "talla", prendaTallaIds);
                break;
            case 3:
                MenuPrendas();
                break;
            case 4:
                MenuExistencias();
                break;
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