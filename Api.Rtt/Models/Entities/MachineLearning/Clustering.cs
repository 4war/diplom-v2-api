using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities.MachineLearning
{
    [Table("clustering")]
    public class Clustering
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int PlayerRni { get; set; }
        [ForeignKey(nameof(PlayerRni))]
        public virtual Player Player { get; set; }
        
        public int Age { get; set; }
        public int Plays { get; set; }
        public int Points { get; set; }
    }
}