namespace Pact.Fhir.Core.Usecase.GetResource
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services.Cache;

  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  /// <summary>
  /// The get patient data interactor.
  /// </summary>
  public class GetResourceInteractor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResourceInteractor"/> class.
    /// </summary>
    /// <param name="fhirRepository">
    /// The fhir Repository.
    /// </param>
    /// <param name="cache">
    /// The cache.
    /// </param>
    public GetResourceInteractor(IFhirPatientRepository fhirRepository, IResourceCache cache)
    {
      this.FhirRepository = fhirRepository;
      this.ResourceCache = cache;
    }

    /// <summary>
    /// Gets the fhir repository.
    /// </summary>
    private IFhirPatientRepository FhirRepository { get; }

    /// <summary>
    /// Gets the patient data cache.
    /// </summary>
    private IResourceCache ResourceCache { get; }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <typeparam name="T">
    /// The fihr type
    /// </typeparam>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <returns>
    /// The <see cref="System.Threading.Tasks.Task"/>.
    /// </returns>
    public async Task<GetResourceResponse<T>> ExecuteAsync<T>(GetResourceRequest request)
      where T : DomainResource
    {
      try
      {
        if (this.ResourceCache.IsSet(request.Root))
        {
          return new GetResourceResponse<T> { Result = InteractionResult.Success, Resource = this.ResourceCache.GetResourceByRoot<T>(request.Root) };
        }

        var resource = await this.FhirRepository.GetResourceAsync<T>(request.Root);

        this.ResourceCache.SetResource(resource, request.Root);

        return new GetResourceResponse<T> { Result = InteractionResult.Success, Resource = resource };
      }
      catch (Exception exception)
      {
        var result = InteractionResult.IotaException;
        switch (exception)
        {
          case InvalidBundleException _:
            result = InteractionResult.InvalidBundle;
            break;
          case ArgumentException _:
            result = InteractionResult.UnknownEntity;
            break;
        }

        return new GetResourceResponse<T> { Result = result };
      }
    }
  }
}