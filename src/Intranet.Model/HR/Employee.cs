using System;
using Intranet.Model.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zek.Data.Entity;
using Zek.Model.Person;

namespace Intranet.Model.HR
{
    public class Employee
    {
        public int Id { get; set; }
        public Person Person { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }


        public int? JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? BranchId { get; set; }
        public Branch Branch { get; set; }


        public Guid RowGuid { get; set; }

        public string Image { get; set; }
    }

    public class EmployeeMap : EntityTypeMap<Employee>
    {
        public EmployeeMap(ModelBuilder builder) : base(builder)
        {
            ToTable("T_Employee", "HR");

            HasKey(t => t.Id);
            Property(t => t.Id).ValueGeneratedNever();


            HasOne(t => t.Person).WithOne().HasForeignKey<Employee>(t => t.Id).OnDelete(DeleteBehavior.Restrict);

            Property(t => t.StartDate).HasColumnType("date");
            Property(t => t.EndDate).HasColumnType("date");


            HasOne(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            HasOne(t => t.JobTitle).WithMany().HasForeignKey(t => t.JobTitleId).OnDelete(DeleteBehavior.Restrict);
            HasOne(t => t.Branch).WithMany().HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);

            Property(t => t.RowGuid).HasDefaultValueSql("newid()");
            HasIndex(t => t.RowGuid).IsUnique();


            Property(t => t.Image).HasMaxLength(255);

            HasIndex(t => new { t.StartDate, t.EndDate });
        }
    }
}
