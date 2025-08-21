using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ReichertsMeatDistributing.Shared
{
    public class ProductItem
    {
        public ProductItem()
        {
            ItemID = string.Empty;
            StockingUM = string.Empty;
            ItemDescription = string.Empty;
            Category = BusinessCategory.All;
        }

        public ProductItem(string itemID, string stockingUM, string itemDescription)
        {
            ItemID = itemID;
            StockingUM = stockingUM;
            ItemDescription = itemDescription;
            Category = BusinessCategory.All;
        }

        [Key]
        [Required]
        [MaxLength(100)]
        public string ItemID { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string StockingUM { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string ItemDescription { get; set; }
        
        public BusinessCategory Category { get; set; } = BusinessCategory.All;
        
        public decimal? Price { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ModifiedDate { get; set; }
    }
}
