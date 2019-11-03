using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.ViewModel.HR;
using Microsoft.AspNetCore.Mvc;
using Zek.Data;
using Zek.Data.Filtering;
using Zek.Linq;
using Zek.Localization;
using Zek.PagedList;
using Zek.Web;
using Zek.Extensions.Collections;
using Zek.Model.ViewModel.Contact;

namespace Intranet.Web.Controllers
{
    public class EmployeesController : UowController
    {
        private readonly IIntranetCacheService _cache;

        public EmployeesController(
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index(EmployeeFilterViewModel model = null)
        {
            if (model == null)
                model = new EmployeeFilterViewModel();

            var query = Uow.Employees.GetAll().OrderByDescending(e => e.Id)
                .Select(e => new EmployeesViewModel
                {
                    Id = e.Id,
                    BranchId = e.BranchId,
                    JobTitleId = e.JobTitleId,
                    DepartmentId = e.DepartmentId,
                    ImageUrl = e.Image,

                    FullName = e.Person.FullName,
                    PersonalNumber = e.Person.PersonalNumber,
                    BirthDate = e.Person.BirthDate,
                    GenderId = e.Person.GenderId,
                    Branch = e.Person.Contact.Phone1,

                    Contact = new ContactBaseViewModel
                    {
                        Id = e.Person.ContactId,
                        Phone1 = e.Person.Contact.Phone1,
                        Mobile1 = e.Person.Contact.Mobile1,
                        Email1 = e.Person.Contact.Email1
                    },

                    IsDeleted = e.Person.IsDeleted,
                });

            query = query.Filter(q => q.IsDeleted, WhereOperator.Equals, false);
            query = query.Filter(q => q.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(q => q.FullName, WhereOperator.Contains, model.FullName);
            query = query.Filter(q => q.Contact.Mobile1, WhereOperator.Contains, model.Mobile);
            query = query.Filter(q => q.Contact.Phone1, WhereOperator.Contains, model.Phone);


            query = query.Filter(q => q.JobTitleId, WhereOperator.Equals, model.JobTitleId);
            query = query.Filter(q => q.BranchId, WhereOperator.Equals, model.BranchId);
            query = query.Filter(q => q.DepartmentId, WhereOperator.Equals, model.DepartmentId);

            model.PagedList = await query.ToPagedListAsync(model.Page, model.PageSize);
            var jobTitles = _cache.GetJobTitle(null, 1);
            var departments = _cache.GetDepartment(1);
            var branches = _cache.GetBranch(1);
            foreach (var row in model.PagedList)
            {
                if (row.JobTitleId != null)
                    row.JobTitle = jobTitles.TryGetValue(row.JobTitleId.Value);
                if (row.DepartmentId != null)
                    row.Department = departments.TryGetValue(row.DepartmentId.Value);

                if (row.BranchId != null)
                    row.Branch = branches.TryGetValue(row.BranchId.Value);
            }


            if (Request.IsAjaxRequest())
                return PartialView("_Grid", model);

            BindControls(model);
            Title = HrResources.Employees;

            return View(model);
        }


        private void BindControls(EmployeeFilterViewModel model)
        {
            if (model == null)
                model = new EmployeeFilterViewModel();

            model.Departments = _cache.GetDepartmentSelectList(1);
            model.JobTitles = _cache.GetJobTitleSelectList(null, 1);
            model.Branchs = _cache.GetBranchSelectList(1);
        }
        public IActionResult GetJobTitlesByDepartmentId(int? id)
        {
            return Ok(_cache.GetJobTitle(id, 1));
        }
    }
}