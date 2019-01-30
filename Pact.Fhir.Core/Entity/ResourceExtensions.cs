namespace Pact.Fhir.Core.Entity
{
  using System;
  using System.Text.RegularExpressions;

  using Hl7.Fhir.Model;

  public static class ResourceExtensions
  {
    /// <summary>
    /// "The server SHALL populate the id, meta.versionId and meta.lastUpdated"
    /// Id Pattern: [A-Za-z0-9\-\.]{1,64} (see Id.PATTERN)
    /// https://www.hl7.org/fhir/datatypes.html#id
    /// </summary>
    public static void PopulateMetadata(this Resource resource, string id, string versionId)
    {
      var idMatches = Regex.Matches(id, Id.PATTERN);
      var versionIdMatches = Regex.Matches(versionId, Id.PATTERN);

      if (idMatches.Count == 0)
      {
        throw new ArgumentException($"Invalid id: {id}");
      }

      if (versionIdMatches.Count == 0)
      {
        throw new ArgumentException($"Invalid versionId: {versionId}");
      }

      // adjust length of ids to FHIR specified length
      id = idMatches[0].Value;
      versionId = versionIdMatches[0].Value;

      resource.Id = id;
      resource.VersionId = versionId;

      if (resource.Meta == null)
      {
        resource.Meta = new Meta();
      }

      resource.Meta.LastUpdated = DateTimeOffset.UtcNow;
      resource.Meta.VersionId = versionId;
    }
  }
}