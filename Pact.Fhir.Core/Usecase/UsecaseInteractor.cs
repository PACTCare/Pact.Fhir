namespace Pact.Fhir.Core.Usecase
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public abstract class UsecaseInteractor<TRequest, TResponse>
    where TResponse : BaseResponse
  {
    public UsecaseInteractor(IFhirRepository repository)
    {
      this.Repository = repository;
    }

    protected IFhirRepository Repository { get; }

    public abstract Task<TResponse> ExecuteAsync(TRequest request);
  }
}