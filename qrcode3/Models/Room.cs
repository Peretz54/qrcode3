using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qrcode3.Models
{
    public class Room
    {
        public short IdRoom { get; set; }
        public string Pavilion { get; set; }
        public int NumberRoom { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
