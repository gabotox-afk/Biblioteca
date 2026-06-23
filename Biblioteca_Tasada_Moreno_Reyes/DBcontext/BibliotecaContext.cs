using Microsoft.EntityFrameworkCore;
using Biblioteca_Tasada_Moreno_Reyes.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Biblioteca_Tasada_Moreno_Reyes.DBcontext
{
    public class BibliotecaContext : DbContext
    {
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<TipoSocio> tipoSocio { get; set; }
        public DbSet<EstadoReserva> estadoReserva { get; set; }
        public DbSet<EstadoPrestamo> estadoPrestamo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Biblioteca.db");            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Libro>().HasKey(l => l.ISBN);

            modelBuilder.Entity<Socio>().HasKey(s => s.NroSocio);

            modelBuilder.Entity<Socio>().Property(s => s.NroSocio).ValueGeneratedNever();

            modelBuilder.Entity<Socio>()
                .HasOne(s => s.TipoSocio)
                .WithMany(ts => ts.Socios)
                .HasForeignKey(s => s.TipoSocioId);

            modelBuilder.Entity<TipoSocio>().HasKey(ts => ts.Id);


            modelBuilder.Entity<Prestamo>().HasKey(p => p.Id);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Socio)
                .WithMany(s => s.Prestamos)
                .HasForeignKey(p => p.SocioId);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany()
                .HasForeignKey(p => p.LibroISBN);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.EstadoPrestamo)
                .WithMany(ep => ep.Prestamos)
                .HasForeignKey(p => p.EstadoId);



            modelBuilder.Entity<Reserva>().HasKey(r => r.Id);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Socio)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.SocioId);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Libro)
                .WithMany()
                .HasForeignKey(r => r.LibroISBN);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.EstadoReserva)
                .WithMany(er => er.Reservas)
                .HasForeignKey(r => r.EstadoId);



            modelBuilder.Entity<EstadoPrestamo>().HasKey(ep => ep.Id);
            modelBuilder.Entity<EstadoReserva>().HasKey(er => er.Id);
        }

    }
}
