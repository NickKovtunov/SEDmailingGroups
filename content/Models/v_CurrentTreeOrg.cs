using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class v_CurrentTreeOrg
    {
        public Guid? OuId { get; set; }

        public int? ID { get; set; }

        public int? PID { get; set; }

        public string SED_NAME { get; set; }

        public string SHORT_NAME { get; set; }

        public string LONG_NAME { get; set; }

        public int? level { get; set; }

        [StringLength(4000)]
        public string KOD { get; set; }

        public int? NPP { get; set; }

        public int? NPP_P { get; set; }

        public int? shtat_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? begdata { get; set; }

        [Column(TypeName = "date")]
        public DateTime? enddata { get; set; }
    }
}
