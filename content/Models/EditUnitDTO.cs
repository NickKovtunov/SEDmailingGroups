using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class EditUnitDTO
    {
        public Guid MailingGroupId { get; set; }
        public IEnumerable<Unit> Units { get; set; }
    }
}
