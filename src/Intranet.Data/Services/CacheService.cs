using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Zek.Services;
using System.Linq;
using Intranet.Model.ViewModel.Employee;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zek.Model.Contact;
using Zek.Extensions.Collections;

namespace Intranet.Data.Services
{
    public interface IIntranetCacheService
    {
        Dictionary<int, string> GetCultures(bool bypassCache = false);

        Dictionary<int, string> GetJobTitle(int? departmentId, byte cultureId, bool bypassCache = false);
        SelectList GetJobTitleSelectList(int? departmentId, byte cultureId, bool bypassCache = false);

        Dictionary<int, string> GetDepartment(byte cultureId, bool bypassCache = false);
        SelectList GetDepartmentSelectList(byte cultureId, bool bypassCache = false);



        Dictionary<int, string> GetCountry(byte cultureId, bool bypassCache = false);
        SelectList GetCountrySelectList(byte cultureId, bool bypassCache = false);

        Dictionary<int, string> GetCitiesByCountryId(int? countryId, byte cultureId, bool bypassCache = false);
        SelectList GetCitiesSelectList(int? countryId, byte cultureId, bool bypassCache = false);

        Dictionary<int, string> GetBranch(byte cultureId, bool bypassCache = false);
        SelectList GetBranchSelectList(byte cultureId, bool bypassCache = false);

        int? GetBirtDayEmployeeCount(bool bypassCache = false);
        List<BirthdayPersonViewModel> GetBirthdays(byte cultureId, bool bypassCache = false);

        Dictionary<int, string> GetAllCategory(byte cultureId, bool bypassCache = false);
        Dictionary<int, string> GetCategory(int? parentId, byte cultureId, bool bypassCache = false);
        SelectList GetCategorySelectList(int? parentId, byte cultureId, bool bypassCache = false);
    }


    public class IntranetCacheService : MemoryCacheService, IIntranetCacheService
    {
        private readonly IIntranetUnitOfWork _uow;

        public IntranetCacheService(
            IIntranetUnitOfWork uow,
            IMemoryCache cache) : base(cache)
        {
            _uow = uow;
        }


