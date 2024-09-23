namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;
    public partial class Post
    {
        public int PostID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string SContents { get; set; }

        [AllowHtml]
        public string Contents { get; set; }

        [StringLength(255)]
        public string Thumb { get; set; }

        public bool Pulished { get; set; }

        [StringLength(255)]
        public string Alias { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(255)]
        public string Author { get; set; }

        public int? AccountID { get; set; }

        public string Tags { get; set; }

        public int? CatID { get; set; }

        public bool isHot { get; set; }

        public bool isNewfeed { get; set; }

        [StringLength(255)]
        public string MetaDesc { get; set; }

        [StringLength(255)]
        public string MetaKey { get; set; }

        public int? Views { get; set; }
    }
}
