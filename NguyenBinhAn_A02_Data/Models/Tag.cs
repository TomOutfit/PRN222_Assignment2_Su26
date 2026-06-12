using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenBinhAn_A02_Data.Models
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        [Column("TagID")]
        public int TagId { get; set; }

        [Column("TagName")]
        [StringLength(50)]
        public string? TagName { get; set; }

        [Column("Note")]
        [StringLength(400)]
        public string? Note { get; set; }

        // Navigation properties
        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
