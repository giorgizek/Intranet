using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zek.Model.ViewModel;
using Zek.Utils;
using Zek.Web;
using Intranet.Data;

namespace Intranet.Web.Controllers
{
    [Authorize]
    public class InfosController : UowController
    {
        public InfosController(
            IIntranetUnitOfWork uow
        ) : base(uow)
        {
        }

        [Ajax]
        public async Task<IActionResult> Index(int id, string name)
        {
            if (id <= 0 || string.IsNullOrEmpty(name))
                return NoContent();

            var isAdmin = true;//User.IsInRole(RoleOptions.AdminRoleName);


            InfoViewModel model;
            switch (name)
            {
                /*case "Accounts":
                    if (!isAdmin)
                    {
                        var personId = await Uow.Accounts.Where(r => r.Id == id).Select(r => r.PersonId).SingleOrDefaultAsync();
                        if (personId == 0)
                            return NotFound();
                        var hotelId = await Uow.Hotels.Where(h => h.PersonId == personId).Select(h => h.Id).FirstOrDefaultAsync();
                        if (hotelId == 0)
                            return NotFound();

                        if (!_cache.UserHotelAny(UserId, hotelId))
                            return NotFound();
                    }

                    model = await Uow.Accounts.Where(r => r.Id == id).Select(r => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = r.CreatorId },
                        CreateDate = r.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = r.ModifierId },
                        ModifiedDate = r.ModifiedDate,
                        IsDeleted = r.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;

                case "Reservations":
                    if (!isAdmin)
                    {
                        var hotelId = await Uow.Reservations.Where(r => r.Id == id).Select(r => r.HotelId).SingleOrDefaultAsync();
                        if (!_cache.UserHotelAny(UserId, hotelId))
                            return NotFound();
                    }

                    model = await Uow.Reservations.Where(r => r.Id == id).Select(r => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = r.CreatorId },
                        CreateDate = r.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = r.ModifierId },
                        ModifiedDate = r.ModifiedDate,
                        IsDeleted = r.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;

                case "Transactions":
                    if (!isAdmin)
                    {
                        var reservationId = await Uow.ReservationTransactions.Where(r => r.TransactionId == id).Select(r => r.ReservationId).SingleOrDefaultAsync();
                        if (reservationId == 0)
                            return NotFound();

                        var hotelId = await Uow.Reservations.Where(r => r.Id == reservationId).Select(r => r.HotelId).SingleOrDefaultAsync();
                        if (!_cache.UserHotelAny(UserId, hotelId))
                            return NotFound();
                    }
                    model = await Uow.Transactions.Where(r => r.Id == id).Select(r => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = r.CreatorId },
                        CreateDate = r.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = r.ModifierId },
                        ModifiedDate = r.ModifiedDate,
                        IsDeleted = r.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;

                case "Inclusions":
                    if (!isAdmin)
                    {
                        var hotelId = await Uow.Inclusions.Where(r => r.Id == id).Select(r => r.HotelId).SingleOrDefaultAsync();
                        if (!_cache.UserHotelAny(UserId, hotelId))
                            return NotFound();
                    }
                    model = await Uow.Inclusions.Where(r => r.Id == id).Select(r => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = r.CreatorId },
                        CreateDate = r.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = r.ModifierId },
                        ModifiedDate = r.ModifiedDate,
                        IsDeleted = r.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;


                case "RateTypes":
                    if (!isAdmin)
                    {
                        var hotelId = await Uow.RateTypes.Where(r => r.Id == id).Select(r => r.HotelId).SingleOrDefaultAsync();
                        if (!_cache.UserHotelAny(UserId, hotelId))
                            return NotFound();
                    }
                    model = await Uow.RateTypes.Where(r => r.Id == id).Select(r => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = r.CreatorId },
                        CreateDate = r.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = r.ModifierId },
                        ModifiedDate = r.ModifiedDate,
                        IsDeleted = r.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;*/
                case "AdminJobTitles":
                    if (!isAdmin)
                        return NotFound();

                    model = await Uow.JobTitles.Where(o => o.Id == id).Select(o => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = o.CreatorId },
                        CreateDate = o.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = o.ModifierId },
                        ModifiedDate = o.ModifiedDate,
                        IsDeleted = o.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;

                case "AdminEmployees":
                    if (!isAdmin)
                        return NotFound();

                    model = await Uow.Persons.Where(o => o.Id == id).Select(o => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = o.CreatorId },
                        CreateDate = o.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = o.ModifierId },
                        ModifiedDate = o.ModifiedDate,
                        IsDeleted = o.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;


                case "Documents":
                    if (!isAdmin)
                        return NotFound();

                    model = await Uow.Documents.Where(o => o.Id == id).Select(o => new InfoViewModel
                    {
                        Creator = new KeyPair<int?, string> { Key = o.CreatorId },
                        CreateDate = o.CreateDate,
                        Modifier = new KeyPair<int?, string> { Key = o.ModifierId },
                        ModifiedDate = o.ModifiedDate,
                        IsDeleted = o.IsDeleted,
                    }).SingleOrDefaultAsync();
                    break;


                default:
                    ModelState.AddModelError(nameof(name), name + " info tab not implemented.");
                    return BadRequest(ModelState);
            }

            if (model == null)
                return NotFound();

            model.Id = id;

            if (model.Creator?.Key != null)
                model.Creator.Value = await Uow.Users.Where(c => c.Id == model.Creator.Key).Select(c => c.UserName).SingleOrDefaultAsync();
            if (model.Modifier?.Key != null)
                model.Modifier.Value = await Uow.Users.Where(c => c.Id == model.Modifier.Key).Select(c => c.UserName).SingleOrDefaultAsync();
            if (model.Approver?.Key != null)
                model.Approver.Value = await Uow.Users.Where(c => c.Id == model.Approver.Key).Select(c => c.UserName).SingleOrDefaultAsync();

            return PartialView("EditorTemplates/InfoViewModel", model);
        }
    }
}
