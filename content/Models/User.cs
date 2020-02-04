using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEDmailingGroups.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public Guid MailingGroupId { get; set; }
        //1 - пользователь
        //2 - утверждающий
        //3 - админы
        //4 - исключения
        public int Type { get; set; }
        public string FIO { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public bool IncorrectInf { get; set; }
        public bool Deleted { get; set; }
    }
}
