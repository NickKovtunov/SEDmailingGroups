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
    public class TreeController : ControllerBase
    {
        MailingGroupContext db;
        EmployeeDBContext EmployeeDB;

        public TreeController(MailingGroupContext context, EmployeeDBContext EmployeeDBcontext)
        {
            this.db = context;
            this.EmployeeDB = EmployeeDBcontext;
        }

        [HttpGet("[action]")]
        public dynamic getParents(bool di, Guid? hi)
        {
            var jsonString = "[";

            IEnumerable<MailingGroup> items = new List<MailingGroup>();

            if (di)
            {
                items = db.MailingGroups.Where(a => a.ParentId == null).OrderBy(a => a.Name).ToList();
            }
            else
            {
                items = db.MailingGroups.Where(a => a.ParentId == null).Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            }
            foreach (MailingGroup item in items)
            {
                jsonString = jsonString + "{ id: '" + item.Id + "', name: '" + item.Name + "', type: '" + item.Type + "', deleted: '" + item.Deleted + "'";
                if (item.Id == hi)
                {
                    jsonString = jsonString + ", locked: true";
                }
                if ((item.Children && di) ||(item.NotDeletedChildren && !di))
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
        public dynamic getChildren(Guid pid, bool di, Guid? hi)
        {
            var jsonString = "[";

            IEnumerable<MailingGroup> items = new List<MailingGroup>();

            if (di)
            {
                items = db.MailingGroups.Where(a => a.ParentId == pid).OrderBy(a => a.Name).ToList();
            }
            else
            {
                items = db.MailingGroups.Where(a => a.ParentId == pid).Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            }

            foreach (MailingGroup item in items)
            {
                jsonString = jsonString + "{ id: '" + item.Id + "', name: '" + item.Name + "', type: '" + item.Type + "', deleted: '" + item.Deleted + "'";
                if (item.Id == hi)
                {
                    jsonString = jsonString + ", locked: true";
                }
                if ((item.Children && di) || (item.NotDeletedChildren && !di))
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

        public IEnumerable<MailingGroup> searchPeople(IEnumerable<MailingGroup> searchItems, Guid? userGuid, int type)
        {
            IEnumerable<User> users = db.Users.Where(a => a.UserGuid == userGuid).Where(a => a.Type == type).Where(a => a.Deleted == false).ToList();
            if (users.Count() > 0)
            {
                List<MailingGroup> searchItemsList = new List<MailingGroup>();
                MailingGroup item = new MailingGroup();
                foreach (User u in users)
                {
                    item = searchItems.Where(a => a.Id == u.MailingGroupId).FirstOrDefault();
                    if (item != null)
                    {
                        searchItemsList.Add(item);
                    }
                }
                searchItems = searchItemsList;
            }
            return searchItems;
        }

        [HttpGet("[action]")]
        public TreeSearchResultDTO getSearchTree(string searchString, bool di, Guid? hi, Guid? user, Guid? approves, Guid? admins, Guid? exceptions, int? department)
        {
            IEnumerable<MailingGroup> allItems = new List<MailingGroup>();

            if (searchString == null) {
                searchString = "";
            };

            if (di)
            {
                allItems = db.MailingGroups.OrderBy(a => a.Name).ToList();
            }
            else
            {
                allItems = db.MailingGroups.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            }

            searchString = searchString.ToLower();
            searchString = searchString.Trim();
            searchString = string.Join(" ", searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            searchString = searchString.Replace("ё", "е");

            IEnumerable<MailingGroup> searchItems = allItems.Where(s => ((s.Name.ToString().ToLower().Contains(searchString)) || (s.Description.ToString().ToLower().Contains(searchString)))).ToList();

            if (user != null)
            {
                searchItems = searchPeople(searchItems, user, 1);
            }

            if (approves != null)
            {
                searchItems = searchPeople(searchItems, approves, 2);
            }

            if (admins != null)
            {
                searchItems = searchPeople(searchItems, admins, 3);
            }

            if (exceptions != null)
            {
                searchItems = searchPeople(searchItems, exceptions, 4);
            }

            if(department != null)
            {
                List<MailingGroup> searchItemsList = new List<MailingGroup>();
                MailingGroup item = new MailingGroup();

                while (department != null)
                {
                    var departmentStr = department.ToString();
                    IEnumerable<Unit> units = db.Units.Where(a => a.UnitEmployeeDbId == departmentStr).Where(a => a.Deleted == false).ToList();
                    if (units.Count() > 0)
                    {
                        foreach (Unit u in units)
                        {
                            item = searchItems.Where(a => a.Id == u.MailingGroupId).FirstOrDefault();
                            if (item != null)
                            {
                                searchItemsList.Add(item);
                            }
                        }
                    }
                    v_CurrentTreeOrg parent = EmployeeDB.v_CurrentTreeOrg.Where(a => a.ID == department).FirstOrDefault();
                    department = parent.PID;
                }

                searchItems = searchItemsList;
            }

            List<Guid> searchItemsId = new List<Guid>();
            List<Guid> treeItems = new List<Guid>();
            List<string> parentItemsId = new List<string>();

            foreach (MailingGroup i in searchItems)
            {
                MailingGroup currentItem = i;
                searchItemsId.Add(currentItem.Id);
                while (currentItem.ParentId != null)
                {
                    treeItems.Add(currentItem.Id);
                    var parentId = currentItem.ParentId;
                    currentItem = allItems.Where(a => a.Id == parentId).FirstOrDefault();
                    parentItemsId.Add(currentItem.Id.ToString());
                }
                treeItems.Add(currentItem.Id);
            }

            var jsonString = "[";

            Guid? pID = null;
            jsonString = jsonString + getSearchTreeChildren(pID, allItems, treeItems, searchItemsId, di, hi);

            jsonString = jsonString + "]";

            TreeSearchResultDTO result = new TreeSearchResultDTO();
            result.Tree = JsonConvert.DeserializeObject(jsonString);
            result.OpenNodes = parentItemsId;

            return result;
        }

        public string getSearchTreeChildren(Guid? id, IEnumerable<MailingGroup> allItems, List<Guid> treeItems, List<Guid> searchItemsId, bool di, Guid? hi)
        {
            var jsonString = "";

            IEnumerable<MailingGroup> parentItems = allItems.Where(a => a.ParentId == id).OrderBy(a => a.Name).ToList();

            foreach (MailingGroup item in parentItems)
            {
                if (treeItems.Contains(item.Id))
                {
                    jsonString = jsonString + "{ id: '" + item.Id + "', name: '" + item.Name + "', type: '" + item.Type + "', deleted: '" + item.Deleted + "'";
                    if (item.Id == hi)
                    {
                        jsonString = jsonString + ", locked: true";
                    }
                    if (searchItemsId.Contains(item.Id))
                    {
                        jsonString = jsonString + ", isSearch: true";
                    }
                    if ((item.Children && di) || (item.NotDeletedChildren && !di))
                    {
                        jsonString = jsonString + ", children: [" + getSearchTreeChildren(item.Id, allItems, treeItems, searchItemsId, di, hi) + "] }, ";
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
        public TreeSearchResultDTO getSearchTreeById(Guid id, bool di, Guid? hi)
        {
            IEnumerable<MailingGroup> allItems = new List<MailingGroup>();

            if (di)
            {
                allItems = db.MailingGroups.OrderBy(a => a.Name).ToList();
            }
            else
            {
                allItems = db.MailingGroups.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            }

            MailingGroup searchItem = allItems.Where(s => s.Id == id).FirstOrDefault();
            List<string> parentItemsId = new List<string>();
            bool cycleContinue = true;
            var jsonString = "";
            while (cycleContinue)
            {
                if(searchItem.ParentId == null) { cycleContinue = false; }
                IEnumerable<MailingGroup> searchItems = allItems.Where(a => a.ParentId == searchItem.ParentId);
                var jsonStringNew = "";
                foreach (MailingGroup i in searchItems)
                {
                    jsonStringNew = jsonStringNew + "{ id: '" + i.Id + "', name: '" + i.Name + "', type: '" + i.Type + "', deleted: '" + i.Deleted + "'";
                    if(i.Id == searchItem.Id && i.Id != id && ((i.Children && di) || (i.NotDeletedChildren && !di)))
                    {
                        jsonStringNew = jsonStringNew + ", children: [" + jsonString + "] }, ";
                        //parentItemsId.Add(i.Id.ToString());
                        parentItemsId.Insert(0, i.Id.ToString());
                    }
                    else if ((i.Children && di) || (i.NotDeletedChildren && !di))
                    {
                        jsonStringNew = jsonStringNew + ", children: [] }, ";
                    }
                    else
                    {
                        jsonStringNew = jsonStringNew + " }, ";
                    }
                }
                jsonString = jsonStringNew;
                searchItem = allItems.Where(s => s.Id == searchItem.ParentId).FirstOrDefault();
            }

            jsonString = "[" + jsonString + "]";

            TreeSearchResultDTO result = new TreeSearchResultDTO();
            result.Tree = JsonConvert.DeserializeObject(jsonString);
            result.OpenNodes = parentItemsId;

            return result;
        }
    }
}
