using System;
using System.ComponentModel.DataAnnotations;
using Zek.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Document
{
    public class DocumentFilterViewModel : FilterViewModel<DocumentsViewModel>
    {
        [Display(Name = nameof(ApplicationResources.Id), ResourceType = typeof(ApplicationResources))]
        public int? Id { get; set; }

        [DateDisplayFormat(ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = nameof(DateTimeResources.FromDate), ResourceType = typeof(DateTimeResources))]
        public DateTime? Date1 { get; set; }

        [DateDisplayFormat(ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = nameof(DateTimeResources.ToDate), ResourceType = typeof(DateTimeResources))]
        public DateTime? Date2 { get; set; }

        public int? TemplateId { get; set; }
    }
}
