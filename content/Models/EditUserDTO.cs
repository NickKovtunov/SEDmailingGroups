using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class EditUserDTO
    {
        public Guid MailingGroupId { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
