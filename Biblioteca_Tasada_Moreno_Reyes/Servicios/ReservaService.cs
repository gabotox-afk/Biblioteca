using System;
using System.Collections.Generic;
using Biblioteca_Tasada_Moreno_Reyes.DBcontext;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_Tasada_Moreno_Reyes.Servicios
{
    public class ReservaService
    {
        private BibliotecaContext _context;

        public ReservaService(BibliotecaContext context)
        {
            _context = context;
        }


        public void RealizarReserva()
        {
            Console.WriteLine("\n=== REALIZAR RESERVA ===");

            Console.Write("Ingrese numero de socio: ");
            string entradaSocio = Console.ReadLine();

            if (!int.TryParse(entradaSocio, out int nroSocio))
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

            // RN-01: un socio inactivo no puede reservar
            if (socio.Activo != 1)
            {
                Console.WriteLine("El socio esta inactivo y no puede hacer reservas.");
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
                Console.WriteLine($"  [{i + 1}] {librosEncontrados[i].Titulo} - {librosEncontrados[i].Autor}");
            }

            Console.Write("Elija un libro (numero): ");
            string entradaLibro = Console.ReadLine();

            if (!int.TryParse(entradaLibro, out int eleccion) || eleccion < 1 || eleccion > librosEncontrados.Count)
            {
                Console.WriteLine("Eleccion invalida.");
                return;
            }

            Libro libroElegido = librosEncontrados[eleccion - 1];

            EstadoReserva estadoPendiente = BuscarEstadoReserva("Pendiente");


            // RN-08: un socio no puede tener mas de una reserva activa para el mismo libro
            List<Reserva> todasReservas = _context.Reservas.ToList();
            bool yaReservado = false;

            foreach (Reserva r in todasReservas)
            {
                if (r.SocioId == nroSocio && r.LibroISBN == libroElegido.ISBN && r.EstadoId == estadoPendiente.Id)
                {
                    yaReservado = true;
                    break;
                }
            }

            if (yaReservado)
            {
                Console.WriteLine("El socio ya tiene una reserva pendiente para este libro.");
                return;
            }


            Reserva nuevaReserva = new Reserva();
            nuevaReserva.SocioId = socio.NroSocio;
            nuevaReserva.LibroISBN = libroElegido.ISBN;
            nuevaReserva.FechaReserva = DateTime.Now;
            nuevaReserva.EstadoId = estadoPendiente.Id;

            _context.Reservas.Add(nuevaReserva);
            _context.SaveChanges();

            Console.WriteLine("\nReserva registrada!");
            Console.WriteLine("  Libro: " + libroElegido.Titulo);
            Console.WriteLine("  Socio: " + socio.NombreApellido);
            Console.WriteLine("  Se le avisara cuando el libro este disponible.");
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
