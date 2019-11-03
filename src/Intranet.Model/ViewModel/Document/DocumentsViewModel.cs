using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zek.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Document
{
    public class DocumentsViewModel : ListBaseViewModel
    {
        public int CreatorId { get; set; }

        [DateDisplayFormat]
        [Display(Name = nameof(DocumentResources.ApplicationDate), ResourceType = typeof(DocumentResources))]
        public DateTime? CreateDate { get; set; }

        public int TemplateId { get; set; }



        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public int? JobTitleId { get; set; }
        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public string JobTitle { get; set; }


        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public string Department { get; set; }

        [Display(Name = nameof(ApplicationResources.Approved), ResourceType = typeof(ApplicationResources))]
        public bool IsApproved { get; set; }
    }
    
}
