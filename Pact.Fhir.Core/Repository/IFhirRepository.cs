namespace Pact.Fhir.Core.Repository
{
  using System.Collections.Generic;
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
    Task<Resource> CreateResourceAsync(Resource resource);

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
    Task<Resource> ReadResourceAsync(string id);

    /// <summary>
    /// see https://www.hl7.org/fhir/search.html#summary
    /// </summary>
    /// <param name="id">
    /// Id of the requested resource
    /// </param>
    /// <returns>
    /// The requested resources
    /// </returns>
    Task<List<Resource>> ReadResourceHistoryAsync(string id);

    /// <summary>
    /// see https://www.hl7.org/fhir/http.html#vread
    /// The returned resource SHALL have an id element with a value that is the [id],
    /// and a meta.versionId element with a value of [vid]
    /// </summary>
    /// <param name="versionId">
    /// Version id of the requested resource
    /// </param>
    /// <returns>
    /// The requested resource
    /// </returns>
    Task<Resource> ReadResourceVersionAsync(string versionId);

    /// <summary>
    /// see https://www.hl7.org/fhir/http.html#update
    /// implementations MUST set VersionId and LastUpdated
    /// see https://www.hl7.org/fhir/datatypes.html#id
    /// Id format: [A-Za-z0-9\-\.]{1,64} (see Id.PATTERN)
    /// </summary>
    /// <param name="resource">
    /// The resource to update
    /// </param>
    /// <returns>
    /// Resource with adjusted values (VersionId, LastUpdated)
    /// </returns>
    Task<Resource> UpdateResourceAsync(Resource resource);
  }
}