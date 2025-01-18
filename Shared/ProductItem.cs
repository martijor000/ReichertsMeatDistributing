using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
    {
        public ProductItem(int ID, string itemID, string stockingUM)
        {
            Id = ID;
            ItemId = itemID;
            StockingUm = stockingUM;
        }

        public int Id { get; }
        public string ItemId { get; }
        public string StockingUm { get; }
    }
}
