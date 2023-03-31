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
        
        [JsonProperty("form")]
        public double Form { get; set; }
        
        [JsonProperty("personal")]
        public double Personal { get; set; }
        
        [JsonProperty("condition")]
        public double Condition { get; set; }
        
        [JsonProperty("psych")]
        public double Psych { get; set; }
        
        [JsonProperty("rating")]
        public double Rating { get; set; }
        
        [JsonProperty("win")]
        public int Win { get; set; }
    }
}