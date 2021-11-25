using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto1.Models
{
    public class StringMessage
    {
        public String UsuarioEmisor { get; set; }
        public String UsuarioReceptor { get; set; }
        public String Texto { get; set; }
        public string Fecha_envio { get; set; }
        public string FilePath { get; set; }
        public int SalaID { get; set; }
    }
}
