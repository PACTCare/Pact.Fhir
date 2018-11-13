namespace Pact.Fhir.Core.Usecase.GetResourceVersion
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services.Cache;
  using Pact.Fhir.Core.Usecase.GetResource;

  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  /// <summary>
  /// The get resource version interactor.
  /// </summary>
  public class GetResourceVersionInteractor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResourceVersionInteractor"/> class.
    /// </summary>
    /// <param name="fhirRepository">
    /// The fhir repository.
    /// </param>
    /// <param name="cache">
    /// The cache.
    /// </param>
    public GetResourceVersionInteractor(IFhirPatientRepository fhirRepository, IResourceCache cache)
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
    /// The execute async.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    public async Task<GetResourceResponse<T>> ExecuteAsync<T>(GetResourceVersionRequest request)
      where T : DomainResource
    {
      try
      {
        if (this.ResourceCache.IsSet(request.Root))
        {
          return new GetResourceResponse<T> { Result = InteractionResult.Success, Resource = this.ResourceCache.GetResourceByRoot<T>(request.Root) };
        }

        var resource = await this.FhirRepository.GetResourceVersion<T>(request.Root);

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