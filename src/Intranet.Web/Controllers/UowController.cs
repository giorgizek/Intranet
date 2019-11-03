using Intranet.Data;
using Zek.Web;

namespace Intranet.Web.Controllers
{
    //[AuthorizeEx]
    public class UowController : BaseUowController<IIntranetUnitOfWork>
    {
        public UowController(IIntranetUnitOfWork uow) : base(uow)
        {

        }
    }

}
