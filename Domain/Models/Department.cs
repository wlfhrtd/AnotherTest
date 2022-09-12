using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Xml.Serialization;


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
        [JsonIgnore] // for Newtonsoft serializer
        [IgnoreDataMember] // for System.Text.Json.Serialization
        public virtual DepartmentStatus Status { get; set; }

        // ManyToOne within single table
        [ForeignKey("DepartmentMain")]
        [JsonIgnore]
        [IgnoreDataMember]
        public string? DepartmentMainName { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Department? DepartmentMain { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Department>? Subdepartments { get; set; }
    }

    public enum DepartmentStatus
    {
        Blocked,
        Active,      
    }
}
