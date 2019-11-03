using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.Config;
using Intranet.Model.Document;
using Intranet.Model.ViewModel.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Zek.Data;
using Zek.Linq;
using Zek.PagedList;
using Zek.Web;
using DocumentResources = Intranet.Localization.DocumentResources;
using Microsoft.EntityFrameworkCore;
using Zek.Data.Filtering;
using Zek.Extensions.Collections;

namespace Intranet.Web.Controllers
{
    [AuthorizeEx(Constants.Roles.IT)]
    public class AdminDocumentTemplatesController : UowController
    {
        private readonly IntranetOptions _options;
        private readonly IIntranetCacheService _cache;

        public AdminDocumentTemplatesController(
            IOptions<IntranetOptions> optionsAccessor,
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _options = optionsAccessor.Value;
            _cache = cache;
        }

        private async Task BindControlsAsync(DocumentTemplateViewModel model)
        {
            if (model == null)
                model = new DocumentTemplateViewModel();

            model.Categories = _cache.GetCategory(1, 1);

            if (model.Products == null)
                model.Products = new List<DocumentTemplateProductViewModel>();


            var products = await Uow.Products
                .Where(p => !p.IsDeleted)
                .Select(p => new DocumentTemplateProductViewModel
                {
                    Id = p.Id,
                    ProductNumber = p.ProductNumber,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
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


        public async Task<IActionResult> Index(DocumentTemplateFilterViewModel model = null)
        {
            if (model == null)
                model = new DocumentTemplateFilterViewModel();

            var query = Uow.DocumentTemplates.GetAll().OrderByDescending(e => e.Id)
                .Select(t => new DocumentTemplatesViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsDeleted = t.IsDeleted
                });

            query = query.Filter(q => q.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(q => q.Name, WhereOperator.Contains, model.Name);

            model.PagedList = await query.ToPagedListAsync(model.Page, model.PageSize);

            if (Request.IsAjaxRequest())
                return PartialView("_Grid", model);

            Title = DocumentResources.DocumentTemplates;
            return View(model);
        }

        public async Task<IActionResult> Create(string returnUrl = null)
        {
            var model = new DocumentTemplateViewModel
            {
                Categories = _cache.GetCategory(1, 1),
                Products = await Uow.Products
                .Where(p => !p.IsDeleted)
                //.OrderBy(p => p.CategoryId)
                //.ThenBy(p => p.Name)
                .Select(p => new DocumentTemplateProductViewModel
                {
                    Id = p.Id,
                    ProductNumber = p.ProductNumber,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                }).ToListAsync()
            };
            await BindControlsAsync(model);

            Title = DocumentResources.DocumentTemplate;
            ReturnUrl = returnUrl;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentTemplateViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControlsAsync(model);
                Title = DocumentResources.DocumentTemplate;
                ReturnUrl = returnUrl;
                return View(model);
            }

            var template = new DocumentTemplate
            {
                Name = model.Name,
                CreatorId = UserId,
                DocumentTemplateProducts = model.Products.Where(p => p.Checked).Select(m => new DocumentTemplateProduct
                {
                    ProductId = m.Id
                }).ToList()
            };

            Uow.DocumentTemplates.Add(template);
            await Uow.SaveAsync();

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var model = await Uow.DocumentTemplates.Where(t => t.Id == id).Select(t => new DocumentTemplateViewModel
            {
                Id = id,
                Name = t.Name,
            }).SingleOrDefaultAsync();
            if (model == null)
                return NotFound();



            model.Products = await Uow.DocumentTemplateProducts
                .Where(t => t.DocumentTemplateId == id && !t.Product.IsDeleted)
                .Select(t => new DocumentTemplateProductViewModel
                {
                    Id = t.ProductId,
                    ProductNumber = t.Product.ProductNumber,
                    CategoryId = t.Product.CategoryId,
                    Name = t.Product.Name,
                    Checked = true
                }).ToListAsync();

            await BindControlsAsync(model);
            Title = DocumentResources.DocumentTemplate;
            ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentTemplateViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                await BindControlsAsync(model);
                Title = DocumentResources.DocumentTemplate;
                ReturnUrl = returnUrl;
                return View(model);
            }


            var template = await Uow.DocumentTemplates.Where(t => t.Id == model.Id && !t.IsDeleted).SingleOrDefaultAsync();
            if (template == null)
                return NotFound();

            template.Name = model.Name;
            template.ModifierId = UserId;
            template.ModifiedDate = DateTime.Now;


            var dbProducts = await Uow.DocumentTemplateProducts.Where(p => p.DocumentTemplateId == model.Id).ToListAsync();
            var dbIds = new HashSet<int>(dbProducts.Select(t => t.ProductId));
            var checkedProducts = model.Products.Where(t => t.Checked).ToList();
            var checkedIds = new HashSet<int>(checkedProducts.Select(t => t.Id));
            var toBeDeletedProducts = dbProducts.Where(t => !checkedIds.Contains(t.ProductId)).ToList();
            Uow.DocumentTemplateProducts.RemoveRange(toBeDeletedProducts);

            Uow.DocumentTemplateProducts.AddRange(checkedProducts.Where(t => !dbIds.Contains(t.Id))
                .Select(t => new DocumentTemplateProduct
                {
                    DocumentTemplateId = template.Id,
                    ProductId = t.Id
                }));


            await Uow.SaveAsync();


            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }
    }
}
