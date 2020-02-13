using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using qrcode3.Models;
using QRCoder;

namespace qrcode3.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MephiWebAppContext _db;


        public HomeController(ILogger<HomeController> logger, MephiWebAppContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Table(string corp = "", string number = "")    // отображение списка комната + человек (изм 11.02.2020)
        {
            // FromSqlInterpolated
            //var info_room = _db.Database. ExecuteSqlCommand("select tr.pavilion, tr.number_room, tr.short_desc, tr.long_desc, tr.phone_number, tr.email, tp.surname, tp.name1, tp.name2 from t_room tr left join room_person rp on rp.id_room = tr.Id_room left join t_person tp on tp.id_person = rp.id_person").ToList();
            var infoDB = from tr in _db.TRoom
                         join rp in _db.RoomPerson on tr.IdRoom equals rp.IdRoom into RP2
                         from rp2 in RP2.DefaultIfEmpty()
                         join tp in _db.TPerson on rp2.IdPerson equals tp.IdPerson into TP2
                         from tp2 in TP2.DefaultIfEmpty()
                         select new
                         {
                             Id = tr.IdRoom,
                             Pavilion = tr.Pavilion,
                             Number = tr.NumberRoom,
                             ShortDesc = tr.ShortDesc,
                             LongDesc = tr.LongDesc,
                             Phone = tr.PhoneNumber,
                             Email = tr.Email,
                             //tp2 != null ? tp2.Surname + " " + tp2.Name1 + " " + tp2.Name2 : ""
                             IdP = tp2.IdPerson,
                             Surname = tp2.Surname,
                             Name1 = tp2.Name1,
                             Name2 = tp2.Name2
                         };

            List<RoomFull> info = new List<RoomFull>();

            foreach (var a in infoDB)
            {

                TRoom r = new TRoom();
                r.IdRoom = a.Id;
                
                r.Pavilion = a.Pavilion;
                r.NumberRoom = a.Number;
                r.ShortDesc = a.ShortDesc;  // != null ? {true} : {false}
                r.LongDesc = a.LongDesc != null ? a.LongDesc : "";
                r.LongDesc = r.LongDesc.Length < 65 ? r.LongDesc : r.LongDesc.Substring(0, 50) + "...";
                r.PhoneNumber = a.Phone;
                r.Email = a.Email;
                string str = "";

                if (a.Surname != null)
                {
                    str = a.Surname;
                    if (a.Name1 != null)
                    {
                        str = str + " " + a.Name1.Substring(0, 1) + ".";
                        if (a.Name2 != null)
                            str = str + " " + a.Name2.Substring(0, 1) + ".";
                    }
                }

                if (corp != null && corp != "" && corp != r.Pavilion)
                    continue;
                if (number != null && number != "" && int.Parse(number) != r.NumberRoom)
                    continue;

                info.Add(new RoomFull()
                {
                    IDPerson = a.IdP,
                    RoomInfo = r,
                    PersonInfo = str
                }) ;

            }

            return View(info);
        }
        [HttpPost]
        public IActionResult Table(string Corp = "", string Number = "", string test = null)
        {
            return Redirect($"~/Home/Table?corp={Corp}&number={Number}");
        }

        public IActionResult DeleteRoom(string corp, string n)   //удаление комнаты
        {
            MephiWebAppContext db = _db;
            // string Corp = Request.Query["corp"].ToString();
            //int Number = int.Parse(Request.Query["n"]);
            string Corp = corp;
            int Number = int.Parse(n);

            if (Corp == "" || Number < 100 || Number > 1300)
                return StatusCode(404);
            var info_room = db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room where pavilion = {Corp} AND number_room = {Number}").ToList();
            int i = info_room[0].IdRoom;

            _db.Database.ExecuteSqlRaw($"DELETE FROM room_person WHERE id_room = {i}");  //удаление связей с людьми

            _db.Database.ExecuteSqlRaw($"DELETE FROM t_room WHERE pavilion = '{Corp}' AND number_room = {Number}"); //удаление самой строки

            return RedirectToAction("Table", "Home");
        }

        [HttpGet]
        public IActionResult WriteRoom(string corp, string n)   // изменение или добавление новой комнаты
        {
            if (corp == null || n == null)
            {
                TRoom inf0 = new TRoom();
                return View(inf0);
            }


            var info_room = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {corp} AND number_room = {int.Parse(n)}").ToList();   //pavilion = {corp} AND
            TRoom inf = new TRoom();
            //inf = db.TRoom ;
            inf.IdRoom = info_room[0].IdRoom;
            inf.Pavilion = info_room[0].Pavilion;
            inf.NumberRoom = info_room[0].NumberRoom;
            inf.ShortDesc = info_room[0].ShortDesc;
            inf.LongDesc = info_room[0].LongDesc;
            inf.PhoneNumber = info_room[0].PhoneNumber;
            inf.Email = info_room[0].Email;
            return View(inf);
        }
        [HttpPost]
        public IActionResult WriteRoom(string Corp, string Number, string ShortDesc, string LongDesc, string Phone, string Email)
        {
            //var LongDesc = Request.Form["LongDesc"];
            TRoom UserEnter = new TRoom();
            UserEnter.Pavilion = Corp;
            UserEnter.NumberRoom = int.Parse(Number);
            UserEnter.ShortDesc = ShortDesc;
            UserEnter.LongDesc = LongDesc;
            UserEnter.PhoneNumber = Phone;
            UserEnter.Email = Email;

            int i = 0;
            i = _db.Database.ExecuteSqlRaw($"UPDATE t_room SET short_desc = '{UserEnter.ShortDesc}', long_desc = '{UserEnter.LongDesc}', phone_number = '{UserEnter.PhoneNumber}', email = '{UserEnter.Email}' WHERE pavilion = '{UserEnter.Pavilion}' AND number_room = {UserEnter.NumberRoom}");

            if (i == 0)
                _db.Database.ExecuteSqlRaw($"INSERT INTO t_room (pavilion, number_room, short_desc, long_desc, phone_number, email) VALUES('{UserEnter.Pavilion}', {UserEnter.NumberRoom}, '{UserEnter.ShortDesc}', '{UserEnter.LongDesc}', '{UserEnter.PhoneNumber}', '{UserEnter.Email}')");

            return Redirect($"~/Home/Qrgen?Corp={Corp}&Number={Number}");
            //    }
            //catch
            //      {
            //        return Redirect($"~/Home/WriteRoom?corp={Corp}&n={Number}");

        }
        [HttpGet]
        public IActionResult Qrgen(string Corp, string Number, string Letter = null)   //  qrcode
        {

            //Национального Исследовательского Ядерного Университета Московского Инженерно-Физического Института
            string str = $"НИЯУ МИФИ {Corp}{Number}";

            if (Letter != null)
                str = Letter;

            if (Corp != null && Number != null)
                str = str + $" http://mephiwebapp.somee.com/code1/qr?c={Corp}&n={Number}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile("Resources/MEPHI.png"));


            //  string str2 = $"~/wwwroot/qr{corp}{my_number}";
            //  qrCodeImage.Save(str2);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                QrGen qr = new QrGen(stream.ToArray(), str, Corp, Number);
                return View(qr);
            }
        }
        [HttpPost]
        public IActionResult Qrgen(string Corp1, string Number1, string Letter1, string test = null)
        {
            return Redirect($"~/Home/Qrgen?Corp={Corp1}&Number={Number1}&Letter={Letter1}");
        }
        

        [HttpGet]
        public IActionResult PersonsTable(string surname = "", string name1 = "", string name2 = "")
        {
            var personDB = _db.TPerson.FromSqlInterpolated($"SELECT * FROM t_person").ToList();

            List<TPerson> info = new List<TPerson>();

            foreach (var a in personDB)
            {

                TPerson r = new TPerson();
                r.IdPerson = a.IdPerson;
                r.Surname = a.Surname;
                r.Name1 = a.Name1;
                r.Name2 = a.Name2;
                r.PhonePerson = a.PhonePerson;
                r.EmailP = a.EmailP;
                r.PhotoP = a.PhotoP;

                if (surname != "" && surname != null && surname != r.Surname)
                    continue;
                if (name1 != "" && name1 != null && name1 != r.Name1)
                    continue;
                if (name2 != "" && name2 != null && name2 != r.Name2)
                    continue;

                info.Add(r);
            }

            return View(info);
        }
        [HttpPost]
        public IActionResult PersonsTable(string Surname = "", string Name1 = "", string Name2 = "", string test = "")
        {
            return Redirect($"~/Home/PersonsTable?surname={Surname}&name1={Name1}&name2={Name2}");
        }
        [HttpGet]
        public IActionResult WritePerson(string id = null)
        {
            if (id == null || id == "")
            {
                TPerson inf0 = new TPerson();
                return View(inf0);
            }

            var info_person = _db.TPerson.FromSqlInterpolated($"SELECT * FROM t_person WHERE id_person = {int.Parse(id)}").ToList();
            TPerson inf = new TPerson();

            inf.IdPerson = info_person[0].IdPerson;
            inf.Surname = info_person[0].Surname;
            inf.Name1 = info_person[0].Name1;
            inf.Name2 = info_person[0].Name2;
            inf.PhonePerson = info_person[0].PhonePerson;
            inf.EmailP = info_person[0].EmailP;

            return View(inf);
        }
        [HttpPost]
        public IActionResult WritePerson(string id, string Surname, string Name1, string Name2, string PhoneP, string EmailP)
        {

            if (id == "" || id == null || id == "0")
                _db.Database.ExecuteSqlRaw($"INSERT INTO t_person (surname, name1, name2, phone_person, email_p) VALUES ('{Surname}','{Name1}','{Name2}','{PhoneP}','{EmailP}')");
            else
                _db.Database.ExecuteSqlRaw($"UPDATE t_person SET surname = '{Surname}', name1 = '{Name1}', name2 = '{Name2}', phone_person = '{PhoneP}', email_p = '{EmailP}' WHERE id_person = {int.Parse(id)}");
                       
            return Redirect($"~/Home/PersonsTable");
        } 
        public IActionResult DeletePerson(string id)
        {
            if (id != null && id != "" && int.Parse(id) > 0)
            {
                _db.Database.ExecuteSqlRaw($"DELETE FROM room_person WHERE id_person = {int.Parse(id)}");  //удаление связей с аудиториями

                _db.Database.ExecuteSqlRaw($"DELETE FROM t_person WHERE id_person = {int.Parse(id)}"); //удаление самой строки
            }
            return Redirect("~/Home/PersonsTable");
        }


        /*  


          /*public IActionResult Home()
          {
              return View();
          }
          [HttpPost]
          public IActionResult Home(string Corp, string Number)
          {
              if (Corp == "" || Number == "" || int.Parse(Number) <100 || int.Parse(Number) > 1300)
                  return RedirectToAction("Home", "Home");
              //MephiWebAppContext db = new MephiWebAppContext();
              /*try
              {

                  var info_room = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {Corp} AND number_room = {int.Parse(Number)}").ToList();   //pavilion = {corp} AND
                  Room inf = new Room();
                  //inf = db.TRoom ;
                  inf.IdRoom = info_room[0].IdRoom;
                  inf.Pavilion = info_room[0].Pavilion;
                  inf.NumberRoom = info_room[0].NumberRoom;
                  inf.ShortDesc = info_room[0].ShortDesc;
                  inf.LongDesc = info_room[0].LongDesc;
                  inf.PhoneNumber = info_room[0].PhoneNumber;
                  inf.Email = info_room[0].Email;

                  //if (inf.IdRoom.empty)
              }
              catch
              {
                   _db.Database.ExecuteSqlInterpolated($"INSERT INTO t_room (Pavilion, Number_room) VALUES ({Corp}, {int.Parse(Number)})");// РАБОТАЕТУРААААААА
                  //_db.TRoom.Add(new Room { IdRoom = 2, Pavilion = Corp, NumberRoom = int.Parse(Number), ShortDesc = null, LongDesc = null, PhoneNumber = null, Email = null });

                  var info_room = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {Corp} AND number_room = {int.Parse(Number)}").ToList();   //pavilion = {corp} AND

                  _db.RoomPerson.Add( new RoomPerson { IdRoom = 123, IdPerson = 321});
                  return StatusCode(404);
              }
              return Redirect($"~/Home/WriteRoom?corp={Corp}&n={Number}");
          }


          /*public IActionResult Index()
          {
              return View();
          }

          public IActionResult Privacy()
          {
              return View();
          }

          [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
          public IActionResult Error()
          {
              return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
          }*/
    }
}
