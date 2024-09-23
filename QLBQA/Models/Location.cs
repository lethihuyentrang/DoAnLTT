namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Location
    {
        public int LocationID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(100)]
        public string Slug { get; set; }

        [StringLength(255)]
        public string NameWithType { get; set; }

        [StringLength(255)]
        public string PathWithType { get; set; }

        public int? ParrentCode { get; set; }

        public int? Levels { get; set; }
    }
}
