using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class BusinessCategory
    {
        public BusinessCategory(int id, string type)
        {
            Id = id;
            Type = type;
        }

        public int Id { get; set; }
        public string Type { get; set; }
    }
}
