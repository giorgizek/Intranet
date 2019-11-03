using System;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zek.Data;
using Zek.Data.Filtering;
using Zek.Extensions;
using Zek.Linq;
using Zek.Localization;
using Zek.Model.Accounting;
using Zek.Model.Production;
using Zek.Model.ViewModel.Production;
using Zek.PagedList;
using Zek.Utils;
using Zek.Web;
using Zek.Extensions.Collections;

namespace Intranet.Web.Controllers
{
    public class AdminProductsController : UowController
    {
        private readonly IIntranetCacheService _cache;

        public AdminProductsController(
            IIntranetCacheService cache,
            IIntranetUnitOfWork uow) : base(uow)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index(ProductFilterViewModel model = null)
        {
            if (model == null)
                model = new ProductFilterViewModel();


            var query = Uow.Products.GetAll().Select(p => new ProductsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                ProductNumber = p.ProductNumber,
                CategoryId = p.CategoryId,

                IsDeleted = p.IsDeleted
            });

            query.Filter(q => q.Id, WhereOperator.Equals, model.Id);
            query = query.Filter(q => q.ProductNumber, WhereOperator.Contains, model.ProductNumber);
            query = query.Filter(q => q.Name, WhereOperator.Contains, model.Name);

            var categories = _cache.GetAllCategory(1);
            model.PagedList = await query.ToPagedListAsync(model.Page, model.PageSize);
            foreach (var product in model.PagedList)
            {
                if (product.CategoryId != null)
                {
                    product.Category = categories.TryGetValue(product.CategoryId.Value);
                }
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Grid", model);

            Title = ProductResources.Products;
            //BindControls(model);


            return View(model);
        }


        public IActionResult Create(string returnUrl = null)
        {
            var model = new ProductViewModel
            {
                
            };
            BindControls(model);

            Title = ProductResources.Product;
            ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model, string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                if (await Uow.Products.Where(p => p.ProductNumber == model.ProductNumber).AnyAsync())
                    ModelState.AddModelError(nameof(ProductViewModel.ProductNumber), ValidationResources.DuplicatedValidationError);

                if (await Uow.Products.Where(p => p.Name == model.Name).AnyAsync())
                    ModelState.AddModelError(nameof(ProductViewModel.Name), ValidationResources.DuplicatedValidationError);
            }

            if (!ModelState.IsValid)
            {
                BindControls(model);
                Title = ProductResources.ProductNumber;
                ReturnUrl = returnUrl;
                return View(model);
            }



            var product = new Product
            {
                Name = model.Name,
                ProductNumber = model.ProductNumber,
                CategoryId = ArrayHelper.Coalesce(model.SubCategoryId, model.CategoryId),
                CurrencyId = ISO4217.GEL.ToInt32(),
                CreatorId = UserId
            };

            Uow.Products.Add(product);
            await Uow.SaveAsync();

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction(nameof(Index));
        }


        private void BindControls(ProductViewModel model)
        {
            model.Categories = _cache.GetCategorySelectList(null, 1);
            if (model.CategoryId != null)
                model.SubCategories = _cache.GetCategorySelectList(model.CategoryId, 1);
        }

        public IActionResult GetSubCategories(int id)
        {
            return Ok(_cache.GetCategory(id, 1));
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var product = await Uow.Products.Where(p => p.Id == id && !p.IsDeleted).SingleOrDefaultAsync();
            if (product == null)
                return NotFound();

            product.IsDeleted = true;
            product.ModifiedDate = DateTime.Now;
            product.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return NotFound();

            var product = await Uow.Products.Where(p => p.Id == id && p.IsDeleted).SingleOrDefaultAsync();
            if (product == null)
                return NotFound();

            product.IsDeleted = false;
            product.ModifiedDate = DateTime.Now;
            product.ModifierId = UserId;
            await Uow.SaveAsync();

            return Ok();
        }
    }
}
