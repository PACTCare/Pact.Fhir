namespace Pact.Fhir.Core.Usecase.GetResourceHistory
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.GetResource;

  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  /// <summary>
  /// The get resource history interactor.
  /// </summary>
  public class GetResourceHistoryInteractor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResourceHistoryInteractor"/> class.
    /// </summary>
    /// <param name="fhirRepository">
    /// The fhir repository.
    /// </param>
    public GetResourceHistoryInteractor(IFhirPatientRepository fhirRepository)
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
    public async Task<GetResourceHistoryResponse<T>> ExecuteAsync<T>(GetResourceRequest request)
      where T : DomainResource
    {
      try
      {
        var history = await this.FhirRepository.GetHistory<T>(request.Root);
        return new GetResourceHistoryResponse<T> { Result = InteractionResult.Success, History = history };
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

        return new GetResourceHistoryResponse<T> { Result = result };
      }
    }
  }
}