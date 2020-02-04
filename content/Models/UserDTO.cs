using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public string UserFIO { get; set; }
        public string UserJobTitle { get; set; }
        public string UserDepartmentUnit { get; set; }
        public Guid MailingGroupId { get; set; }
        public int Type { get; set; }
        public bool Removed { get; set; }
    }
}
