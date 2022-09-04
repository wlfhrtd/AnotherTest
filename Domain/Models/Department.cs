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

        [NotMapped]
        public virtual DepartmentStatus Status { get; set; }

        // ManyToOne within single table
        [ForeignKey("DepartmentMain")]
        public string? DepartmentMainName { get; set; }
        public virtual Department? DepartmentMain { get; set; }
        public virtual ICollection<Department>? Subdepartments { get; set; }
    }

    public enum DepartmentStatus
    {
        Blocked,
        Active,      
    }
}
