using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenBinhAn_A02_Data.Models
{
    [Table("SystemAccount")]
    public class SystemAccount
    {
        [Key]
        [Column("AccountID")]
        public short AccountId { get; set; }

        [Column("AccountName")]
        [StringLength(100)]
        public string? AccountName { get; set; }

        [Column("AccountEmail")]
        [StringLength(70)]
        public string? AccountEmail { get; set; }

        [Column("AccountRole")]
        public int? AccountRole { get; set; }

        [Column("AccountPassword")]
        [StringLength(70)]
        public string? AccountPassword { get; set; }

        // Navigation properties
        public virtual ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
        public virtual ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
    }
}
