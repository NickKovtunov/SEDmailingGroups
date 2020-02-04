using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        MailingGroupContext db;

        public ShowController(MailingGroupContext context)
        {
            this.db = context;
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Get(Guid id)
        {
            MailingGroup mg = db.MailingGroups.Find(id);
            if (mg == null)
                return NotFound();
            return new ObjectResult(mg);
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetPeople(Guid id)
        {
            IEnumerable<User> people = db.Users.Where(a => a.MailingGroupId == id).Where(a => a.Deleted == false).OrderBy(a => a.FIO);
            return new ObjectResult(people);
        }
    }
}
