using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public Guid MailingGroupId { get; set; }
        [MaxLength(50)]
        public string UnitEmployeeDbId { get; set; }
        public bool Deleted { get; set; }
    }
}
