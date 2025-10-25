using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Models
{
    public class MenuItem: BaseModel
    {
       

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters")]
        [Display(Name = "Item Name")]
        [Remote(action: "IsMenuItemNameInUse", controller: "MenuItem", AdditionalFields = "Id")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(0,10000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Preparation time is required")]
        [Display(Name = "Preparation Time (Minutes)")]
        [Range(0,100)]
        public int PreparationTimeMinutes { get; set; }

        //[Display(Name = "Available")]
        //public bool IsAvailable { get; set; } = true;
        [Range(0,1000)]
        [Required]
        public int quanty { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Display(Name = "Image URL")]
        [DataType(DataType.ImageUrl)]
        
        public string? ImageUrl { get; set; }
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        [Display(Name = "Daily Order Count")]
        public int DailyOrderCount { get; set; } = 0;

        //[Display(Name = "Last Reset Date")]
        //public DateTime LastResetDate { get; set; } = DateTime.Today;

        public  MenuCategory? Category { get; set; }

        public  List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); 
    }

}
