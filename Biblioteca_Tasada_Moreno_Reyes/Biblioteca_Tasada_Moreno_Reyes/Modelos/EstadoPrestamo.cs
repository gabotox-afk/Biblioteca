using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class EstadoPrestamo
    {
        public int Id { get; set;  }
        public string Nombre { get;set;  }

        public ICollection<Prestamo> Prestamos { get; set; } 


    }
}
