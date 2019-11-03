using Microsoft.EntityFrameworkCore;
using Zek.Model.Base;

namespace Intranet.Model.Dictionary
{
    public class Branch : BaseModel<int>
    {
    }

    public class BranchMap : BaseModelMap<Branch, int>
    {
        public BranchMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DD_Branch", "Dictionary");
        }
    }

    public class BranchTranslate : TranslateModel<Branch, int>
    {
    }

    public class BranchTranslateMap : TranslateModelMap<BranchTranslate, Branch, int>
    {
        public BranchTranslateMap(ModelBuilder builder) : base(builder)
        {
            ToTable("DT_Branch", "Translate");
        }
    }
}
