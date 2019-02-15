namespace Pact.Fhir.Core.Services
{
  using Hl7.Fhir.Rest;

  public static class SummaryTypeParser
  {
    public static SummaryType Parse(string summaryType)
    {
      if (string.IsNullOrEmpty(summaryType))
      {
        return SummaryType.False;
      }

      switch (summaryType.ToLower())
      {
        case "true":
          return SummaryType.True;
        case "text":
          return SummaryType.Text;
        case "data":
          return SummaryType.Data;
        case "count":
          return SummaryType.Count;
        default:
          return SummaryType.False;
      }
    }
  }
}