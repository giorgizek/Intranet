using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Dictionary
{
    public class JobTitleFilterViewModel : FilterViewModel<JobTitlesViewModel>
    {
        [Display(Name = nameof(ApplicationResources.Id), ResourceType = typeof(ApplicationResources))]
        public int? Id { get; set; }

        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public string Name { get; set; }

        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        public SelectList Departments { get; set; }
    }

    public class JobTitlesViewModel : ListBaseViewModel
    {
        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public string Name { get; set; }

        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int DepartmentId { get; set; }
        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public string Department { get; set; }
    }
}
