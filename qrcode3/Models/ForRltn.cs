using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qrcode3.Models
{
    public class ForRltn
    {
        public string Pavilion { get; set; }
        public string Number { get; set; }
        public List<TPerson> Persons { get; set; }
    }
}
