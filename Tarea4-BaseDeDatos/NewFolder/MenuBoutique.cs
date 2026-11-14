public class MenuBoutique
{
    private Inventario inventario = new Inventario();

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

        int campo = LeerEntero("\nSeleccione una opción pls: ");

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

    public void Iniciar()
    {
        bool ejecutando = true;

        while (ejecutando)
        {
            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine(" REGISTRO DE ROPA - BOUTIQUE");
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
                        Console.WriteLine("\nGracias por usar el sistema. ¡Chau!");
                    }
                    break;

                default:
                    Console.WriteLine("Opción inválida, pls intentelo de nuevo.");
                    Pausa();
                    break;
            }
        }
    }
}


