namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// The update resource interactor.
  /// </summary>
  public class UpdateResourceInteractor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateResourceInteractor"/> class.
    /// </summary>
    /// <param name="fhirRepository">
    /// The fhir repository.
    /// </param>
    public UpdateResourceInteractor(IFhirPatientRepository fhirRepository)
    {
      this.FhirRepository = fhirRepository;
    }

    /// <summary>
    /// Gets the fhir repository.
    /// </summary>
    private IFhirPatientRepository FhirRepository { get; }

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
    public async Task<UpdateResourceResponse<T>> ExecuteAsync<T>(UpdateResourceRequest<T> request)
      where T : DomainResource
    {
      if (!await this.FhirRepository.HasChannel(request.ResourceSubseed))
      {
        return new UpdateResourceResponse<T> { Result = InteractionResult.UnknownEntity };
      }

      var updatedResource = await this.FhirRepository.UpdateResourceAsync(request.Resource, request.ResourceSubseed);

      return new UpdateResourceResponse<T> { Resource = updatedResource.Resource, Result = InteractionResult.Success };
    }
  }
}