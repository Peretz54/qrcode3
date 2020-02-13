using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qrcode3.Models;

namespace qrcode3.Controllers
{
    [Authorize]
    public class RelationController : Controller
    {
        private MephiWebAppContext _db;

        
        public RelationController(MephiWebAppContext db)
        {
            _db = db;
        }
         [HttpGet]
          public IActionResult RelationRP(string corp = "", string n = "")
          {
              ForRltn info = new ForRltn();
              List<TPerson> lst = new List<TPerson>();

              info.Pavilion = corp;
              info.Number = n;
              var personsDB = _db.TPerson.FromSqlInterpolated($"SELECT * FROM t_person").ToList();

              foreach (var a in personsDB)
              {
                  TPerson r = new TPerson();
                  r.IdPerson = a.IdPerson;
                  r.Surname = a.Surname;
                  r.Name1 = a.Name1 != null ? a.Name1 : "";
                  r.Name2 = a.Name2 != null ? a.Name2 : "";
                  r.PhonePerson = a.PhonePerson != null ? a.PhonePerson : "";
                  r.EmailP = a.EmailP != null ? a.EmailP : "";
                  r.PhotoP = a.PhotoP != null ? a.PhotoP : "";

                  lst.Add(r);
                  // info.Persons.Add(r);
              }
              info.Persons = lst;

              return View(info);
          }
          [HttpPost]
          public IActionResult RelationRP(string Corp, string Number, string person = "")
          {
              try
              {
                  string corp1 = Corp;
                  int n1 = int.Parse(Number);

                  var rm = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE t_room.pavilion = {corp1} AND t_room.number_room = {n1}").ToList();
                  //  $"SELECT * FROM t_room WHERE pavilion = '{Corp}' AND number_room = {Number}
                  int IDroom = rm[0].IdRoom;

                  string[] words = person.Split(' ');
                  if (words.Length == 0)
                      return StatusCode(404);

                  var flag = _db.RoomPerson.FromSqlInterpolated($"SELECT * FROM room_person WHERE id_room = {IDroom} AND id_person = {int.Parse(words[0])}").ToList().Count;

                  if (flag == 0)
                      _db.Database.ExecuteSqlRaw($"INSERT INTO room_person VALUES({IDroom}, {int.Parse(words[0])})");
              }
              catch
              {
                  return StatusCode(404);
              }
              return Redirect("~/Home/Table");
          }
          public IActionResult DeleteRelationRP (string corp = "", string n = "", string person = "")
          {
              try
              {
                  var room = _db.TRoom.FromSqlInterpolated($"SELECT * FROM t_room WHERE pavilion = {corp} AND number_room = {int.Parse(n)}").ToList();
                  int IDroom = room[0].IdRoom;

                  _db.Database.ExecuteSqlRaw($"DELETE FROM room_person WHERE id_room = {IDroom} AND id_person = {int.Parse(person)}");
              }
              catch
              {
                  return StatusCode(404);
              }
              return Redirect("~/Home/Table");
          }
    }
}