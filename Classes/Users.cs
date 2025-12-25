using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerTelegramBot_Classes
{
    public class Users
    {
        public long IdUser { get; set; }
        public List<Events> Events { get; set; }
        public Users()
        {
            Events = new List<Events>();
        }
        public Users(long idUser) : this()
        {
            IdUser = idUser;
        }
    }
}