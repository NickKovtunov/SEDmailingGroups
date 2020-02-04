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
    public class DepartmentsTreeController : ControllerBase
    {
        EmployeeDBContext db;

        public DepartmentsTreeController(EmployeeDBContext context)
        {
            this.db = context;
        }

        [HttpGet("[action]")]
        public dynamic getParents()
        {
            var jsonString = "[";

            IEnumerable<v_CurrentTreeOrg> allItems = db.v_CurrentTreeOrg.ToList();
            IEnumerable<v_CurrentTreeOrg> items = db.v_CurrentTreeOrg.Where(a => a.PID == null).OrderBy(a => a.LONG_NAME).ToList();

            foreach (v_CurrentTreeOrg item in items)
            {
                jsonString = jsonString + "{ id: '" + item.ID + "', name: '" + item.LONG_NAME + "'";
                if (allItems.Where(t => t.PID == item.ID).Count() > 0)
                {
                    jsonString = jsonString + ", children: [] }, ";
                }
                else
                {
                    jsonString = jsonString + " }, ";
                }
            }

            jsonString = jsonString + "]";
            dynamic result = JsonConvert.DeserializeObject(jsonString);

            return result;
        }

        [HttpGet("[action]")]
        public dynamic getChildren(int pid)
        {
            var jsonString = "[";

            IEnumerable<v_CurrentTreeOrg> allItems = db.v_CurrentTreeOrg.ToList();
            IEnumerable<v_CurrentTreeOrg> items = db.v_CurrentTreeOrg.Where(a => a.PID == pid).OrderBy(a => a.LONG_NAME).ToList();

            foreach (v_CurrentTreeOrg item in items)
            {
                jsonString = jsonString + "{ id: '" + item.ID + "', name: '" + item.LONG_NAME + "'";
                if (allItems.Where(t => t.PID == item.ID).Count() > 0)
                {
                    jsonString = jsonString + ", children: [] }, ";
                }
                else
                {
                    jsonString = jsonString + " }, ";
                }
            }

            jsonString = jsonString + "]";

            dynamic result = JsonConvert.DeserializeObject(jsonString);

            return result;
        }

        [HttpGet("[action]")]
        public TreeSearchResultDTO getSearchTree(string searchString)
        {
            IEnumerable<v_CurrentTreeOrg> allItems = db.v_CurrentTreeOrg.OrderBy(a => a.LONG_NAME).ToList();

            searchString = searchString.ToLower();
            searchString = searchString.Trim();
            searchString = string.Join(" ", searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            IEnumerable<v_CurrentTreeOrg> searchItems = allItems.Where(s => ((s.SED_NAME.ToString().ToLower().Contains(searchString)) || (s.LONG_NAME.ToString().ToLower().Contains(searchString)))).ToList();

            List<int?> searchItemsId = new List<int?>();
            List<int?> treeItems = new List<int?>();
            List<string> parentItemsId = new List<string>();

            foreach (v_CurrentTreeOrg i in searchItems)
            {
                v_CurrentTreeOrg currentItem = i;
                searchItemsId.Add(currentItem.ID);
                while (currentItem.PID != null)
                {
                    treeItems.Add(currentItem.ID);
                    var parentId = currentItem.PID;
                    currentItem = allItems.Where(a => a.ID == parentId).FirstOrDefault();
                    parentItemsId.Add(currentItem.ID.ToString());
                }
                treeItems.Add(currentItem.ID);
            }

            var jsonString = "[";

            int? pID = null;
            jsonString = jsonString + getSearchTreeChildren(pID, allItems, treeItems, searchItemsId);

            jsonString = jsonString + "]";

            TreeSearchResultDTO result = new TreeSearchResultDTO();
            result.Tree = JsonConvert.DeserializeObject(jsonString);
            result.OpenNodes = parentItemsId;

            return result;
        }

        public string getSearchTreeChildren(int? id, IEnumerable<v_CurrentTreeOrg> allItems, List<int?> treeItems, List<int?> searchItemsId)
        {
            var jsonString = "";

            IEnumerable<v_CurrentTreeOrg> parentItems = allItems.Where(a => a.PID == id).OrderBy(a => a.LONG_NAME).ToList();

            foreach (v_CurrentTreeOrg item in parentItems)
            {
                if (treeItems.Contains(item.ID))
                {
                    jsonString = jsonString + "{ id: '" + item.ID + "', name: '" + item.LONG_NAME + "'";
                    if (searchItemsId.Contains(item.ID))
                    {
                        jsonString = jsonString + ", isSearch: true";
                    }
                    if (allItems.Where(t => t.PID == item.ID).Count() > 0)
                    {
                        jsonString = jsonString + ", children: [" + getSearchTreeChildren(item.ID, allItems, treeItems, searchItemsId) + "] }, ";
                    }
                    else
                    {
                        jsonString = jsonString + " }, ";
                    }
                }
            }

            return jsonString;
        }

        [HttpGet("[action]")]
        public string GetFullDepartmentName(int? id)
        {
            string departmentFullName = "";

            while (true)
            {
                v_CurrentTreeOrg org = db.v_CurrentTreeOrg.Where( a => a.ID == id).FirstOrDefault();
                if (org.SED_NAME == "")
                {
                    departmentFullName = org.LONG_NAME + departmentFullName;
                }
                else
                {
                    departmentFullName = org.SED_NAME + departmentFullName;
                }
                id = org.PID;
                if (id == null)
                {
                    return departmentFullName;
                }
                else
                {
                    departmentFullName = " - " + departmentFullName;
                }
            }
        }
    }
}
