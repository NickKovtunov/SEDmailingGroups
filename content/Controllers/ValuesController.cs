using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        MailingGroupContext db;

        public ValuesController(MailingGroupContext context)
        {
            this.db = context;
        }

        [HttpGet("[action]")]
        public bool HasAllRights()
        {
            if (User.IsInRole("GD-URENGOY\\editors-SEDmailingGroups"))
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        private Guid? GetGuidByIdentity(string identity)
        {
            using (var context = new PrincipalContext(ContextType.Domain, "gd-urengoy"))
            {
                UserPrincipal principal = UserPrincipal.FindByIdentity(context, identity);
                if (principal != null)
                {
                    return principal.Guid; //principal.DisplayName;
                }
            }
            return null;
        }

        [HttpGet("[action]")]
        public bool HasAdminRights(Guid id)
        {
            if (User.IsInRole("GD-URENGOY\\editors-SEDmailingGroups"))
            {
                return true;
            }
            else
            {
                var userGuid = GetGuidByIdentity(@User.Identity.Name);

                if (userGuid != null)
                {
                    IEnumerable<User> users = db.Users.Where(s => s.UserGuid == userGuid).Where(s => s.Type == 3).Where(d => d.Deleted == false);
                    Guid? mailingGroupId = id;
                    do
                    {
                        foreach (User i in users)
                        {
                            if (i.MailingGroupId == mailingGroupId)
                            {
                                return true;
                            }
                        }
                        MailingGroup mailingGroup = db.MailingGroups.Find(mailingGroupId);
                        mailingGroupId = mailingGroup.ParentId;
                    }
                    while (mailingGroupId != null);
                }
                return false;
            }
        }

        [HttpGet("[action]")]
        public string GetFullParentName(Guid? id)
        {
            string parentFullName = "";

            while (true)
            {
                MailingGroup mailingGroup = db.MailingGroups.Find(id);
                parentFullName = mailingGroup.Name + parentFullName;
                id = mailingGroup.ParentId;
                if (id == null)
                {
                    return parentFullName;
                }
                else
                {
                    parentFullName = " - " + parentFullName;
                }
            }
        }

        [HttpPost("[action]")]
        public IActionResult DeleteObj([FromBody]MailingGroup obj)
        {
            Guid id = obj.Id;
            if (id == null)
            {
                return BadRequest();
            }

            MailingGroup mailingGroup = db.MailingGroups.Find(id);
            mailingGroup.Deleted = true;
            db.Entry(mailingGroup).State = EntityState.Modified;
            db.SaveChanges();

            DeleteChildren(id);

            Log log = new Log();
            log.Date = DateTime.Now;
            log.Person = @User.Identity.Name;
            log.ObjectGuid = mailingGroup.Id;
            log.Place = "MailingGroup";
            log.Fact = "Удаление группы рассылки";
            db.Logs.Add(log);
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public void DeleteChildren(Guid id)
        {
            IEnumerable<MailingGroup> mailingGroups = db.MailingGroups.Where(t => t.ParentId == id).Where(t => t.Deleted == false);
            foreach (MailingGroup mg in mailingGroups)
            {
                DeleteChildren(mg.Id);
                mg.Deleted = true;
                db.Entry(mg).State = EntityState.Modified;

                Log log = new Log();
                log.Date = DateTime.Now;
                log.Person = @User.Identity.Name;
                log.ObjectGuid = mg.Id;
                log.Place = "MailingGroup";
                log.Fact = "Удаление группы рассылки при удалении родителя";
                db.Logs.Add(log);
            }
        }

        [HttpPost("[action]")]
        public IActionResult CanRestoreObj([FromBody]MailingGroup obj)
        {
            MailingGroup mailingGroup = db.MailingGroups.Find(obj.Id);

            while (mailingGroup.ParentId != null)
            {
                mailingGroup = db.MailingGroups.Find(mailingGroup.ParentId);
                if (mailingGroup.Deleted) return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost("[action]")]
        public IActionResult RestoreObj([FromBody]MailingGroup obj)
        {
            Guid id = obj.Id;

            MailingGroup mailingGroup = db.MailingGroups.Find(id);
            mailingGroup.Deleted = false;
            db.Entry(mailingGroup).State = EntityState.Modified;

            Log log = new Log();
            log.Date = DateTime.Now;
            log.Person = @User.Identity.Name;
            log.ObjectGuid = mailingGroup.Id;
            log.Place = "MailingGroup";
            log.Fact = "Восстановление группы рассылки";
            db.Logs.Add(log);
            db.SaveChanges();

            return Ok();
        }
    }
}
