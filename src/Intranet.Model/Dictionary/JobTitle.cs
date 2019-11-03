using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zek.Model.Base;

namespace Intranet.Model.Dictionary
{
    public class JobTitle : BaseModel<int>
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }

    public class JobTitleMap : BaseModelMap<JobTitle, int>
    {
        public JobTitleMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DD_JobTitle", "Dictionary");
            HasOne(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }


    public class JobTitleTranslate : TranslateModel<JobTitle, int>
    {
    }

    public class JobTitleTranslateMap : TranslateModelMap<JobTitleTranslate, JobTitle, int>
    {
        public JobTitleTranslateMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DT_JobTitle", "Translate");
        }
    }
}
