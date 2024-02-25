using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
{
        [Key]
        public int ID { get; set; }
        public string ItemID { get; set; }
        public string StockingUM { get; set; }
        public string ItemDescription { get; set; }
        public int BusinessCategoryID { get; set; }
        public BusinessCategories BusinessCategory { get; set; }
    }


}
