using System;
using System.ComponentModel.DataAnnotations;
using Intranet.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zek.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel.Person;

namespace Intranet.Model.ViewModel.HR
{
    public class EmployeeViewModel : PersonViewModel
    {
        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public int? JobTitleId { get; set; }
        public SelectList JobTitles { get; set; }

        [DataType(DataType.Date)]
        [DateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Display(Name = nameof(ContactResources.Branch), ResourceType = typeof(ContactResources))]
        public int? BranchId { get; set; }
        public SelectList Branchs { get; set; }

        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        public SelectList Departments { get; set; }

        [Display(Name = nameof(DrawingResources.Image), ResourceType = typeof(DrawingResources))]
        public IFormFile Image { get; set; }

        [BindNever]
        public string ImageUrl { get; set; }

        public bool ImageDelete { get; set; }
    }
}
