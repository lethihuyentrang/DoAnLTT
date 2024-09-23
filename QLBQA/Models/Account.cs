namespace QLBQA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
        public int AccountID { get; set; }

        [StringLength(12)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(6)]
        public string Salt { get; set; }

        public bool Active { get; set; }

        [StringLength(150)]
        public string FullName { get; set; }

        public int? RoleID { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? CreateDate { get; set; }

        public virtual Role Role { get; set; }
    }
}
