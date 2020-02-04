using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class SearchUser
    {
        public string fio { get; set; }

        public string position { get; set; }

        public string department { get; set; }

        public string Login { get; set; }

        public Guid? userGuid { get; set; }

        public bool Removed { get; set; }

        public static SearchUser FromDirectoryEntry(System.DirectoryServices.DirectoryEntry entry)
        {
            SearchUser user = null;
            if (entry.SchemaClassName == "user")
            {
                user = new SearchUser
                {
                    fio = entry.Properties["Name"].Value?.ToString(),
                    position = entry.Properties["description"].Value?.ToString(),
                    department = entry.Properties["department"].Value?.ToString(),
                    Login = entry.Properties["sAMAccountName"].Value?.ToString(),
                    userGuid = entry.Guid,
                    Removed = entry.Parent.Name == "OU=Уволенные" ? true : false
                };
            }

            return user;
        }
    }
}
