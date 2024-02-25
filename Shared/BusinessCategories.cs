using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class BusinessCategories
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
    }
}
