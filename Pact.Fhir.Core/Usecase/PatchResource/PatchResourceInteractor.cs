namespace Pact.Fhir.Core.Usecase.PatchResource
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public class PatchResourceInteractor : UsecaseInteractor<PatchResourceRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public PatchResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override Task<ResourceResponse> ExecuteAsync(PatchResourceRequest request)
    {
      
      return null;
    }
  }
}