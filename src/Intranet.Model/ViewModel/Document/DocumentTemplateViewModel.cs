using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Document
{
    public class DocumentTemplatesViewModel : ListBaseViewModel
    {
        [Display(Name = nameof(ApplicationResources.Name), ResourceType = typeof(ApplicationResources))]
        public string Name { get; set; }
    }

    public class DocumentTemplateViewModel : EditBaseViewModel
    {
        [Required(ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        [Display(Name = nameof(ApplicationResources.Name), ResourceType = typeof(ApplicationResources))]
        public string Name { get; set; }

        public Dictionary<int, string> Categories { get; set; }
        public List<DocumentTemplateProductViewModel> Products { get; set; }
    }

    public class DocumentTemplateProductCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DocumentTemplateProductViewModel
    {
        public int Id { get; set; }
        public string ProductNumber { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}
