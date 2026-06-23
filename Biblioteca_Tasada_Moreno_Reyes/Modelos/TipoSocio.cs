using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class TipoSocio
    {
        public int Id {  get; set; }
        public string Nombre { get; set; }
        public int Maxlibros { get; set; } 
        public string DiasPrestamo { get; set; }
        public double MultaPorDia { get; set; }

        public ICollection<Socio> Socios { get; set; }
    }
}
