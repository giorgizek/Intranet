using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.Document;
using Intranet.Model.ViewModel.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zek.Data;
using Zek.Data.Filtering;
using Zek.Extensions.Collections;
using Zek.Extensions.Security.Claims;
using Zek.Linq;
using Zek.PagedList;
using Zek.Web;

namespace Intranet.Web.Controllers
{
    [AuthorizeEx]
    public class DocumentsController : UowController
    {
        private readonly IIntranetCacheService _cache;

        public DocumentsController(
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _cache = cache;
        }


        public async Task<IActionResult> Index(int templateId, DocumentFilterViewModel model = null)
        {
            if (templateId <= 0)
                return NotFound();


            if (model == null)
                model = new DocumentFilterViewModel();

            model.TemplateId = templateId;

            var query = from d in Uow.Documents.GetAll()
                        join e in Uow.Employees.GetAll() on d.CreatorId equals e.Id//todo user and employee mapping table and auto initialize
                        select new DocumentsViewModel
                        {
                            Id = d.Id,
                            DepartmentId = d.DepartmentId,
                            JobTitleId = d.JobTitleId,
                            

                            CreateDate = d.CreateDate,
                            CreatorId = d.CreatorId,
                            IsDeleted = d.IsDeleted,

                            IsApproved = d.IsApproved
                            
                        };


            query = query.Filter(q => q.Id, WhereOperator.Equals, model.Id);

            if (!User.IsInAnyRole(Constants.Roles.Admin, Constants.Roles.IT, Constants.Roles.Administration))
            {
                query = query.Filter(q => q.CreatorId, WhereOperator.Equals, UserId);
            }


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

                //if (row.BranchId != null)
                //    row.Branch = branches.TryGetValue(row.BranchId.Value);
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Grid", model);

            ReturnUrl = Url.Action(nameof(Index), "Documents", new { templateId });
            Title = await Uow.DocumentTemplates.Where(t => t.Id == templateId && !t.IsDeleted).Select(t => t.Name).SingleOrDefaultAsync();
            return View(model);
        }




        private async Task BindControlsAsync(DocumentViewModel model)
        {
            model.Categories = _cache.GetCategory(1, 1);


            if (model.Products == null)
                model.Products = new List<DocumentProductsViewModel>();

            //model.Products = await Uow.DocumentTemplateProducts
            //    .Where(t => t.DocumentTemplateId == templateId && !t.Product.IsDeleted)
            //    .Select(t => new DocumentProductsViewModel
            //    {
            //        Id = t.ProductId,
            //        ProductNumber = t.Product.ProductNumber,
            //        Name = t.Product.Name,
            //        CategoryId = t.Product.CategoryId,
            //    }).ToListAsync();


            var products = await Uow.DocumentTemplateProducts
                .Where(t => t.DocumentTemplateId == model.TemplateId && !t.Product.IsDeleted)
                .Select(t => new DocumentProductsViewModel
                {
                    Id = t.ProductId,
                    ProductNumber = t.Product.ProductNumber,
                    Name = t.Product.Name,
                    CategoryId = t.Product.CategoryId,
                }).ToDictionaryAsync(p => p.Id, p => p);

            if (model.Products.IsNullOrEmpty())
                model.Products = products.Select(p => p.Value).ToList();
            else
            {
                foreach (var product in model.Products)
                {
                    var tmp = products.TryGetValue(product.Id);
                    if (tmp == null)
                        continue;

                    product.ProductNumber = tmp.ProductNumber;
                    product.Name = tmp.Name;
                    product.CategoryId = tmp.CategoryId;

                    products.Remove(tmp.Id);
                }
                model.Products.AddRange(products.Values);
            }


        }


        public async Task<IActionResult> Create(int templateId, string returnUrl = null)
        {
            if (templateId <= 0)
                return NotFound();

            var model = await Uow.DocumentTemplates.Where(t => t.Id == templateId && !t.IsDeleted).Select(
                t => new DocumentViewModel
                {
                    Date = DateTime.Today,
                    TemplateId = templateId,
                }).SingleOrDefaultAsync();

            if (model == null)
                return NotFound();

            await BindControlsAsync(model);
            Title = await Uow.DocumentTemplates.Where(t => t.Id == model.TemplateId).Select(t => t.Name).SingleOrDefaultAsync();
            ReturnUrl = Url.Action(nameof(Index), "Documents", new { templateId });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControlsAsync(model);
                ReturnUrl = returnUrl;
                return View(model);
            }

            var template = await Uow.DocumentTemplates.Where(t => t.Id == model.TemplateId && !t.IsDeleted).Select(t => new { t.Name }).SingleOrDefaultAsync();
            if (template == null)
                return BadRequest();

            var document = new Document
            {
                CreatorId = UserId,
                TemplateId = model.TemplateId.GetValueOrDefault()
            };
            await UpdateEmployeeInfoAsync(document);

            var dbIds = new HashSet<int>(await Uow.DocumentTemplateProducts.Where(t => t.DocumentTemplateId == model.TemplateId && !t.Product.IsDeleted).Select(t => t.ProductId).ToListAsync());

