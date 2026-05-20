using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenBinhAn_A02_Data.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CategoryID")]
        public short CategoryId { get; set; }

        [Column("CategoryName")]
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        [Column("CategoryDesciption")]
        [Required]
        [StringLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        [Column("ParentCategoryID")]
        public short? ParentCategoryId { get; set; }

        [Column("IsActive")]
        public bool? IsActive { get; set; }

        // Navigation properties
        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}
