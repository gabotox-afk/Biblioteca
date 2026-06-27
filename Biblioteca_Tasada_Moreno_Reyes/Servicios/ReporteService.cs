using System;
using System.Collections.Generic;
using System.Linq;
using Biblioteca_Tasada_Moreno_Reyes.DBcontext;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_Tasada_Moreno_Reyes.Servicios
{
    public class ReporteService
    {
        private BibliotecaContext _context;

        public ReporteService(BibliotecaContext context)
        {
            _context = context;
        }


        // 1) Los 5 libros con mas prestamos historicos
        public void LibrosMasPrestados()
        {
            Console.WriteLine("\n=== LIBROS MAS PRESTADOS ===");

            var ranking = _context.Prestamos
                .Include(p => p.Libro)
                .GroupBy(p => p.Libro.Titulo)
                .Select(g => new { Titulo = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            if (ranking.Count == 0)
            {
                Console.WriteLine("Todavia no hay prestamos registrados.");
                return;
            }

            int puesto = 1;
            foreach (var item in ranking)
            {
                Console.WriteLine($"  {puesto}. {item.Titulo} - {item.Cantidad} prestamo(s)");
                puesto++;
            }
        }


        // 2) Socios con multas pendientes y el monto total
        public void SociosConMultas()
        {
            Console.WriteLine("\n=== SOCIOS CON MULTAS PENDIENTES ===");

            List<Socio> socios = _context.Socios.ToList();
            List<Prestamo> prestamos = _context.Prestamos.ToList();

            bool hayAlguno = false;

            foreach (Socio socio in socios)
            {
                decimal totalAdeudado = 0;

                foreach (Prestamo p in prestamos)
                {
                    if (p.SocioId == socio.NroSocio && p.MultaMonto > 0 && p.MultaPagada == 0)
                    {
                        totalAdeudado += p.MultaMonto;
                    }
                }

                if (totalAdeudado > 0)
                {
                    Console.WriteLine($"  Socio N° {socio.NroSocio} - {socio.NombreApellido} | Debe: ${totalAdeudado}");
                    hayAlguno = true;
                }
            }

            if (!hayAlguno)
            {
                Console.WriteLine("No hay socios con multas pendientes.");
            }
        }


        // 3) Prestamos vencidos (no devueltos y con fecha de vencimiento pasada)
        public void PrestamosVencidos()
        {
            Console.WriteLine("\n=== PRESTAMOS VENCIDOS ===");

            List<Prestamo> vencidos = _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Socio)
                .Where(p => p.FechaDevolucion == null && p.FechaVencimiento < DateTime.Now)
                .ToList();

            if (vencidos.Count == 0)
            {
                Console.WriteLine("No hay prestamos vencidos.");
                return;
            }

            foreach (Prestamo p in vencidos)
            {
                int diasAtraso = (DateTime.Now - p.FechaVencimiento).Days;
                Console.WriteLine($"  {p.Socio.NombreApellido} | {p.Libro.Titulo} | Vencio: {p.FechaVencimiento:dd/MM/yyyy} ({diasAtraso} dia(s) de atraso)");
            }
        }


        // 4) Disponibilidad de un libro dado un ISBN o titulo
        public void DisponibilidadLibro()
        {
            Console.WriteLine("\n=== DISPONIBILIDAD DE UN LIBRO ===");

            Console.Write("Ingrese ISBN o titulo: ");
            string busqueda = Console.ReadLine().ToLower();

            Libro libro = null;
            List<Libro> todosLibros = _context.Libros.ToList();

            foreach (Libro l in todosLibros)
            {
                if (l.ISBN.ToLower() == busqueda || l.Titulo.ToLower().Contains(busqueda))
                {
                    libro = l;
                    break;
                }
            }

            if (libro == null)
            {
                Console.WriteLine("No se encontro el libro.");
                return;
            }

            EstadoPrestamo estadoActivo = _context.estadoPrestamo.FirstOrDefault(e => e.Nombre == "Activo");
            EstadoReserva estadoPendiente = _context.estadoReserva.FirstOrDefault(e => e.Nombre == "Pendiente");

            int prestadosAhora = _context.Prestamos
                .Count(p => p.LibroISBN == libro.ISBN && p.EstadoId == estadoActivo.Id);

            int reservasPendientes = _context.Reservas
                .Count(r => r.LibroISBN == libro.ISBN && r.EstadoId == estadoPendiente.Id);

            int disponibles = libro.CantidadCopias - prestadosAhora;

            Console.WriteLine($"\n  Libro: {libro.Titulo} ({libro.ISBN})");
            Console.WriteLine($"  Copias totales:     {libro.CantidadCopias}");
            Console.WriteLine($"  Copias disponibles: {disponibles}");
            Console.WriteLine($"  Reservas pendientes: {reservasPendientes}");
        }


        // 5) Historial de un socio: prestamos y reservas
        public void HistorialSocio()
        {
            Console.WriteLine("\n=== HISTORIAL DE UN SOCIO ===");

            Console.Write("Ingrese numero de socio: ");
            string entrada = Console.ReadLine();

            if (!int.TryParse(entrada, out int nroSocio))
            {
                Console.WriteLine("Numero invalido.");
                return;
            }

            Socio socio = _context.Socios.FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("No existe un socio con ese numero.");
                return;
            }

            Console.WriteLine($"\nSocio: {socio.NombreApellido} (N° {socio.NroSocio})");

            List<Prestamo> prestamos = _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.SocioId == nroSocio)
                .ToList();

            Console.WriteLine("\n  --- Prestamos ---");
            if (prestamos.Count == 0)
            {
                Console.WriteLine("  (sin prestamos)");
            }
            else
            {
                foreach (Prestamo p in prestamos)
                {
                    string devolucion = p.FechaDevolucion == null ? "sin devolver" : ((DateTime)p.FechaDevolucion).ToString("dd/MM/yyyy");
                    Console.WriteLine($"  {p.Libro.Titulo} | Estado: {p.EstadoPrestamo.Nombre} | Prestado: {p.FechaPrestamo:dd/MM/yyyy} | Devuelto: {devolucion}");
                }
            }

            List<Reserva> reservas = _context.Reservas
                .Include(r => r.Libro)
                .Include(r => r.EstadoReserva)
                .Where(r => r.SocioId == nroSocio)
                .ToList();

            Console.WriteLine("\n  --- Reservas ---");
            if (reservas.Count == 0)
            {
                Console.WriteLine("  (sin reservas)");
            }
            else
            {
                foreach (Reserva r in reservas)
                {
                    Console.WriteLine($"  {r.Libro.Titulo} | Estado: {r.EstadoReserva.Nombre} | Reservado: {r.FechaReserva:dd/MM/yyyy}");
                }
            }
        }


        // 6) Ranking de los 10 socios mas activos por cantidad de prestamos historicos
        public void RankingSocios()
        {
            Console.WriteLine("\n=== RANKING DE SOCIOS MAS ACTIVOS ===");

            List<Socio> socios = _context.Socios
                .Include(s => s.TipoSocio)
                .ToList();
            List<Prestamo> prestamos = _context.Prestamos.ToList();

            var ranking = new List<(string Nombre, string Tipo, int Cantidad, bool TieneMulta)>();

            foreach (Socio socio in socios)
            {
                int cantidad = 0;
                bool tieneMulta = false;

                foreach (Prestamo p in prestamos)
                {
                    if (p.SocioId == socio.NroSocio)
                    {
                        cantidad++;
                        if (p.MultaMonto > 0 && p.MultaPagada == 0)
                        {
                            tieneMulta = true;
                        }
                    }
                }

                ranking.Add((socio.NombreApellido, socio.TipoSocio.Nombre, cantidad, tieneMulta));
            }

            var top10 = ranking
                .OrderByDescending(x => x.Cantidad)
                .Take(10)
                .ToList();

            int puesto = 1;
            foreach (var item in top10)
            {
                string multa = item.TieneMulta ? "SI" : "NO";
                Console.WriteLine($"  {puesto}. {item.Nombre} ({item.Tipo}) | Prestamos: {item.Cantidad} | Multa pendiente: {multa}");
                puesto++;
            }
        }
    }
}
