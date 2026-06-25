using System;
using System.Collections.Generic;
using Biblioteca_Tasada_Moreno_Reyes.DBcontext;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_Tasada_Moreno_Reyes.Servicios
{
    public class PrestamoService
    {
        private BibliotecaContext _context;

        public PrestamoService(BibliotecaContext context)
        {
            _context = context;
        }


        public void RealizarPrestamo()
        {
            Console.WriteLine("\n=== REALIZAR PRESTAMO ===");


            Console.Write("Ingrese numero de socio: ");
            string entradaSocio = Console.ReadLine();

            if (!int.TryParse(entradaSocio, out int nroSocio))
            {
                Console.WriteLine("Numero invalido.");
                return;
            }


            Socio socio = _context.Socios
                .Include(s => s.TipoSocio)
                .FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("No existe un socio con ese numero.");
                return;
            }


            if (socio.Activo != 1)
            {
                Console.WriteLine("El socio esta inactivo y no puede sacar libros.");
                return;
            }

            List<Prestamo> todosPrestamos = _context.Prestamos.ToList();
            bool tieneMultaPendiente = false;

            foreach (Prestamo prestamo in todosPrestamos)
            {
                if (prestamo.SocioId == nroSocio && prestamo.MultaMonto > 0 && prestamo.MultaPagada == 0)
                {
                    tieneMultaPendiente = true;
                    break;
                }
            }

            if (tieneMultaPendiente)
            {
                Console.WriteLine("El socio tiene multas sin pagar. Debe abonarlas antes de sacar libros.");
                return;
            }


            EstadoPrestamo estadoActivo = BuscarEstadoPrestamo("Activo");
            int cantPrestamosActivos = 0;

            foreach (Prestamo prestamo in todosPrestamos)
            {
                if (prestamo.SocioId == nroSocio && prestamo.EstadoId == estadoActivo.Id)
                {
                    cantPrestamosActivos++;
                }
            }

            if (cantPrestamosActivos >= socio.TipoSocio.Maxlibros)
            {
                Console.WriteLine("El socio ya tiene el maximo de libros prestados permitidos para su tipo.");
                return;
            }


            Console.Write("Ingrese titulo o autor del libro: ");
            string busqueda = Console.ReadLine().ToLower();

            List<Libro> librosEncontrados = new List<Libro>();
            List<Libro> todosLibros = _context.Libros.ToList();

            foreach (Libro libro in todosLibros)
            {
                if (libro.Titulo.ToLower().Contains(busqueda) || libro.Autor.ToLower().Contains(busqueda))
                {
                    librosEncontrados.Add(libro);
                }
            }

            if (librosEncontrados.Count == 0)
            {
                Console.WriteLine("No se encontraron libros con ese criterio.");
                return;
            }


            Console.WriteLine("\nLibros encontrados:");
            for (int i = 0; i < librosEncontrados.Count; i++)
            {
                int disponibles = CantidadDisponible(librosEncontrados[i].ISBN, estadoActivo.Id, todosPrestamos);
                Console.WriteLine($"  [{i + 1}] {librosEncontrados[i].Titulo} - {librosEncontrados[i].Autor} | Disponibles: {disponibles}");
            }

            Console.Write("Elija un libro (numero): ");
            string entradaLibro = Console.ReadLine();

            if (!int.TryParse(entradaLibro, out int eleccion) || eleccion < 1 || eleccion > librosEncontrados.Count)
            {
                Console.WriteLine("Eleccion invalida.");
                return;
            }

            Libro libroElegido = librosEncontrados[eleccion - 1];
            int copiasDisponibles = CantidadDisponible(libroElegido.ISBN, estadoActivo.Id, todosPrestamos);


            if (copiasDisponibles <= 0)
            {
                Console.WriteLine($"No hay copias disponibles de '{libroElegido.Titulo}'.");
                Console.Write("Desea hacer una reserva? (S/N): ");
                string respuesta = Console.ReadLine().ToUpper();

                if (respuesta == "S")
                {
                    HacerReserva(socio, libroElegido);
                }
                return;
            }


            int diasPrestamo = int.Parse(socio.TipoSocio.DiasPrestamo);

            Prestamo nuevoPrestamo = new Prestamo();
            nuevoPrestamo.SocioId = socio.NroSocio;
            nuevoPrestamo.LibroISBN = libroElegido.ISBN;
            nuevoPrestamo.FechaPrestamo = DateTime.Now;
            nuevoPrestamo.FechaVencimiento = DateTime.Now.AddDays(diasPrestamo);
            nuevoPrestamo.EstadoId = estadoActivo.Id;
            nuevoPrestamo.Renovado = 0;
            nuevoPrestamo.MultaPagada = 0;

            _context.Prestamos.Add(nuevoPrestamo);
            _context.SaveChanges();

            Console.WriteLine("\nPrestamo registrado!");
            Console.WriteLine("  Libro:             " + libroElegido.Titulo);
            Console.WriteLine("  Fecha de prestamo: " + nuevoPrestamo.FechaPrestamo.ToString("dd/MM/yyyy"));
            Console.WriteLine("  Devolver antes de: " + nuevoPrestamo.FechaVencimiento.ToString("dd/MM/yyyy"));
        }


        public void RegistrarDevolucion()
        {
            Console.WriteLine("\n=== REGISTRAR DEVOLUCION ===");

            Console.Write("Ingrese numero de socio: ");
            string entrada = Console.ReadLine();

            if (!int.TryParse(entrada, out int nroSocio))
            {
                Console.WriteLine("Numero invalido.");
                return;
            }


            EstadoPrestamo estadoActivo = BuscarEstadoPrestamo("Activo");

            List<Prestamo> prestamosActivos = new List<Prestamo>();
            List<Prestamo> todosPrestamos = _context.Prestamos.Include(p => p.Libro).ToList();

            foreach (Prestamo p in todosPrestamos)
            {
                if (p.SocioId == nroSocio && p.EstadoId == estadoActivo.Id)
                {
                    prestamosActivos.Add(p);
                }
            }

            if (prestamosActivos.Count == 0)
            {
                Console.WriteLine("El socio no tiene prestamos activos.");
                return;
            }


            Console.WriteLine("\nPrestamos activos:");
            for (int i = 0; i < prestamosActivos.Count; i++)
            {
                Prestamo p = prestamosActivos[i];
                string vencido = p.FechaVencimiento < DateTime.Now ? " [VENCIDO]" : "";
                Console.WriteLine($"  [{i + 1}] {p.Libro.Titulo} | Vence: {p.FechaVencimiento:dd/MM/yyyy}{vencido}");
            }

            Console.Write("Cual devuelve? (numero): ");
            string entradaNum = Console.ReadLine();

            if (!int.TryParse(entradaNum, out int eleccion) || eleccion < 1 || eleccion > prestamosActivos.Count)
            {
                Console.WriteLine("Eleccion invalida.");
                return;
            }

            Prestamo prestamoADevolver = prestamosActivos[eleccion - 1];
            DateTime fechaDevolucion = DateTime.Now;


            if (fechaDevolucion > prestamoADevolver.FechaVencimiento)
            {
                Socio socio = _context.Socios.Include(s => s.TipoSocio).FirstOrDefault(s => s.NroSocio == nroSocio);

                int diasDemora = (fechaDevolucion - prestamoADevolver.FechaVencimiento).Days;
                decimal multa = diasDemora * (decimal)socio.TipoSocio.MultaPorDia;

                prestamoADevolver.MultaMonto = multa;
                prestamoADevolver.MultaPagada = 0;

                Console.WriteLine($"Devolucion con {diasDemora} dia(s) de demora. Multa: ${multa}");
            }


            EstadoPrestamo estadoDevuelto = BuscarEstadoPrestamo("Devuelto");
            prestamoADevolver.FechaDevolucion = fechaDevolucion;
            prestamoADevolver.EstadoId = estadoDevuelto.Id;

            _context.SaveChanges();

            EstadoReserva estadoReservaPendiente = BuscarEstadoReserva("Pendiente");
            EstadoReserva estadoReservaCumplida = BuscarEstadoReserva("Cumplida");

            Reserva reservaPendiente = null;
            List<Reserva> todasReservas = _context.Reservas.Include(r => r.Socio).ToList();

            foreach (Reserva r in todasReservas)
            {
                if (r.LibroISBN == prestamoADevolver.LibroISBN && r.EstadoId == estadoReservaPendiente.Id)
                {
                    if (reservaPendiente == null || r.FechaReserva < reservaPendiente.FechaReserva)
                    {
                        reservaPendiente = r;
                    }
                }
            }

            if (reservaPendiente != null)
            {
                reservaPendiente.EstadoId = estadoReservaCumplida.Id;
                _context.SaveChanges();
                Console.WriteLine($"\n[AVISO] El libro '{prestamoADevolver.Libro.Titulo}' estaba reservado.");
                Console.WriteLine($"  Notificar a: {reservaPendiente.Socio.NombreApellido} (Socio N° {reservaPendiente.SocioId})");
            }

            Console.WriteLine("\nDevolucion registrada el " + fechaDevolucion.ToString("dd/MM/yyyy") + ".");
        }


        public void RenovarPrestamo()
        {
            Console.WriteLine("\n=== RENOVAR PRESTAMO ===");

            Console.Write("Ingrese numero de socio: ");
            string entrada = Console.ReadLine();

            if (!int.TryParse(entrada, out int nroSocio))
            {
                Console.WriteLine("Numero invalido.");
                return;
            }


            EstadoPrestamo estadoActivo = BuscarEstadoPrestamo("Activo");

            List<Prestamo> prestamosActivos = new List<Prestamo>();
            List<Prestamo> todosPrestamos = _context.Prestamos.Include(p => p.Libro).ToList();

            foreach (Prestamo p in todosPrestamos)
            {
                if (p.SocioId == nroSocio && p.EstadoId == estadoActivo.Id)
                {
                    prestamosActivos.Add(p);
                }
            }

            if (prestamosActivos.Count == 0)
            {
                Console.WriteLine("El socio no tiene prestamos activos.");
                return;
            }


            Console.WriteLine("\nPrestamos activos:");
            for (int i = 0; i < prestamosActivos.Count; i++)
            {
                Prestamo p = prestamosActivos[i];
                string renovado = p.Renovado == 1 ? " [ya renovado]" : "";
                Console.WriteLine($"  [{i + 1}] {p.Libro.Titulo} | Vence: {p.FechaVencimiento:dd/MM/yyyy}{renovado}");
            }

            Console.Write("Cual quiere renovar? (numero): ");
            string entradaNum = Console.ReadLine();

            if (!int.TryParse(entradaNum, out int eleccion) || eleccion < 1 || eleccion > prestamosActivos.Count)
            {
                Console.WriteLine("Eleccion invalida.");
                return;
            }

            Prestamo prestamoARenovar = prestamosActivos[eleccion - 1];


            if (prestamoARenovar.Renovado == 1)
            {
                Console.WriteLine("Este prestamo ya fue renovado anteriormente. No se puede renovar de nuevo.");
                return;
            }

 
            if (prestamoARenovar.FechaVencimiento < DateTime.Now)
            {
                Console.WriteLine("No se puede renovar un prestamo vencido.");
                return;
            }


            EstadoReserva estadoReservaPendiente = BuscarEstadoReserva("Pendiente");
            List<Reserva> todasReservas = _context.Reservas.ToList();
            bool hayReservaDeOtro = false;

            foreach (Reserva r in todasReservas)
            {
                if (r.LibroISBN == prestamoARenovar.LibroISBN && r.EstadoId == estadoReservaPendiente.Id && r.SocioId != nroSocio)
                {
                    hayReservaDeOtro = true;
                    break;
                }
            }

            if (hayReservaDeOtro)
            {
                Console.WriteLine("No se puede renovar: otro socio tiene este libro reservado.");
                return;
            }


            int diasOriginales = (prestamoARenovar.FechaVencimiento - prestamoARenovar.FechaPrestamo).Days;
            prestamoARenovar.FechaVencimiento = prestamoARenovar.FechaVencimiento.AddDays(diasOriginales);
            prestamoARenovar.Renovado = 1;

            _context.SaveChanges();

            Console.WriteLine("\nPrestamo renovado!");
            Console.WriteLine("  Nueva fecha de vencimiento: " + prestamoARenovar.FechaVencimiento.ToString("dd/MM/yyyy"));
        }




        private int CantidadDisponible(string isbn, int estadoActivoId, List<Prestamo> todosPrestamos)
        {
            Libro libro = _context.Libros.Find(isbn);
            if (libro == null) return 0;

            int prestadosAhora = 0;
            foreach (Prestamo p in todosPrestamos)
            {
                if (p.LibroISBN == isbn && p.EstadoId == estadoActivoId)
                {
                    prestadosAhora++;
                }
            }

            return libro.CantidadCopias - prestadosAhora;
        }

        private void HacerReserva(Socio socio, Libro libro)
        {
            EstadoReserva estadoPendiente = BuscarEstadoReserva("Pendiente");


            List<Reserva> todasReservas = _context.Reservas.ToList();
            bool yaReservado = false;

            foreach (Reserva r in todasReservas)
            {
                if (r.SocioId == socio.NroSocio && r.LibroISBN == libro.ISBN && r.EstadoId == estadoPendiente.Id)
                {
                    yaReservado = true;
                    break;
                }
            }

            if (yaReservado)
            {
                Console.WriteLine("Ya tiene una reserva pendiente para este libro.");
                return;
            }

            Reserva nuevaReserva = new Reserva();
            nuevaReserva.SocioId = socio.NroSocio;
            nuevaReserva.LibroISBN = libro.ISBN;
            nuevaReserva.FechaReserva = DateTime.Now;
            nuevaReserva.EstadoId = estadoPendiente.Id;

            _context.Reservas.Add(nuevaReserva);
            _context.SaveChanges();

            Console.WriteLine("Reserva registrada. Se le avisara cuando el libro este disponible.");
        }


        private EstadoPrestamo BuscarEstadoPrestamo(string nombre)
        {
            List<EstadoPrestamo> estados = _context.estadoPrestamo.ToList();

            foreach (EstadoPrestamo estado in estados)
            {
                if (estado.Nombre == nombre)
                {
                    return estado;
                }
            }

            return null;
        }


        private EstadoReserva BuscarEstadoReserva(string nombre)
        {
            List<EstadoReserva> estados = _context.estadoReserva.ToList();

            foreach (EstadoReserva estado in estados)
            {
                if (estado.Nombre == nombre)
                {
                    return estado;
                }
            }

            return null;
        }
    }
}
