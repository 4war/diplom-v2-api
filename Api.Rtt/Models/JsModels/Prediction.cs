using System;
using Api.Rtt.Models.Entities;
using Newtonsoft.Json;

namespace Api.Rtt.Models.JsModels
{
    public class Prediction
    {
        [JsonProperty("self")]
        public Player Self { get; set; }
        
        [JsonProperty("enemy")]
        public Player Enemy { get; set; }
        
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        
        [JsonProperty("clearPrediction")]
        public int ClearPrediction { get; set; }

        [JsonProperty("ratingPrediction")]
        public int RatingPrediction { get; set; }
    }
}