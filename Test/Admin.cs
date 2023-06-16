using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsClassLib
{
    public class Admin
    {
        public int Id { get; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SaltHash { get; set; }
    }
}
