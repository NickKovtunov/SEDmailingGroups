using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SEDmailingGroups.Models;

namespace SEDmailingGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADUserController : ControllerBase
    {
        protected static Guid GduPolzovateli = new Guid("{06C47007-9D3F-4691-8930-BBF8C6D08B1A}");
        protected static string RootEntry = string.Format("LDAP://<GUID={0}>", GduPolzovateli.ToString());

        [HttpGet("[action]")]
        [AllowAnonymous]
        public dynamic Search(string term)
        {
            var result = new List<Models.SearchUser>();

            using (var dEntry = new DirectoryEntry(RootEntry))
            {
                using (var dSearcher = new DirectorySearcher(dEntry))
                {
                    dSearcher.PropertiesToLoad.Add("objectGUID");
                    dSearcher.PropertiesToLoad.Add("name");
                    dSearcher.PropertiesToLoad.Add("mail");
                    dSearcher.PropertiesToLoad.Add("sAMAccountName");
                    dSearcher.PropertiesToLoad.Add("Description");
                    if (!string.IsNullOrEmpty(term))
                    {
                        dSearcher.Filter = string.Format("(&(objectCategory=person)(objectClass=user)(|(DisplayName={0}*)(sAMAccountName=*{0}*){1}))",
                            term,
                            GetGuidFilter(term));
                    }
                    else
                    {
                        dSearcher.Filter = "(&(objectCategory=person)(objectClass=user))";
                    }

                    dSearcher.CacheResults = false;
                    dSearcher.SizeLimit = 10;
                    dSearcher.PageSize = 0;

                    dSearcher.Sort = new SortOption("Name", SortDirection.Ascending);
                    var searchResults = dSearcher.FindAll();
                    foreach (SearchResult searchReuslt in searchResults)
                    {
                        var entry = searchReuslt.GetDirectoryEntry();
                        if (entry != null)
                        {
                            Models.SearchUser user = Models.SearchUser.FromDirectoryEntry(entry);
                            result.Add(user);
                        }
                    }
                }
                return JsonConvert.SerializeObject(result);
            }
        }

        private string GetGuidFilter(string guidstring)
        {
            Guid guid;
            if (Guid.TryParse(guidstring, out guid))
            {
                byte[] bytes = guid.ToByteArray();

                StringBuilder sb = new StringBuilder();

                foreach (byte b in bytes)
                {
                    sb.Append(string.Format(@"\{0}", b.ToString("X")));
                }

                return string.Format("(objectGUID={0})", sb.ToString());
            }

            return string.Empty;
        }
    }
}
