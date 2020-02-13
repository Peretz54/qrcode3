using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qrcode3.Models
{
    public class RoomFull
    {
        public virtual TRoom RoomInfo { get; set; }
        public string PersonInfo { get; set; }
        public int IDPerson{ get; set; }
    }
}
