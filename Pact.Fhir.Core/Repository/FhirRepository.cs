namespace Pact.Fhir.Core.Repository
{
  using System;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  public abstract class FhirRepository
  {
    /// <summary>
    /// see http://hl7.org/fhir/http.html#create
    /// implementations MUST set LogicalId, VersionId and LastUpdated
    /// see https://www.hl7.org/fhir/datatypes.html#id
    /// Id format: [A-Za-z0-9\-\.]{1,64} (see Id.PATTERN)
    /// </summary>
    /// <param name="resource">
    /// The resource to create
    /// </param>
    /// <returns>
    /// Resource with adjusted values (LogicalId, VersionId, LastUpdated)
    /// </returns>
    public abstract Task<DomainResource> CreateResourceAsync(DomainResource resource);

    /// <summary>
    /// see https://www.hl7.org/fhir/http.html#read
    /// The returned resource SHALL have an id element with a value that is the [id]
    /// </summary>
    /// <param name="id">
    /// Id of the requested resource
    /// </param>
    /// <returns>
    /// The requested resource
    /// </returns>
    public abstract Task<DomainResource> ReadResourceAsync(string id);

    /// <summary>
    /// "The server SHALL populate the id, meta.versionId and meta.lastUpdated"
    /// Id Pattern: [A-Za-z0-9\-\.]{1,64} (see Id.PATTERN)
    /// https://www.hl7.org/fhir/datatypes.html#id
    /// </summary>
    protected void PopulateMetadata(Resource resource, string id, string versionId)
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