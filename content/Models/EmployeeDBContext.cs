using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class EmployeeDBContext : DbContext
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options)
            : base(options)
        { }

        public DbQuery<v_CurrentTreeOrg> v_CurrentTreeOrg { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<v_CurrentTreeOrg>().ToView("v_CurrentTreeOrg");
        }
    }
}