        public Dictionary<int, string> GetCultures(bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCultures)}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                //bug data = _uow.Cultures.GetAll()
                //    .Select(t => new { t.Id, t.Name })
                //    .ToDictionary(t => t.Id, t => t.Name);
            }
            return data;
        }

        public Dictionary<int, string> GetJobTitle(int? departmentId, byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetJobTitle)} {nameof(departmentId)}: {departmentId}, {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                if (departmentId == null)
                    data = _uow.JobTitleTranslates
                        .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                        .OrderBy(t => t.Text)
                        .Select(t => new { t.Id, t.Text })
                        .ToDictionary(t => t.Id, t => t.Text);
                else
                    data = _uow.JobTitleTranslates
                        .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted && t.Dictionary.DepartmentId == departmentId)
                        .OrderBy(t => t.Text)
                        .Select(t => new { t.Id, t.Text })
                        .ToDictionary(t => t.Id, t => t.Text);

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public SelectList GetJobTitleSelectList(int? departmentId, byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetJobTitleSelectList)} {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetJobTitle(departmentId, cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }



        public Dictionary<int, string> GetDepartment(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetDepartment)} {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                data = _uow.DepartmentTranslates
                    .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                    .OrderBy(t => t.Text)
                    .Select(t => new { t.Id, t.Text })
                    .ToDictionary(t => t.Id, t => t.Text);

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public SelectList GetDepartmentSelectList(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetDepartmentSelectList)} {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetDepartment(cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }


        public Dictionary<int, string> GetCountry(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCountry)} {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                data = ISO3166.Countries.ToDictionary(d => d.NumericCode, d => d.Name);
                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public SelectList GetCountrySelectList(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCountrySelectList)} {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetCountry(cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }


        public Dictionary<int, string> GetCitiesByCountryId(int? countryId, byte cultureId, bool bypassCache = false)
        {
            if (countryId.GetValueOrDefault() <= 0)
                return null;

            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCitiesByCountryId)} {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                //bug data = _uow.CityTranslates
                //    .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                //    .OrderBy(t => t.Text)
                //    .Select(t => new { t.Id, t.Text })
                //    .ToDictionary(t => t.Id, t => t.Text);

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }
            return data;
        }
        public SelectList GetCitiesSelectList(int? countryId, byte cultureId, bool bypassCache = false)
        {
            if (countryId.GetValueOrDefault() <= 0)
                return null;

            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCitiesSelectList)} {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetCitiesByCountryId(countryId, cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }


        public Dictionary<int, string> GetBranch(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetBranch)} {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                data = _uow.BranchTranslates
                    .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                    .OrderBy(t => t.Text)
                    .Select(t => new { t.Id, t.Text })
                    .ToDictionary(t => t.Id, t => t.Text);

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public SelectList GetBranchSelectList(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetBranchSelectList)} {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetBranch(cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }

        private List<int> GetBirtDayEmployeeIds()
        {
            //return _uow.Employees.Where(e => e.Id > 5 && e.Id < 10)
            //    .Select(p => p.Id).ToList();

            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            return _uow.Employees
                .Where(e => e.Person.BirthDate != null && e.Person.BirthDate.Value.Month == month && e.Person.BirthDate.Value.Day == day && !e.Person.IsDeleted)
                .Select(p => p.Id).ToList();
        }
        public int? GetBirtDayEmployeeCount(bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetBirtDayEmployeeCount)}";
            var data = bypassCache ? null : Get<int?>(cacheKey);
            if (data == null)
            {

                var ids = GetBirtDayEmployeeIds();
                data = ids.Count;

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public List<BirthdayPersonViewModel> GetBirthdays(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetBirthdays)}, {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<List<BirthdayPersonViewModel>>(cacheKey);
            if (data == null)
            {
                var ids = GetBirtDayEmployeeIds();
                data = _uow.Employees.Where(e => ids.Contains(e.Id))
                    .Select(e => new BirthdayPersonViewModel
                    {
                        Id = e.Id,
                        FirstName = e.Person.FirstName,
                        LastName = e.Person.LastName,
                        JobTitleId = e.JobTitleId,
                        ImageUrl = e.Image
                    }).ToList();

                var jobTitles = GetJobTitle(null, cultureId);
                foreach (var person in data)
                {
                    if (person.JobTitleId != null)
                        person.JobTitle = jobTitles.TryGetValue(person.JobTitleId.Value);
                }

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }



        public Dictionary<int, string> GetAllCategory(byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetAllCategory)} {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                data = _uow.CategoryTranslates
                    .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                    .OrderBy(t => t.Text)
                    .Select(t => new { t.Id, t.Text })
                    .ToDictionary(t => t.Id, t => t.Text);
                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public Dictionary<int, string> GetCategory(int? parentId, byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCategory)} {nameof(parentId)}: {parentId}, {nameof(cultureId)}: {cultureId}";
            var data = bypassCache ? null : Get<Dictionary<int, string>>(cacheKey);
            if (data == null)
            {
                //if (parentId == null)
                //    data = _uow.CategoryTranslates
                //        .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted)
                //        .OrderBy(t => t.Text)
                //        .Select(t => new { t.Id, t.Text })
                //        .ToDictionary(t => t.Id, t => t.Text);
                //else
                data = _uow.CategoryTranslates
                    .Where(t => t.CultureId == cultureId && !t.Dictionary.IsDeleted && t.Dictionary.ParentId == parentId)
                    .OrderBy(t => t.Text)
                    .Select(t => new { t.Id, t.Text })
                    .ToDictionary(t => t.Id, t => t.Text);

                Set(cacheKey, data, DateTime.Today.AddDays(1));
            }

            return data;
        }
        public SelectList GetCategorySelectList(int? parentId, byte cultureId, bool bypassCache = false)
        {
            var cacheKey = $"{nameof(IntranetCacheService)}.{nameof(GetCategorySelectList)} {nameof(parentId)}:{parentId}, {nameof(cultureId)}:{cultureId}";
            var data = bypassCache ? null : Get<SelectList>(cacheKey);
            if (bypassCache || data == null)
            {
                data = new SelectList(GetCategory(parentId, cultureId, bypassCache), "Key", "Value");
                Set(cacheKey, data, DateTime.Today.AddMonths(1));
            }
            return data;
        }
    }
}