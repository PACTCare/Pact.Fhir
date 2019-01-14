namespace Pact.Fhir.Core.Usecase
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public abstract class UsecaseInteractor<TRequest, TResponse>
    where TResponse : UsecaseResponse
  {
    public UsecaseInteractor(FhirRepository repository)
    {
      this.Repository = repository;
    }

    protected FhirRepository Repository { get; }

    public abstract Task<TResponse> ExecuteAsync(TRequest request);
  }
}