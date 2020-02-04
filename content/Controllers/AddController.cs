using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddController : ControllerBase
    {
        MailingGroupContext db;

        public AddController(MailingGroupContext context)
        {
            this.db = context;
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult AddObj([FromBody]MailingGroup mailingGroup)
        {
            if (mailingGroup == null)
            {
                return BadRequest();
            }

            mailingGroup.Name = mailingGroup.Name.Trim();
            mailingGroup.Name = string.Join(" ", mailingGroup.Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            mailingGroup.Name = mailingGroup.Name.Replace("ё", "е");

            mailingGroup.Description = mailingGroup.Description.Trim();
            mailingGroup.Description = string.Join(" ", mailingGroup.Description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            mailingGroup.Description = mailingGroup.Description.Replace("ё", "е");

            db.MailingGroups.Add(mailingGroup);
            db.SaveChanges();

            Log log = new Log();
            log.Date = DateTime.Now;
            log.Person = @User.Identity.Name;
            log.ObjectGuid = mailingGroup.Id;
            log.Place = "MailingGroup";
            log.Fact = "Создание группы рассылки";
            db.Logs.Add(log);
            db.SaveChanges();

            return Ok(mailingGroup);
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult AddUnits(IEnumerable<Unit> units)
        {
            if (units == null)
            {
                return BadRequest();
            }

            foreach (var u in units)
            {
                db.Units.Add(u);
                db.SaveChanges();

                Log log = new Log();
                log.Date = DateTime.Now;
                log.Person = @User.Identity.Name;
                log.ObjectId = u.Id;
                log.Place = "Unit";
                log.Fact = "Создание предприятия";
                db.Logs.Add(log);
                db.SaveChanges();
            }

            return Ok();
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult AddUsers(IEnumerable<User> users)
        {
            if (users == null)
            {
                return BadRequest();
            }

            foreach (var u in users)
            {
                db.Users.Add(u);
                db.SaveChanges();

                Log log = new Log();
                log.Date = DateTime.Now;
                log.Person = @User.Identity.Name;
                log.ObjectId = u.Id;
                log.Place = "User";
                switch (u.Type)
                {
                    case 1:
                        log.Fact = "Создание пользователя";
                        break;
                    case 2:
                        log.Fact = "Создание утверждающего";
                        break;
                    case 3:
                        log.Fact = "Создание администратора";
                        break;
                    case 4:
                        log.Fact = "Создание исключения";
                        break;
                    default:
                        log.Fact = "Создание сотрудника";
                        break;
                }
                db.Logs.Add(log);
                db.SaveChanges();
            }

            return Ok();
        }
    }
}
