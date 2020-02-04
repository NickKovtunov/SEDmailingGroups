using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class MailingGroupContext : DbContext
    {
        public DbSet<MailingGroup> MailingGroups { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public MailingGroupContext(DbContextOptions<MailingGroupContext> options)
            : base(options)
        { }

        public DbQuery<v_CurrentTreeOrg> v_CurrentTreeOrg { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<v_CurrentTreeOrg>().ToView("v_CurrentTreeOrg");
        }
    }
}
