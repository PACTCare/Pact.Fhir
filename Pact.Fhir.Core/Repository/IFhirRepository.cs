namespace Pact.Fhir.Core.Repository
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  public interface IFhirRepository
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
    Task<DomainResource> CreateResourceAsync(DomainResource resource);

    Task<DomainResource> ReadResourceAsync(string id);
  }
}