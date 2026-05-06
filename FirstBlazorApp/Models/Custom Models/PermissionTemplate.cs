using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FirstBlazorApp.Models.Custom_Models
{
    public class  PermissionTemplateDetails
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string FormDisplayName { get; set; }
        public int FunctionalityId { get; set; }
        public string FunctionalityName { get; set; }
        public bool IsAllow { get; set; }

    }
    public class PermissionTemplateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Template Name is required")]
        [Remote(action: "ValidateTemplateName", controller: "Permission")]
        public string TemplateName { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionTemplateDetails> permissionTemplates { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Enum makerAction { get; set; }

        public bool ReadOnly { get; set; }
    }
}
