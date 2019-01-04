namespace Pact.Fhir.Core.Usecase
{
  using Pact.Fhir.Core.Repository;

  public abstract class UsecaseInteractor<TRequest, TResponse>
    where TResponse : UsecaseResponse
  {
    public UsecaseInteractor(IFhirRepository repository)
    {
      this.Repository = repository;
    }

    protected IFhirRepository Repository { get; }

    public abstract TResponse Execute(TRequest request);
  }
}