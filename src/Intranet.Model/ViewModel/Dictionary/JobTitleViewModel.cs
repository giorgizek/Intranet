using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zek.Localization;
using Zek.Model.ViewModel;
using Zek.Utils;

namespace Intranet.Model.ViewModel.Dictionary
{
    public class JobTitleViewModel : EditBaseViewModel
    {
        [Required(ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        public SelectList Departments { get; set; }

        [Required(ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        public List<KeyPair<int, string>> Texts { get; set; }

        public Dictionary<int, string> Cultures { get; set; }
    }
}