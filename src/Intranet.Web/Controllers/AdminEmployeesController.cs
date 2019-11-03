using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.Config;
using Intranet.Model.HR;
using Intranet.Model.ViewModel.HR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Zek.Data;
using Zek.Data.Filtering;
using Zek.Extensions;
using Zek.Linq;
using Zek.Localization;
using Zek.Model.Contact;
using Zek.Model.Person;
using Zek.Model.ViewModel;
using Zek.Model.ViewModel.Contact;
using Zek.PagedList;
using Zek.Utils;
using Zek.Web;
using Zek.Extensions.Collections;

namespace Intranet.Web.Controllers
{
    [AuthorizeEx(Constants.Roles.Admin, Constants.Roles.IT, Constants.Roles.HR)]
    public class AdminEmployeesController : UowController
    {
        private readonly IntranetOptions _options;
        private readonly IHostingEnvironment _environment;
        private readonly IIntranetCacheService _cache;

        public AdminEmployeesController(
            IOptions<IntranetOptions> optionsAccessor,
            IHostingEnvironment environment,
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _options = optionsAccessor.Value;
            _environment = environment;
            _cache = cache;
        }

        public IActionResult GetJobTitlesByDepartmentId(int? id)
        {
            return Ok(_cache.GetJobTitle(id, 1));
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

                    FullName = e.Person.FullName,
                    PersonalNumber = e.Person.PersonalNumber,
                    BirthDate = e.Person.BirthDate,
                    GenderId = e.Person.GenderId,
                    IsDeleted = e.Person.IsDeleted
                });

            query = query.Filter(q => q.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(q => q.PersonalNumber, WhereOperator.Equals, model.PersonalNumber);
            query = query.Filter(q => q.FullName, WhereOperator.Contains, model.FullName);


            query = query.Filter(q => q.JobTitleId, WhereOperator.Equals, model.JobTitleId);
            query = query.Filter(q => q.BranchId, WhereOperator.Equals, model.BranchId);

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

            Title = HrResources.Employees;
            BindControls(model);


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
        private void BindControls(EmployeeViewModel model)
        {
            if (model == null)
                model = new EmployeeViewModel();

            if (model.Address == null)
                model.Address = new AddressViewModel();

            //model.Address.Countries = _cache.GetCountrySelectList(CurrentCultureId);
            //model.Address.Cities = _cache.GetCitiesSelectList(model.Address.CountryId, CurrentCultureId);



            model.Departments = _cache.GetDepartmentSelectList(1);
            if (model.DepartmentId != null)
                model.JobTitles = _cache.GetJobTitleSelectList(model.DepartmentId, 1);


            model.Branchs = _cache.GetBranchSelectList(1);
        }

        public IActionResult Create(string returnUrl = null)
        {
            var model = new EmployeeViewModel
            {
                Address = new AddressViewModel
                {
                    CountryId = _options.CountryId,
                    City = _options.City
                }
            };
            BindControls(model);

            Title = HrResources.Employee;
            ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                BindControls(model);
                Title = HrResources.Employee;
                ReturnUrl = returnUrl;
                return View(model);
            }


            var person = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                FirstNameEn = string.Empty,
                LastNameEn = string.Empty,
                PersonalNumber = model.PersonalNumber.IfNullEmpty(),
                Passport = model.Passport.IfNullEmpty(),
                BirthDate = model.BirthDate,
                GenderId = model.GenderId,//.NullIfDefault(Gender.None),
                CreatorId = UserId,

                Address = new Address
                {
                    City = model.Address.City,
                    Street = model.Address.Street,
                    HouseNumber = model.Address.HouseNumber,
                    PostalCode = model.Address.PostalCode,
                },
                Contact = new Contact
                {
                    Phone1 = model.Contact.Phone1,
                    Phone2 = model.Contact.Phone2,
                    Phone3 = model.Contact.Phone3,
                    Fax1 = model.Contact.Fax1,
                    Fax2 = model.Contact.Fax2,
                    Fax3 = model.Contact.Fax3,
                    Mobile1 = model.Contact.Mobile1,
                    Mobile2 = model.Contact.Mobile2,
                    Mobile3 = model.Contact.Mobile3,
                    Email1 = model.Contact.Email1,
                    Email2 = model.Contact.Email2,
                    Email3 = model.Contact.Email3,
                    Url = model.Contact.Url,
                }
            };


            var employee = new Employee
            {
                DepartmentId = model.DepartmentId,
                JobTitleId = model.JobTitleId,
                BranchId = model.BranchId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,

                Person = person
            };

            Uow.Employees.Add(employee);
            await Uow.SaveAsync();
            await ImageUploadAsync(model.Image, employee);

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var employee = await Uow.Employees.Where(t => t.Id == id).SingleOrDefaultAsync();
            if (employee == null)
                return NotFound();

            var person = await Uow.Persons.Where(t => t.Id == id).SingleAsync();

            var address = await Uow.Addresses.Where(t => t.Id == person.AddressId).SingleAsync();
            //var countryId = await Uow.Cities.Where(t => t.Id == address.CityId).Select(t => t.CountryId).SingleAsync();
            var contact = await Uow.Contacts.Where(t => t.Id == person.ContactId).SingleAsync();

