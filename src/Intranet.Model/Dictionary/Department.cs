using Microsoft.EntityFrameworkCore;
using Zek.Model.Base;

namespace Intranet.Model.Dictionary
{
    public class Department : BaseModel<int>
    {
    }

    public class DepartmentMap : BaseModelMap<Department, int>
    {
        public DepartmentMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DD_Department", "Dictionary");
        }
    }

    public class DepartmentTranslate : TranslateModel<Department, int>
    {
    }

    public class DepartmentTranslateMap : TranslateModelMap<DepartmentTranslate, Department, int>
    {
        public DepartmentTranslateMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DT_Department", "Translate");
        }
    }
}
