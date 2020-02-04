using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Person { get; set; }
        public int? ObjectId { get; set; }
        public Guid? ObjectGuid { get; set; }
        public string Place { get; set; }
        public string Fact { get; set; }
    }
}
