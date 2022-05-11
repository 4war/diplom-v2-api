using Api.Rtt.Models.Entities;
using Newtonsoft.Json;

namespace Api.Rtt.Filter
{
  public class PlayerFilterOptions
  {
    [JsonProperty("surname")]
    public string Surname { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("gender")]
    public int? Gender { get; set; }

    [JsonProperty("pointsFrom")]
    public int PointsFrom { get; set; }

    [JsonProperty("pointsUntil")]
    public int PointsUntil { get; set; }

    [JsonProperty("dobYearFrom")]
    public int DobYearFrom { get; set; }

    [JsonProperty("dobYearUntil")]
    public int DobYearUntil { get; set; }

    [JsonProperty("page")]
    public int? Page { get; set; }

    [JsonProperty("take")]
    public int? Take { get; set; }
  }
}
