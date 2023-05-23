using System.ComponentModel.DataAnnotations;

namespace AspNetIndustryProject.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal PricePerUnit { get; set; }
        [Required]
        public string Quantity { get; set; }
    }
}
