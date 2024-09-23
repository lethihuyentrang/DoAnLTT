namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail
    {
        public int OrderDetailID { get; set; }

        public int? OrderID { get; set; }

        public int? ProductDetailID { get; set; }



        public int? Quantity { get; set; }

        public int? Price { get; set; }

  

        public DateTime? ShipDate { get; set; }

        public virtual Order Order { get; set; }

       
    }
}
