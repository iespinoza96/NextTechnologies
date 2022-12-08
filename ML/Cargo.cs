using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class Cargo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string company_id { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string paid_at { get; set; }
        public List<object> Cargos { get; set; }

    }
}
