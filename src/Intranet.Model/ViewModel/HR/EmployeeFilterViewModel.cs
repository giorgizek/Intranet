using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.HR
{
    public class EmployeeFilterViewModel : FilterViewModel<EmployeesViewModel>
    {
        [Display(Name = nameof(ApplicationResources.Id), ResourceType = typeof(ApplicationResources))]
        public int? Id { get; set; }

        [Display(Name = nameof(PersonResources.FullName), ResourceType = typeof(PersonResources))]
        public string FullName { get; set; }

        [Display(Name = nameof(PersonResources.PersonalNumber), ResourceType = typeof(PersonResources))]
        public string PersonalNumber { get; set; }


        [Display(Name = nameof(ContactResources.ExtensionNumber), ResourceType = typeof(ContactResources))]
        public string Phone { get; set; }

        [Display(Name = nameof(ContactResources.Mobile), ResourceType = typeof(ContactResources))]
        public string Mobile { get; set; }

        [Display(Name = nameof(ContactResources.Branch), ResourceType = typeof(ContactResources))]
        public int? BranchId { get; set; }
        public SelectList Branchs { get; set; }

        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public int? JobTitleId { get; set; }
        public SelectList JobTitles { get; set; }

        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        public SelectList Departments { get; set; }


        protected override int MaxPageSize => 100;
    }
}
