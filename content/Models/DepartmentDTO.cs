using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string FullTitle { get; set; }
        [MaxLength(50)]
        public string UnitEmployeeDbId { get; set; }
    }
}
