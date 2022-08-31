using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models
{
    [Table("Departments")]
    public partial class Department : BaseModel
    {
        public Department()
        {
            Status = DepartmentStatus.Active;
        }

        public Department(string name) : this()
        {
            Name = name;
        }

        public Department(string name, DepartmentStatus status) : this(name)
        {
            Status = status;
        }


        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        [NotMapped]
        public virtual DepartmentStatus Status { get; set; }
        // ManyToMany inside single table using DepartmentMapping
        public virtual ICollection<Department>? Departments { get; set; }
        public virtual ICollection<Department>? Subdepartments { get; set; }
    }

    public enum DepartmentStatus
    {
        Active,
        Blocked,
    }
}
