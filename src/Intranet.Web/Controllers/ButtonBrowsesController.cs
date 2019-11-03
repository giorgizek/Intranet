using System;
using System.Threading.Tasks;
using Intranet.Data;
using Microsoft.AspNetCore.Mvc;
using Zek.Data;
using Zek.Model.ViewModel;
using Zek.Web;

namespace Intranet.Web.Controllers
{
    [AuthorizeEx]
    public class ButtonBrowsesController : UowController
    {
        public ButtonBrowsesController(IIntranetUnitOfWork uow) : base(uow)
        {
        }


        [Ajax]
        public IActionResult Popup(Constants.Controllers partial, string parameters)
        {
            switch (partial)
            {
                case Constants.Controllers.Contracts:
                case Constants.Controllers.VehicleModels:
                case Constants.Controllers.PolicyHolders:
                    return PartialView("_Modal", new ButtonBrowseViewModel { Controller = partial, Parameters = parameters });
            }

            ModelState.AddModelError(string.Empty, $"{partial} partial not found");
            return BadRequest(ModelState);
        }



       /* [Ajax]
        public async Task<IActionResult> Contracts(CorporateContractPopupFilterViewModel model = null)
        {
            if (model == null)
                model = new CorporateContractPopupFilterViewModel();



            var query = from c in Uow2.Contracts.Where(c => c.ClientID == CompanyId)
                orderby c.ID descending
                select new ContractBasePopupViewModel
                {
                    Id = c.ID,
                    ContractNumber = c.ContractNumber,

                };
            query = query.Filter(h => h.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(h => h.ContractNumber, WhereOperator.Contains, model.ContractNumber);




            if (!model.AllContract)
                query = query.Where(c => c.FromDate <= DateTime.Today && c.ToDate >= DateTime.Today);


            model.PagedList = await query.ToPagedListAsync(model.Page, model.PageSize);

            var viewName = Request.IsPost() ? $"_{nameof(Contracts)}Grid" : $"_{nameof(Contracts)}";
            return PartialView(viewName, model);
        }

        [Ajax]
        public async Task<IActionResult> ContractsText(int id)
        {
            if (id <= 0)
                return NotFound();

            var text = await Uow2.Contracts.Where(c => c.ID == id && c.ClientID == CompanyId && c.IsApproved && !c.IsDeleted).Select(c => c.ContractNumber).FirstOrDefaultAsync();
            if (text == null)
                return NotFound();

            return Ok(text);
        }*/

    }
}
