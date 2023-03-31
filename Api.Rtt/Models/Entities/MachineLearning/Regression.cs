using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Rtt.Models.Entities.MachineLearning
{
    [Table("regression")]
    public class Regression
    {
        [Key, ForeignKey(nameof(Match))]
        public int MatchId { get; set; }
        public virtual Match Match { get; set; }

        [Column("f_ste")]
        public double FactorScoreTimeEffect { get; set; }
        
        [Column("f_count")]
        public double FactorCount { get; set; }
        
        [Column("f_div")]
        public double FactorDiversity { get; set; }

        [Column("f_per")]
        public double FactorScoreTimeEffectPersonal { get; set; }
        
        [Column("f_dt")]
        public double FactorDayTime { get; set; }
        
        [Column("f_fatigue")]
        public double FactorFatigue { get; set; }
        
        [Column("f_moral")]
        public double FactorMoral { get; set; }
        
        [Column("f_style")]
        public double FactorStyle { get; set; }
        
        [Column("f_rating")]
        public double FactorRating { get; set; }
        
        [Column("CurrentPrediction")]
        public double CurrentPrediction { get; set; }
        
        [Column("Actual")]
        public double Actual { get; set; }
    }
}