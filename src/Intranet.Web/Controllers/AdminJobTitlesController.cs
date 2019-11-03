using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.Dictionary;
using Intranet.Model.ViewModel.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zek.Data;
using Zek.Data.Filtering;
using Zek.Linq;
using Zek.Localization;
using Zek.PagedList;
using Zek.Web;
using Zek.Extensions.Collections;
using Zek.Utils;

namespace Intranet.Web.Controllers
{
    [AuthorizeEx(Constants.Roles.Admin, Constants.Roles.IT, Constants.Roles.HR)]
    public class AdminJobTitlesController : UowController
    {
        private readonly IIntranetCacheService _cache;

        public AdminJobTitlesController(
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _cache = cache;
        }

        private void BindControls(JobTitleFilterViewModel model)
        {
            if (model == null)
                model = new JobTitleFilterViewModel();

            model.Departments = _cache.GetDepartmentSelectList(1);
        }
        private async Task BindControls(JobTitleViewModel model)
        {
            if (model == null)
                model = new JobTitleViewModel();


            model.Cultures = _cache.GetCultures();


            model.Departments = _cache.GetDepartmentSelectList(1);
            if (model.Texts == null)
                model.Texts = new List<KeyPair<int, string>>();

            //todo var cultures = await AsyncPagedListExtensions.ToListAsync(Uow.Cultures.GetAll());
            //foreach (var culture in cultures)
            //{
            //    var text = model.Texts.FirstOrDefault(t => t.Key == culture.Id);
            //    if (text == null)
            //    {
            //        text = new KeyPair<byte, string>(culture.Id, null);
            //        model.Texts.Add(text);
            //    }
            //}
        }

        public async Task<IActionResult> Index(JobTitleFilterViewModel model = null)
        {
            if (model == null)
                model = new JobTitleFilterViewModel();

            var query = Uow.JobTitleTranslates.Where(t => t.CultureId == 1).OrderByDescending(e => e.Id)
                .Select(t => new JobTitlesViewModel
                {
                    Id = t.Id,
                    DepartmentId = t.Dictionary.DepartmentId,
                    Name = t.Text,
                    IsDeleted = t.Dictionary.IsDeleted
                });

            query = query.Filter(q => q.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(q => q.Name, WhereOperator.Contains, model.Name);
            query = query.Filter(q => q.DepartmentId, WhereOperator.Equals, model.DepartmentId);


            model.PagedList = await query.ToPagedListAsync(model.Page, model.PageSize);
            var departments = _cache.GetDepartment(1);
            foreach (var row in model.PagedList)
            {
                row.Department = departments.TryGetValue(row.DepartmentId);
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Grid", model);

            Title = HrResources.JobTitles;
            BindControls(model);

            return View(model);
        }


        public async Task<IActionResult> Create(string returnUrl = null)
        {
            var model = new JobTitleViewModel();
            await BindControls(model);

            Title = HrResources.JobTitle;
            ReturnUrl = returnUrl;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobTitleViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControls(model);
                Title = HrResources.JobTitle;
                ReturnUrl = returnUrl;
                return View(model);
            }
            var jobTitle = new JobTitle
            {
                DepartmentId = model.DepartmentId.GetValueOrDefault(),
                CreatorId = UserId
            };



            var translates = new List<JobTitleTranslate>();//  model.Texts.Select(x => );
            for (var i = 0; i < model.Texts.Count; i++)
            {
                var translate = new JobTitleTranslate { Dictionary = jobTitle, CultureId = model.Texts[i].Key, Text = model.Texts[i].Value };
                if (await Uow.JobTitleTranslates.Where(t => t.Text == translate.Text).AnyAsync())
                {
                    ModelState.AddModelError($"{nameof(JobTitleViewModel.Texts)}[{i}].Value", string.Format(ValidationResources.DuplicatedValidationError, "ტექსტი"));
                    continue;
                }

                translates.Add(translate);
            }

            if (!ModelState.IsValid)
            {
                await BindControls(model);
                Title = HrResources.JobTitle;
                ReturnUrl = returnUrl;
                return View(model);
            }

            Uow.JobTitles.Add(jobTitle);
            Uow.JobTitleTranslates.AddRange(translates);
            await Uow.SaveAsync();

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            if (id <= 0)
                return NotFound();

            var model = await Uow.JobTitles.Where(t => t.Id == id).Select(t => new JobTitleViewModel
            {
                Id = id,
                DepartmentId = t.DepartmentId,
                ReadOnly = t.IsDeleted
            }).SingleOrDefaultAsync();
            model.Texts = await Uow.JobTitleTranslates.Where(t => t.Id == id).Select(t => new KeyPair<int, string> { Key = t.CultureId, Value = t.Text }).ToListAsync();


            await BindControls(model);

            Title = HrResources.JobTitle;
            ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobTitleViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControls(model);
                Title = HrResources.JobTitle;
                ReturnUrl = returnUrl;
                return View(model);
            }


            if (model.Id.GetValueOrDefault() <= 0)
                return NotFound();

            var jobTitle = await Uow.JobTitles.Where(t => t.Id == model.Id && !t.IsDeleted).SingleOrDefaultAsync();
            if (jobTitle == null)
                return NotFound();

            jobTitle.DepartmentId = model.DepartmentId.GetValueOrDefault();
            jobTitle.ModifierId = UserId;
            jobTitle.ModifiedDate = DateTime.Now;


            var translates = await Uow.JobTitleTranslates.Where(t => t.Id == model.Id).ToListAsync();
            for (var i = 0; i < model.Texts.Count; i++)
            {
                var translate = new JobTitleTranslate { Id = model.Id.GetValueOrDefault(), CultureId = model.Texts[i].Key, Text = model.Texts[i].Value };
                if (await Uow.JobTitleTranslates.Where(t => t.Text == translate.Text && t.Id != model.Id).AnyAsync())
                {
                    ModelState.AddModelError($"{nameof(JobTitleViewModel.Texts)}[{i}].Value", string.Format(ValidationResources.DuplicatedValidationError, "ტექსტი"));
                    continue;
                }

                var dbTranslate = translates.SingleOrDefault(t => t.CultureId == translate.CultureId);
                if (dbTranslate == null)
                {
                    dbTranslate = translate;
                    Uow.JobTitleTranslates.Add(dbTranslate);
                }
                else
                    dbTranslate.Text = translate.Text;
            }

            await Uow.SaveAsync();

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var person = await Uow.JobTitles.Where(p => p.Id == id && !p.IsDeleted).SingleOrDefaultAsync();
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

            var person = await Uow.JobTitles.Where(p => p.Id == id && p.IsDeleted).SingleOrDefaultAsync();
            if (person == null)
                return NotFound();

            person.IsDeleted = false;
            person.ModifiedDate = DateTime.Now;
            person.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

    }
}