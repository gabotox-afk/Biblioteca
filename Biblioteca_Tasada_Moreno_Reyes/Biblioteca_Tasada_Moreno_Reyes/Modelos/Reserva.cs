using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class Reserva
    {
        public int id {  get; set; }
        public int Socioid { get; set; }
        public string LibroISBN { get; set; }
        public DateTime FechaReserva { get; set; }
        public int Estadoid { get; set; }

        public Socio Socio { get; set; }
        public estadoReserva EstadoReserva { get; set; }
        public Libro Libro { get; set; }

    }
}
