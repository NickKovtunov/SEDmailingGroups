using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class SearchGroup
    {
        public string Name { get; set; }
        public Guid? Guid { get; set; }
        public string Mail { get; set; }

        public static SearchGroup FromDirectoryEntry(System.DirectoryServices.DirectoryEntry entry)
        {
            SearchGroup group = null;
            if (entry.SchemaClassName == "group")
            {
                group = new SearchGroup
                {
                    Name = entry.Properties["cn"].Value?.ToString(),
                    Guid = entry.Guid,
                    Mail = entry.Properties["Mail"].Value?.ToString(),
                };
            }

            return group;
        }
    }
}
