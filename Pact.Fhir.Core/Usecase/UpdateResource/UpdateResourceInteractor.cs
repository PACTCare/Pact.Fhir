namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public class UpdateResourceInteractor : UsecaseInteractor<UpdateResourceRequest, UpdateResourceResponse>
  {
    /// <inheritdoc />
    public UpdateResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override Task<UpdateResourceResponse> ExecuteAsync(UpdateResourceRequest request)
    {
      return null;
    }
  }
}