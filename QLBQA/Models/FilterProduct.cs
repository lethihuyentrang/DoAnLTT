using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBQA.Models
{
    public class FilterProduct
    {
        public bool isDiscount { get; set; }
        public bool isHot { get; set; }
        public bool isInStock { get; set; }

        public bool isLowPrice { get; set; }
        public bool isMediumPrice { get; set; }
        public bool isHighPrice { get; set; }
        public bool isVeryHighPrice { get; set; }
        public bool isSSize { get; set; }
        public bool isMSize { get; set; }
        public bool isLSize { get; set; }
        public bool isXLSize { get; set; }
        public bool is2XLSize { get; set; }
        public bool is3XLSize { get; set; }

        public int CatID { get; set; }
        public int? page { get; set; }
    }
}   