using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditController : ControllerBase
    {
        MailingGroupContext db;

        public EditController(MailingGroupContext context)
        {
            this.db = context;
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult EditObj([FromBody]MailingGroup mailingGroup)
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

            MailingGroup oldMailingGroup = db.MailingGroups.Find(mailingGroup.Id);
            oldMailingGroup.Name = mailingGroup.Name;
            oldMailingGroup.Description = mailingGroup.Description;
            oldMailingGroup.ADgroup = mailingGroup.ADgroup;
            oldMailingGroup.IsSync = mailingGroup.IsSync;
            oldMailingGroup.Type = mailingGroup.Type;
            oldMailingGroup.ParentId = mailingGroup.ParentId;
            db.Entry(oldMailingGroup).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.Date = DateTime.Now;
            log.Person = @User.Identity.Name;
            log.ObjectGuid = mailingGroup.Id;
            log.Place = "MailingGroup";
            log.Fact = "Редактирование группы рассылки";
            db.Logs.Add(log);
            db.SaveChanges();

            return Ok(mailingGroup);
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult EditUnits(EditUnitDTO obj)
        {
            IEnumerable<Unit> units = obj.Units;
            Guid MailingGroupId = obj.MailingGroupId;

            IEnumerable<Unit> oldUnits = db.Units.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.Deleted == false).ToList();

            foreach (var u in oldUnits)
            {
                if (units.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.UnitEmployeeDbId == u.UnitEmployeeDbId).Count() == 0)
                {
                    u.Deleted = true;
                    db.Entry(u).State = EntityState.Modified;

                    Log log = new Log();
                    log.Date = DateTime.Now;
                    log.Person = @User.Identity.Name;
                    log.ObjectId = u.Id;
                    log.Place = "Unit";
                    log.Fact = "Удаление предприятия";
                    db.Logs.Add(log);
                }
            }

            db.SaveChanges();

            foreach (var u in units)
            {
                oldUnits = db.Units.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.Deleted == false).ToList();
                if (oldUnits.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.UnitEmployeeDbId == u.UnitEmployeeDbId).Count() == 0)
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
            }

            return Ok();
        }

        [Authorize(Roles = "GD-URENGOY\\editors-SEDmailingGroups")]
        [HttpPost("[action]")]
        public IActionResult EditUsers(EditUserDTO obj)
        {
            IEnumerable<User> users = obj.Users;
            Guid MailingGroupId = obj.MailingGroupId;

            IEnumerable<User> oldUsers = db.Users.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.Deleted == false).ToList();

            foreach (var u in oldUsers)
            {
                if (users.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.UserGuid == u.UserGuid).Where(a => a.Type == u.Type).Count() == 0)
                {
                    u.Deleted = true;
                    db.Entry(u).State = EntityState.Modified;

                    Log log = new Log();
                    log.Date = DateTime.Now;
                    log.Person = @User.Identity.Name;
                    log.ObjectId = u.Id;
                    log.Place = "User";
                    switch (u.Type)
                    {
                        case 1:
                            log.Fact = "Удаление пользователя";
                            break;
                        case 2:
                            log.Fact = "Удаление утверждающего";
                            break;
                        case 3:
                            log.Fact = "Удаление администратора";
                            break;
                        case 4:
                            log.Fact = "Удаление исключения";
                            break;
                        default:
                            log.Fact = "Удаление сотрудника";
                            break;
                    }
                    db.Logs.Add(log);
                }
            }
            db.SaveChanges();

            foreach (var u in users)
            {
                oldUsers = db.Users.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.Deleted == false).ToList();
                if (oldUsers.Where(a => a.MailingGroupId == MailingGroupId).Where(a => a.UserGuid == u.UserGuid).Where(a => a.Type == u.Type).Count() == 0)
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
            }

            return Ok();
        }
    }
}
