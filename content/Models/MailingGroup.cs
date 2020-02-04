using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class MailingGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Children { get; set; }
        public bool NotDeletedChildren { get; set; }
        public bool Deleted { get; set; }
        public Guid? ADgroup { get; set; }
        public bool IsSync { get; set; }
        // 0 - группа (по умолчанию)
        // 1 - контейнер
        public int Type { get; set; }
    }
}
