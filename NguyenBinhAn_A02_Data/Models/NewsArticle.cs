using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenBinhAn_A02_Data.Models
{
    [Table("NewsArticle")]
    public class NewsArticle
    {
        [Key]
        [Column("NewsArticleID")]
        [StringLength(20)]
        public string NewsArticleId { get; set; } = null!;

        [Column("NewsTitle")]
        [StringLength(400)]
        public string? NewsTitle { get; set; }

        [Column("Headline")]
        [Required]
        [StringLength(150)]
        public string Headline { get; set; } = null!;

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("NewsContent")]
        [StringLength(4000)]
        public string? NewsContent { get; set; }

        [Column("NewsSource")]
        [StringLength(400)]
        public string? NewsSource { get; set; }

        [Column("CategoryID")]
        public short? CategoryId { get; set; }

        [Column("NewsStatus")]
        public bool? NewsStatus { get; set; }

        [Column("CreatedByID")]
        public short? CreatedById { get; set; }

        [Column("UpdatedByID")]
        public short? UpdatedById { get; set; }

        [Column("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        
        [ForeignKey("CreatedById")]
        public virtual SystemAccount? Creator { get; set; }
        
        [ForeignKey("UpdatedById")]
        public virtual SystemAccount? Updater { get; set; }
        
        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
