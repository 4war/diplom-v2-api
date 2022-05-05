﻿using Newtonsoft.Json;

namespace Api.Rtt.Filter
{
  public class PlayerFilterOptions
  {
    [JsonProperty("startWith")]
    public string StartWith { get; set; }

    [JsonProperty("skip")]
    public int? Skip { get; set; }

    [JsonProperty("take")]
    public int? Take { get; set; }
  }
}