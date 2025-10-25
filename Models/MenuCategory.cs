using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class MenuCategory: BaseModel
    {
     

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        [Remote(action:"IsCategoryNameInUse", controller:"MenuCategory",AdditionalFields ="Id")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Display order must be between 0 and 100")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        // Navigation Property
        public  List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }

}
