using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        MailingGroupContext db;
        EmployeeDBContext EmployeeDB;

        public DepartmentsController(MailingGroupContext context, EmployeeDBContext EmployeeDBcontext)
        {
            this.db = context;
            this.EmployeeDB = EmployeeDBcontext;
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetDepartments(Guid id)
        {
            List<DepartmentDTO> departmentsDTO = new List<DepartmentDTO>();
            IEnumerable<Unit> departments = db.Units.Where(a => a.MailingGroupId == id).Where(t => t.Deleted == false);
            
            foreach (var d in departments)
            {
                DepartmentDTO departmentDTO = new DepartmentDTO();
                departmentDTO.Id = d.Id;
                departmentDTO.FullTitle = GetFullOrgName(d.UnitEmployeeDbId);
                departmentDTO.UnitEmployeeDbId = d.UnitEmployeeDbId;
                departmentsDTO.Add(departmentDTO);
            }

            return new ObjectResult(departmentsDTO);
        }

        [HttpGet("[action]")]
        public IEnumerable<LogDTO> GetHistory(Guid id)
        {
            List<LogDTO> historyList = new List<LogDTO>();

            IEnumerable<Log> allLogs = db.Logs.ToList();

            IEnumerable<Log> logs = allLogs.Where(a => a.ObjectGuid == id);

            foreach (Log l in logs)
            {
                LogDTO newLog = new LogDTO();
                newLog.Id = l.Id;
                newLog.Date = l.Date;
                newLog.Person = l.Person;
                newLog.Place = l.Place;
                newLog.Fact = l.Fact;
                newLog.Name = "";
                newLog.DepId = null;
                newLog.UserGuid = null;
                historyList.Add(newLog);
            }

            IEnumerable<User> users = db.Users.Where(a => a.MailingGroupId == id).ToList();
            foreach (User u in users)
            {
                logs = allLogs.Where(a => a.ObjectId == u.Id).Where(a => a.Place == "User").ToList();
                foreach (Log l in logs)
                {
                    LogDTO newLog = new LogDTO();
                    newLog.Id = l.Id;
                    newLog.Date = l.Date;
                    newLog.Person = l.Person;
                    newLog.Place = l.Place;
                    newLog.Fact = l.Fact;
                    newLog.Name = u.FIO;
                    newLog.DepId = null;
                    newLog.UserGuid = u.UserGuid;
                    historyList.Add(newLog);
                }
            }

            IEnumerable<Unit> units = db.Units.Where(a => a.MailingGroupId == id).ToList();
            foreach (Unit u in units)
            {
                logs = allLogs.Where(a => a.ObjectId == u.Id).Where(a => a.Place == "Unit").ToList();
                foreach (Log l in logs)
                {
                    LogDTO newLog = new LogDTO();
                    newLog.Id = l.Id;
                    newLog.Date = l.Date;
                    newLog.Person = l.Person;
                    newLog.Place = l.Place;
                    newLog.Fact = l.Fact;
                    newLog.Name = GetFullOrgName(u.UnitEmployeeDbId);
                    newLog.DepId = u.UnitEmployeeDbId;
                    newLog.UserGuid = null;
                    historyList.Add(newLog);
                }
            }

            IEnumerable<LogDTO> history = historyList.OrderByDescending(a => a.Date).Distinct();

            return history;
        }

        //Получение полного имени подразделения при редактировании
        //На входе - EmployDBId подразделения, на выходе - полная цепочка подразделения
        public string GetFullOrgName(string id)
        {
            IEnumerable<v_CurrentTreeOrg> orgStructures = EmployeeDB.v_CurrentTreeOrg.ToList();

            int? rightId = Convert.ToInt32(id);
            string fullName = "";

            while (true)
            {
                IEnumerable<v_CurrentTreeOrg> departments = orgStructures.Where(t => t.ID == rightId);
                foreach (v_CurrentTreeOrg d in departments)
                {
                    if (d.SED_NAME == "")
                    {
                        fullName = d.LONG_NAME + fullName;
                    }
                    else
                    {
                        fullName = d.SED_NAME + fullName;
                    }
                    rightId = d.PID;
                    if (rightId == null)
                    {
                        return fullName;
                    }
                    else
                    {
                        fullName = " - " + fullName;
                    }
                }
            }
        }
    }
}
