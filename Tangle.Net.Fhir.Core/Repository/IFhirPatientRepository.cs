namespace Tangle.Net.Fhir.Core.Repository
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;
  using Tangle.Net.Fhir.Core.Repository.Responses;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The FhirPatientRepository interface.
  /// </summary>
  public interface IFhirPatientRepository
  {
    /// <summary>
    /// The create.
    /// </summary>
    /// <param name="resource">
    /// The resource.
    /// </param>
    /// <param name="seed">
    /// The seed.
    /// </param>
    /// <param name="channelKey">
    /// The channel Key.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="System.Threading.Tasks.Task"/>.
    /// </returns>
    Task<ResourceReponse<T>> CreateResourceAsync<T>(T resource, Seed seed, TryteString channelKey)
      where T : DomainResource;

    /// <summary>
    /// The get history.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <param name="channelKey">
    /// The channel key.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    Task<List<T>> GetHistory<T>(Hash root, TryteString channelKey)
      where T : DomainResource;

    /// <summary>
    /// The get patient async.
    /// </summary>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <param name="channelKey">
    /// The channel key.
    /// </param>
    /// <returns>
    /// The <see cref="System.Threading.Tasks.Task"/>.
    /// </returns>
    Task<T> GetResourceAsync<T>(Hash root, TryteString channelKey)
      where T : DomainResource;

    /// <summary>
    /// The update resource async.
    /// </summary>
    /// <param name="resource">
    /// The resource.
    /// </param>
    /// <param name="seed">
    /// The seed.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    Task<ResourceReponse<T>> UpdateResourceAsync<T>(T resource, Seed seed)
      where T : DomainResource;

    /// <summary>
    /// The add channel.
    /// </summary>
    /// <param name="channel">
    /// The channel.
    /// </param>
    void AddChannel(MamChannel channel);
  }
}