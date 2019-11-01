namespace Pact.Fhir.Core.Usecase.SearchResources
{
  using System.Collections.Generic;

  public class SearchPrefix
  {
    public enum Prefix
    {
      Equal,

      NotEqual,

      GreaterThan,

      LessThan,

      GreaterThanOrEqual,

      LessThanOrEqual,

      StartsAfter,

      EndsBefore,

      ApproximatelySame
    }

    public static string Equal => "eq";

    public static Dictionary<string, Prefix> Dictionary =>
      new Dictionary<string, Prefix>
        {
          { Equal, Prefix.Equal },
          { "ne", Prefix.NotEqual },
          { "gt", Prefix.GreaterThan },
          { "lt", Prefix.LessThan },
          { "ge", Prefix.GreaterThanOrEqual },
          { "le", Prefix.LessThanOrEqual },
          { "sa", Prefix.StartsAfter },
          { "eb", Prefix.EndsBefore },
          { "ap", Prefix.ApproximatelySame },
        };

    public static bool HasPrefix(string value)
    {
      return Dictionary.ContainsKey(value.Substring(0, 2));
    }
  }
}