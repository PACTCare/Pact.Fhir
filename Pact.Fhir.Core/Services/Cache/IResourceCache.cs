namespace Pact.Fhir.Core.Services.Cache
{
  using System;

  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The ResourceCache interface.
  /// </summary>
  public interface IResourceCache
  {
    /// <summary>
    /// The get resource by channel key.
    /// </summary>
    /// <typeparam name="T">
    /// The type.
    /// </typeparam>
    /// <param name="root">
    /// The channel key.
    /// </param>
    /// <returns>
    /// The <see cref="Tuple"/>.
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
    void SetResource<T>(T resource, Hash root)
      where T : DomainResource;

    /// <summary>
    /// The is set.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    bool IsSet(Hash root);
  }
}