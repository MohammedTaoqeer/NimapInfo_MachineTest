using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductImageName { get; set; }
        public bool Discontinued { get; set; }
        public Category Category { get; set; }
    }
}