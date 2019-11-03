using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zek.DataAnnotations;
using Zek.Localization;
using Zek.Model.ViewModel;

namespace Intranet.Model.ViewModel.Document
{
    public class DocumentViewModel : EditBaseViewModel
    {
        [BindNever]
        [DateDisplayFormat(ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = nameof(DocumentResources.ApplicationDate), ResourceType = typeof(DocumentResources))]
        public DateTime? Date { get; set; }

        [Required(ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        [Display(Name = nameof(ApplicationResources.Comment), ResourceType = typeof(ApplicationResources))]
        public int? TemplateId { get; set; }

        public Dictionary<int, string> Categories { get; set; }

        [StringLength(400, ErrorMessageResourceName = nameof(DataAnnotationsResources.StringLengthAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        [Display(Name = nameof(ApplicationResources.Comment), ResourceType = typeof(ApplicationResources))]
        public string Comment { get; set; }


        public List<DocumentProductsViewModel> Products { get; set; }

    }

    public class DocumentProductsViewModel
    {
        public int Id { get; set; }
        public string ProductNumber { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }

        [Display(Name = nameof(ProductResources.Quantity), ResourceType = typeof(ProductResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = nameof(DataAnnotationsResources.RequiredAttribute_ValidationError), ErrorMessageResourceType = typeof(DataAnnotationsResources))]
        public int? Quantity { get; set; }
    }
}
