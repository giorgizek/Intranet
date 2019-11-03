using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Document
{
    public class DocumentTemplateFilterViewModel : FilterViewModel<DocumentTemplatesViewModel>
    {
        [Display(Name = nameof(ApplicationResources.Id), ResourceType = typeof(ApplicationResources))]
        public int? Id { get; set; }

        [Display(Name = nameof(ApplicationResources.Name), ResourceType = typeof(ApplicationResources))]
        public string Name { get; set; }
    }
}