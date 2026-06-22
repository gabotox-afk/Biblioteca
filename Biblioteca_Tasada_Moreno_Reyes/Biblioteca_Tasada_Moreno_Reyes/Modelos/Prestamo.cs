using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class Prestamo
    {
        public int Id{  get; set; }
        public int SocioId { get; set; }
        public string LibroISBN { get; set; }
        public DateTime FechaPrestamo {  get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public int Renovado { get; set; }
        public int EstadoId{ get; set; }


        public Socio Socio { get; set; }
        public Libro Libro { get; set; }
        public EstadoPrestamo EstadoPrestamo { get; set; }
    }
}
