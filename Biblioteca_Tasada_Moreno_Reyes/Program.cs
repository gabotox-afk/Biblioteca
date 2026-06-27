using System;
using System.Linq;
using Biblioteca_Tasada_Moreno_Reyes.DBcontext;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;
using Biblioteca_Tasada_Moreno_Reyes.Servicios;

namespace Biblioteca_Tasada_Moreno_Reyes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   SISTEMA DE GESTIÓN DE BIBLIOTECA");
            Console.WriteLine("========================================\n");

            using (var context = new BibliotecaContext())
            {
                try
                {
                    Console.WriteLine("Conectando a la base de datos de SQLite...");

                    // Al iniciar mostramos la lista de libros disponibles
                    MostrarLibros(context);

                    var prestamoService = new PrestamoService(context);
                    var reservaService = new ReservaService(context);
                    var reporteService = new ReporteService(context);

                    bool salir = false;

                    while (!salir)
                    {
                        Console.WriteLine("\n========== MENU PRINCIPAL ==========");
                        Console.WriteLine("1. Ver libros");
                        Console.WriteLine("2. Realizar prestamo");
                        Console.WriteLine("3. Registrar devolucion");
                        Console.WriteLine("4. Renovar prestamo");
                        Console.WriteLine("5. Realizar reserva");
                        Console.WriteLine("6. Reportes");
                        Console.WriteLine("0. Salir");
                        Console.Write("Elija una opcion: ");

                        string opcion = Console.ReadLine();

                        switch (opcion)
                        {
                            case "1":
                                MostrarLibros(context);
                                break;
                            case "2":
                                prestamoService.RealizarPrestamo();
                                break;
                            case "3":
                                prestamoService.RegistrarDevolucion();
                                break;
                            case "4":
                                prestamoService.RenovarPrestamo();
                                break;
                            case "5":
                                reservaService.RealizarReserva();
                                break;
                            case "6":
                                MenuReportes(reporteService);
                                break;
                            case "0":
                                salir = true;
                                break;
                            default:
                                Console.WriteLine("Opcion invalida.");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n[X] ERROR AL CONECTAR O LEER LA BASE DE DATOS:");
                    Console.WriteLine(ex.Message);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Detalle técnico: {ex.InnerException.Message}");
                    }
                }
            }

            Console.WriteLine("\n========================================");
            Console.WriteLine("Gracias por usar el sistema. Hasta luego!");
        }


        static void MostrarLibros(BibliotecaContext context)
        {
            var libros = context.Libros.ToList();

            Console.WriteLine("\n--- Libros Disponibles ---");

            if (libros.Count == 0)
            {
                Console.WriteLine("La conexión fue un éxito, pero la tabla está vacía.");
            }
            else
            {
                foreach (var libro in libros)
                {
                    Console.WriteLine($"> ISBN: {libro.ISBN} | {libro.Titulo} de {libro.Autor} ({libro.CantidadCopias} copias)");
                }
            }
        }


        static void MenuReportes(ReporteService reporteService)
        {
            bool volver = false;

            while (!volver)
            {
                Console.WriteLine("\n========== REPORTES ==========");
                Console.WriteLine("1. Libros mas prestados");
                Console.WriteLine("2. Socios con multas pendientes");
                Console.WriteLine("3. Prestamos vencidos");
                Console.WriteLine("4. Disponibilidad de un libro");
                Console.WriteLine("5. Historial de un socio");
                Console.WriteLine("6. Ranking de socios");
                Console.WriteLine("0. Volver");
                Console.Write("Elija una opcion: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        reporteService.LibrosMasPrestados();
                        break;
                    case "2":
                        reporteService.SociosConMultas();
                        break;
                    case "3":
                        reporteService.PrestamosVencidos();
                        break;
                    case "4":
                        reporteService.DisponibilidadLibro();
                        break;
                    case "5":
                        reporteService.HistorialSocio();
                        break;
                    case "6":
                        reporteService.RankingSocios();
                        break;
                    case "0":
                        volver = true;
                        break;
                    default:
                        Console.WriteLine("Opcion invalida.");
                        break;
                }
            }
        }
    }
}
