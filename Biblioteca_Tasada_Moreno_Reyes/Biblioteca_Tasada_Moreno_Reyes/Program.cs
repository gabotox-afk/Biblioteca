using System;
using System.Linq;
using Biblioteca_Tasada_Moreno_Reyes.DBcontext;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;

namespace Biblioteca_Tasada_Moreno_Reyes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   SISTEMA DE GESTIÓN DE BIBLIOTECA");
            Console.WriteLine("========================================\n");

            // Instanciamos el DbContext que acabás de terminar
            using (var context = new BibliotecaContext())
            {
                try
                {
                    Console.WriteLine("Conectando a la base de datos de SQLite...");

                    // Intentamos traer la lista de libros
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
                catch (Exception ex)
                {
                    // Si el puente de Entity Framework se llega a quejar por algo, salta acá
                    Console.WriteLine("\n[X] ERROR AL CONECTAR O LEER LA BASE DE DATOS:");
                    Console.WriteLine(ex.Message);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Detalle técnico: {ex.InnerException.Message}");
                    }
                }
            }

            Console.WriteLine("\n========================================");
            Console.WriteLine("Presione cualquier tecla para cerrar...");
            Console.ReadKey();
        }
    }
}