namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CatID { get; set; }

        [StringLength(250)]
        public string CatName { get; set; }

        public string Description { get; set; }

        public int? ParentID { get; set; }

        public int? Levels { get; set; }

        public int? Ordering { get; set; }

        public bool Published { get; set; }

        [StringLength(250)]
        public string Thumb { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(250)]
        public string Alias { get; set; }

        [StringLength(250)]
        public string MetaDesc { get; set; }

        [StringLength(250)]
        public string MetaKey { get; set; }

        [StringLength(255)]
        public string Cover { get; set; }

        public string SchemaMarkup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
