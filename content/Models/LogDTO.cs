using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class LogDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Person { get; set; }
        public string Place { get; set; }
        public string Fact { get; set; }
        public string Name { get; set; }
        public string DepId { get; set; }
        public Guid? UserGuid { get; set; }
    }
}
