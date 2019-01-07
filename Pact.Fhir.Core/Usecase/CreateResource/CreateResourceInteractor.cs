namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public class CreateResourceInteractor : UsecaseInteractor<CreateResourceRequest, CreateResourceResponse>
  {
    /// <inheritdoc />
    public CreateResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    public override async Task<CreateResourceResponse> ExecuteAsync(CreateResourceRequest request)
    {
      var resource = await this.Repository.CreateResourceAsync(request.Resource);

      return new CreateResourceResponse { Code = ResponseCode.Success, LogicalId = resource.Id, VersionId = resource.VersionId };
    }
  }
}