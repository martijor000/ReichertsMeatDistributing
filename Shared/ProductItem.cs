using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
    {
        public ProductItem(int id, string itemID, string stockingUM, string description)
        {
            Id = id;
            ItemId = itemID;
            StockingUm = stockingUM;
            Description = description;

        }

        public int Id { get; }
        public string ItemId { get; }
        public string StockingUm { get; }
        public string Description { get; }
    }
}