            document.DocumentProducts = model.Products.Where(t => t.Quantity > 0 && dbIds.Contains(t.Id)).Select(t => new DocumentProduct
            {
                ProductId = t.Id,
                Quantity = t.Quantity.GetValueOrDefault()
            }).ToList();


            Uow.Documents.Add(document);
            await Uow.SaveAsync();

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var query = Uow.Documents.Where(t => t.Id == id);
            if (User.IsInAnyRole())
                query = query.Where(t => t.CreatorId == UserId);

            var model = await query.Select(t => new DocumentViewModel
            {
                Id = id,
                Date = t.CreateDate,
                TemplateId = t.TemplateId,

                ReadOnly = t.IsApproved || t.IsDeleted
            }).SingleOrDefaultAsync();
            if (model == null)
                return NotFound();


            model.Products = await Uow.DocumentProducts
                .Where(t => t.DocumentId == id)
                .Select(t => new DocumentProductsViewModel
                {
                    Id = t.ProductId,
                    ProductNumber = t.Product.ProductNumber,
                    CategoryId = t.Product.CategoryId,
                    Name = t.Product.Name,
                    Quantity = t.Quantity
                }).ToListAsync();

            await BindControlsAsync(model);

            Title = await Uow.DocumentTemplates.Where(t => t.Id == model.TemplateId).Select(t => t.Name).SingleOrDefaultAsync();
            ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// Updates jobt title, department, branch by creator
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private async Task UpdateEmployeeInfoAsync(Document document)
        {
            if (document.DepartmentId == null || document.JobTitleId == null || document.BranchId == null)
            {
                var employee = await Uow.Employees.Where(e => e.Id == document.CreatorId).Select(e => new { e.DepartmentId, e.JobTitleId, e.BranchId }).SingleOrDefaultAsync();
                if (employee != null)
                {
                    if (document.DepartmentId == null)
                        document.DepartmentId = employee.DepartmentId;

                    if (document.JobTitleId == null)
                        document.JobTitleId = employee.JobTitleId;

                    if (document.BranchId == null)
                        document.BranchId = employee.BranchId;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControlsAsync(model);
                Title = await Uow.DocumentTemplates.Where(t => t.Id == model.TemplateId).Select(t => t.Name).SingleOrDefaultAsync();
                ReturnUrl = returnUrl;
                return View(model);
            }



            var template = await Uow.DocumentTemplates.Where(t => t.Id == model.TemplateId && !t.IsDeleted).Select(t => new { t.Name }).SingleOrDefaultAsync();
            if (template == null)
                return BadRequest();


            var query = Uow.Documents.Where(t => t.Id == model.Id && !t.IsDeleted);
            if (User.IsInAnyRole())
                query = query.Where(t => t.CreatorId == UserId);
            var document = await query.SingleOrDefaultAsync();
            if (document == null)
                return NotFound();

            await UpdateEmployeeInfoAsync(document);

            document.ModifierId = UserId;
            document.ModifiedDate = DateTime.Now;

            var templateProductIds = new HashSet<int>(await Uow.DocumentTemplateProducts.Where(t => t.DocumentTemplateId == model.TemplateId && !t.Product.IsDeleted).Select(t => t.ProductId).ToListAsync());
            var dbProducts = await Uow.DocumentProducts.Where(t => t.DocumentId == model.Id).ToListAsync();
            var checkedProducts = model.Products.Where(t => t.Quantity > 0 && templateProductIds.Contains(t.Id)).ToList();
            var checkedIds = new HashSet<int>(checkedProducts.Select(t => t.Id));
            var toBeDeletedProducts = dbProducts.Where(t => !checkedIds.Contains(t.ProductId)).ToList();
            Uow.DocumentProducts.RemoveRange(toBeDeletedProducts);
            foreach (var checkedProduct in checkedProducts)
            {
                var product = dbProducts.SingleOrDefault(p => p.ProductId == checkedProduct.Id);
                if (product == null)
                {
                    product = new DocumentProduct
                    {
                        DocumentId = model.Id.GetValueOrDefault(),
                        ProductId = checkedProduct.Id,
                        Quantity = checkedProduct.Quantity.GetValueOrDefault()
                    };
                    Uow.DocumentProducts.Add(product);
                }
                else
                {
                    product.Quantity = checkedProduct.Quantity.GetValueOrDefault();
                }
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

            var document = await Uow.Documents.Where(p => p.Id == id && !p.IsDeleted).SingleOrDefaultAsync();
            if (document == null)
                return NotFound();

            document.IsDeleted = true;
            document.ModifiedDate = DateTime.Now;
            document.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return NotFound();

            var person = await Uow.Documents.Where(p => p.Id == id && p.IsDeleted).SingleOrDefaultAsync();
            if (person == null)
                return NotFound();

            person.IsDeleted = false;
            person.ModifiedDate = DateTime.Now;
            person.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            if (id <= 0)
                return NotFound();

            var document = await Uow.Documents.Where(d => d.Id == id && !d.IsDeleted && !d.IsApproved).SingleOrDefaultAsync();
            if (document == null)
                return NotFound();

            document.IsApproved = true;
            document.ModifiedDate = DateTime.Now;
            document.ApproverId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

    }
}