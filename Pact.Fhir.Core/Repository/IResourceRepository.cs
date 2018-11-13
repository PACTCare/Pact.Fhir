namespace Pact.Fhir.Core.Repository
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The ResourceRepository interface.
  /// </summary>
  public interface IResourceRepository
  {
    /// <summary>
    /// The get resource by root.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="T"/>.
    /// </returns>
    T GetResourceByRoot<T>(Hash root)
      where T : DomainResource;

    /// <summary>
    /// The set resource.
    /// </summary>
    /// <typeparam name="T">
    /// The type.
    /// </typeparam>
    /// <param name="resource">
    /// The resource.
    /// </param>
    /// <param name="root">
    /// The channel key.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="patientId">
    /// The patient Id.
    /// </param>
    void SaveResource<T>(T resource, Hash root, int index, int patientId)
      where T : DomainResource;
  }
}