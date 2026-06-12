using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenBinhAn_A02_Data.Models
{
    [Table("NewsTag")]
    public class NewsTag
    {
        [Column("NewsArticleID")]
        [StringLength(20)]
        public string NewsArticleId { get; set; } = null!;

        [Column("TagID")]
        public int TagId { get; set; }

        // Navigation properties
        [ForeignKey("NewsArticleId")]
        public virtual NewsArticle NewsArticle { get; set; } = null!;
        
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
