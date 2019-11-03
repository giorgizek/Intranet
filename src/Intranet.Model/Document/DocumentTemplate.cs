using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zek.Data.Entity;
using Zek.Model.Base;
using Zek.Model.Production;

namespace Intranet.Model.Document
{
    public class DocumentTemplate : BaseModel<int>
    {
        public string Name { get; set; }

        public List<DocumentTemplateProduct> DocumentTemplateProducts { get; set; }
    }
    public class DocumentTemplateMap : BaseModelMap<DocumentTemplate, int>
    {
        public DocumentTemplateMap(ModelBuilder builder) : base(builder)
        {
            ToTable("T_DocumentTemplate", "Template");

            Property(t => t.Name).HasMaxLength(255);
            HasIndex(t => t.Name).IsUnique();
        }
    }


    public class DocumentTemplateProduct
    {
        public int DocumentTemplateId { get; set; }
        public DocumentTemplate DocumentTemplate { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }


    public class DocumentTemplateProductMap : EntityTypeMap<DocumentTemplateProduct>
    {
        public DocumentTemplateProductMap(ModelBuilder builder) : base(builder)
        {
            ToTable("T_DocumentTemplateProduct", "Template");
            HasKey(t => new { t.DocumentTemplateId, t.ProductId });

            HasOne(t => t.DocumentTemplate).WithMany(t => t.DocumentTemplateProducts).HasForeignKey(t => t.DocumentTemplateId).OnDelete(DeleteBehavior.Cascade);
            HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }


}
