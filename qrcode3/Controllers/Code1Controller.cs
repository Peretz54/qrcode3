using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace qrcode3.Controllers
{
    [AllowAnonymous]
    public class Code1Controller : Controller
    {
        private MephiWebAppContext _db;
        public Code1Controller(MephiWebAppContext db)
        {
            _db = db;
        }
        public IActionResult Index(string c, string n)
        {
            try
            {
                
                if (Request.Query.ContainsKey("c") && Request.Query.ContainsKey("n"))
                {
                    //Console.WriteLine($"{Request.Query["corp"]}  {Request.Query["number"]}");

                    string corp = c;    //Request.Query["c"].ToString();
                    int my_number = int.Parse(n);    //int.Parse(Request.Query["number"]);
                    var info_room = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {corp} AND number_room = {my_number}").ToList();   //pavilion = {corp} AND
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
                else
                {
                    return StatusCode(404);
                    //Response.WriteAsync("Ресурс не найден");
                }
            }
            catch
            {
                return StatusCode(404);
            }
        }
        public IActionResult Qr()
        {
            return View();
        }
    }
}