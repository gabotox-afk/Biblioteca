using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_Tasada_Moreno_Reyes.Modelos
{
    public class TipoSocio
    {
        public int id {  get; set; }
        public string nombre { get; set; }
        public int Maxlibros { get; set; } 
        public string DiasPrestamo { get; set; }
        public double multaPorDia { get; set; }


    }
}
