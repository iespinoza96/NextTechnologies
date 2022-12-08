using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class ErrorExcel
    {
        public int IdRegistro { get; set; }
        public string Message { get; set; }
        public List<object> Errores { get; set; }
        public ML.Cargo Cargo { get; set; }
        public bool Correct { get; set; }
    }
}
