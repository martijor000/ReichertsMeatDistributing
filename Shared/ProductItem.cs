using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
    {
        public ProductItem(string itemID, string stockingUM, string itemDescription)
        {
            ItemID = itemID;
            StockingUM = stockingUM;
            ItemDescription = itemDescription;
        }

        public string ItemID { get; }
        public string StockingUM { get; }
        public string ItemDescription { get;}
    }
}
