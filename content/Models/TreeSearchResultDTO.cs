using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class TreeSearchResultDTO
    {
        public dynamic Tree { get; set; }
        public List<string> OpenNodes { get; set; }
    }
}
