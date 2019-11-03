using System.ComponentModel.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel.Contact;
using Zek.Model.ViewModel.Person;

namespace Intranet.Model.ViewModel.HR
{
    public class EmployeesViewModel : PersonsViewModel
    {
        [Display(Name = nameof(ContactResources.Branch), ResourceType = typeof(ContactResources))]
        public int? BranchId { get; set; }
        [Display(Name = nameof(ContactResources.Branch), ResourceType = typeof(ContactResources))]
        public string Branch { get; set; }


        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public int? JobTitleId { get; set; }
        [Display(Name = nameof(HrResources.JobTitle), ResourceType = typeof(HrResources))]
        public string JobTitle { get; set; }


        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public int? DepartmentId { get; set; }
        [Display(Name = nameof(HrResources.Department), ResourceType = typeof(HrResources))]
        public string Department { get; set; }

        public string ImageUrl { get; set; }


        public ContactBaseViewModel Contact { get; set; }
    }
}
