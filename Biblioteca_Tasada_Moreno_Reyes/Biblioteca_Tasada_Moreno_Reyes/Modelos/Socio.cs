using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class Socio
    {
        public int NroSocio { get; set; }
        public string NombreApellido { get; set; }
        public string Email { get; set; }
        public int TipoSocioId { get; set; }
        public int Activo { get; set; }


        public TipoSocio TipoSocio { get; set; }


        public ICollection<Prestamo> Prestamos { get; set; }
        public ICollection<Reserva> Reservas { get; set; }


    }
}
