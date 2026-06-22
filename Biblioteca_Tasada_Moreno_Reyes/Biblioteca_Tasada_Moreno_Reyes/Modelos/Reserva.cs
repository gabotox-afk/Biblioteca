using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class Reserva
    {
        public int Id {  get; set; }
        public int SocioId { get; set; }
        public string LibroISBN { get; set; }
        public DateTime FechaReserva { get; set; }
        public int EstadoId { get; set; }

        public Socio Socio { get; set; }
        public EstadoReserva EstadoReserva { get; set; }
        public Libro Libro { get; set; }




    }
}
