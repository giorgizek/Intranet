using System;
using System.Collections.Generic;
using Intranet.Model.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zek.Data.Entity;
using Zek.Model.Base;
using Zek.Model.Identity;
using Zek.Model.Production;

namespace Intranet.Model.Document
{
    public class Document : BaseModel<int>
    {
        //public DateTime Date { get; set; }
        public int TemplateId { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }


        public int? JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }

        public int? BranchId { get; set; }
        public Branch Branch { get; set; }


        public List<DocumentProduct> DocumentProducts { get; set; }


        public bool IsApproved { get; set; }
        public int? ApproverId { get; set; }
        public User Approver { get; set; }
        public DateTime? ApproveDate { get; set; }

    }

    public class DocumentMap : BaseModelMap<Document, int>
    {
        public DocumentMap(ModelBuilder builder) : base(builder)
        {
            ToTable("T_Document", "Document");

            HasIndex(t => t.TemplateId);


            HasOne(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            HasOne(t => t.JobTitle).WithMany().HasForeignKey(t => t.JobTitleId).OnDelete(DeleteBehavior.Restrict);
            HasOne(t => t.Branch).WithMany().HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);

            Property(t => t.IsApproved).HasDefaultValue(false);
            HasIndex(t => t.IsApproved);

            HasOne(t => t.Approver).WithMany().HasForeignKey(t => t.ApproverId).OnDelete(DeleteBehavior.Restrict);

            Property(t => t.ApproveDate).HasColumnType("datetime2(0)");
            HasIndex(t => t.ApproveDate);
        }
    }


    public class DocumentProduct
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; }



        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }

    public class DocumentProductMap : EntityTypeMap<DocumentProduct>
    {
        public DocumentProductMap(ModelBuilder builder) : base(builder)
        {
            ToTable("T_DocumentProduct", "Document");
            HasKey(t => new { t.DocumentId, t.ProductId });

            HasOne(t => t.Document).WithMany(t => t.DocumentProducts).HasForeignKey(t => t.DocumentId).OnDelete(DeleteBehavior.Cascade);
            HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}