            var model = new EmployeeViewModel
            {
                Id = id,

                JobTitleId = employee.JobTitleId,
                StartDate = employee.StartDate,
                EndDate = employee.EndDate,
                BranchId = employee.BranchId,
                DepartmentId = employee.DepartmentId,
                ImageUrl = employee.Image,

                FirstName = person.FirstName,
                LastName = person.LastName,
                FirstNameEn = person.FirstNameEn,
                LastNameEn = person.LastNameEn,
                PersonalNumber = person.PersonalNumber,
                Passport = person.Passport,
                GenderId = person.GenderId,
                BirthDate = person.BirthDate,
                IsDeleted = person.IsDeleted,

                Address = new AddressViewModel
                {
                    Id = address.Id,
                    City = address.City,
                    CountryId = address.CountryId,
                    Street = address.Street,
                    HouseNumber = address.HouseNumber,
                    PostalCode = address.PostalCode
                },
                Contact = new ContactViewModel
                {
                    Id = contact.Id,
                    Phone1 = contact.Phone1,
                    Phone2 = contact.Phone2,
                    Phone3 = contact.Phone3,
                    Fax1 = contact.Fax1,
                    Fax2 = contact.Fax2,
                    Fax3 = contact.Fax3,
                    Mobile1 = contact.Mobile1,
                    Mobile2 = contact.Mobile2,
                    Mobile3 = contact.Mobile3,
                    Email1 = contact.Email1,
                    Email2 = contact.Email2,
                    Email3 = contact.Email3,
                    Url = contact.Url
                }
            };
            

            BindControls(model);
            Title = HrResources.Employee;
            ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                BindControls(model);
                Title = HrResources.Employee;
                ReturnUrl = returnUrl;
                return View(model);
            }


            var employee = await Uow.Employees.Where(h => h.Id == model.Id).SingleOrDefaultAsync();
            if (employee == null)
                return NotFound();

            employee.DepartmentId = model.DepartmentId;
            employee.JobTitleId = model.JobTitleId;
            employee.BranchId = model.BranchId;
            employee.StartDate = model.StartDate;
            employee.EndDate = model.EndDate;

            var person = await Uow.Persons.Where(h => h.Id == model.Id)
                .Include(h => h.Address)
                .Include(h => h.Contact)
                .SingleOrDefaultAsync();

            person.ModifierId = UserId;
            person.ModifiedDate = DateTime.Now;

            person.FirstName = model.FirstName;
            person.LastName = model.LastName;
            person.FirstNameEn = string.Empty;
            person.LastNameEn = string.Empty;
            person.PersonalNumber = model.PersonalNumber.IfNullEmpty();
            person.Passport = model.Passport.IfNullEmpty();
            person.BirthDate = model.BirthDate;
            person.GenderId = model.GenderId;



            if (model.Address != null)
            {
                person.Address.City = model.Address.City;
                person.Address.Street = model.Address.Street;
                person.Address.HouseNumber = model.Address.HouseNumber;
                person.Address.PostalCode = model.Address.PostalCode;
            }


            if (model.Contact != null)
            {
                person.Contact.Phone1 = model.Contact.Phone1;
                person.Contact.Phone2 = model.Contact.Phone2;
                person.Contact.Phone3 = model.Contact.Phone3;
                person.Contact.Fax1 = model.Contact.Fax1;
                person.Contact.Fax2 = model.Contact.Fax2;
                person.Contact.Fax3 = model.Contact.Fax3;
                person.Contact.Mobile1 = model.Contact.Mobile1;
                person.Contact.Mobile2 = model.Contact.Mobile2;
                person.Contact.Mobile3 = model.Contact.Mobile3;
                person.Contact.Email1 = model.Contact.Email1;
                person.Contact.Email2 = model.Contact.Email2;
                person.Contact.Email3 = model.Contact.Email3;
                person.Contact.Url = model.Contact.Url;
            }
            await Uow.SaveAsync();
            if (model.ImageDelete && !string.IsNullOrEmpty(employee.Image))
            {
                var extension = Path.GetExtension(employee.Image);
                var file = $"{employee.Id}{extension}";
                var path = Path.Combine(_environment.WebRootPath, $@"images\employees\{file}");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    employee.Image = null;
                    await Uow.SaveAsync();
                }
            }
            await ImageUploadAsync(model.Image, employee);

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var person = await Uow.Persons.Where(p => p.Id == id && !p.IsDeleted).SingleOrDefaultAsync();
            if (person == null)
                return NotFound();

            person.IsDeleted = true;
            person.ModifiedDate = DateTime.Now;
            person.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return NotFound();

            var person = await Uow.Persons.Where(p => p.Id == id && p.IsDeleted).SingleOrDefaultAsync();
            if (person == null)
                return NotFound();

            person.IsDeleted = false;
            person.ModifiedDate = DateTime.Now;
            person.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

        private async Task ImageUploadAsync(IFormFile image, Employee employee)
        {
            if (image?.Length > 0 && _options.ImageContentTypes.Contains(image.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileName(image.FileName);

                if (fileName != null)
                {
                    var extension = Path.GetExtension(image.FileName);
                    var file = $"{employee.Id}{extension}";
                    var path = Path.Combine(_environment.WebRootPath, $@"images\employees\{file}");
                    FileHelper.CreateDirectoryIfNotExists(path);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    employee.Image = file;
                    await Uow.SaveAsync();
                }
            }
        }
    }
}
