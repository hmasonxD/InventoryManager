using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}