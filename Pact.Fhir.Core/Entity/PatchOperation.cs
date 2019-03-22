namespace Pact.Fhir.Core.Entity
{
  using System.Collections.Generic;

  using Newtonsoft.Json;

  public class PatchOperation
  {
    [JsonProperty("op")]
    [JsonRequired]
    public string Operation { get; set; }

    [JsonProperty("path")]
    [JsonRequired]
    public string Path { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    public static List<PatchOperation> Parse(string json)
    {
      return JsonConvert.DeserializeObject<List<PatchOperation>>(json);
    }
  }
}