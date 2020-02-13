using System;
using System.Collections.Generic;

namespace qrcode3
{
    public partial class TRoom
    {
        public short IdRoom { get; set; }
        public string Pavilion { get; set; }
        public int NumberRoom { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        //public string Pathqr { get; set; }


        /*  public TRoom GetRoomInf(string corp, int my_number)
          {
              MephiWebAppContext db = new MephiWebAppContext();
              //string str = "";

              var info_room = db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {corp} AND number_room = {my_number}").ToList();   //pavilion = {corp} AND
              TRoom inf = new TRoom();
              //inf = db.TRoom ;
              inf.IdRoom = info_room[0].IdRoom;
              inf.Pavilion = info_room[0].Pavilion;
              inf.NumberRoom = info_room[0].NumberRoom;
              inf.ShortDesc = info_room[0].ShortDesc;
              inf.LongDesc = info_room[0].LongDesc;
              inf.PhoneNumber = info_room[0].PhoneNumber;
              inf.Email = info_room[0].Email;

              return inf;
          }*/
    }
}
