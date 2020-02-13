using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace qrcode3.Models
{
    public class QrGen
    {
    
        public byte[] Img { get; set; }
        public string Letter { get; set; }
        public string Pavilion { get; set; }
        public string Number { get; set; }
        public QrGen(byte[] img0, string letter0, string pav, string n)
        {
            Img = img0;
            Letter = letter0;
            Pavilion = pav;
            Number = n;
        }
    }
        
}
