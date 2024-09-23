using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBQA.Models
{
    public class CartItem
    {
        public ProductDetail productDetail { get; set; }
        public int amount { get; set; }
        public double? TotalMoney()
        {
            if (productDetail.Product.Discount == 0)
                return amount * productDetail.Product.Price;
            return amount * productDetail.Product.Discount;
        }

    }
}