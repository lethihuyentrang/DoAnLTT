namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Shipper
    {
        public int ShipperID { get; set; }

        [StringLength(150)]
        public string ShipperName { get; set; }

        [StringLength(12)]
        public string Phone { get; set; }

        [StringLength(150)]
        public string Company { get; set; }

        public DateTime? ShipDate { get; set; }
    }
}
