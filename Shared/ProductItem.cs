using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
    {
        public ProductItem(int Id,string itemID, string stockingUM, string itemDescription, int businessCategoryID)
        {
            ID = Id;
            ItemID = itemID;
            StockingUM = stockingUM;
            ItemDescription = itemDescription;
            BusinessCategoryID = businessCategoryID;

        }

        public int ID { get; set; }
        public string ItemID { get; set; }
        public string StockingUM { get; set; }
        public string ItemDescription { get; set; }
        public int BusinessCategoryID { get; set; }
    }
}
