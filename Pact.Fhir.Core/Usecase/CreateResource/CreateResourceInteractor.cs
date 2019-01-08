namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see http://hl7.org/fhir/http.html#create
  /// </summary>
  public class CreateResourceInteractor : UsecaseInteractor<CreateResourceRequest, CreateResourceResponse>
  {
    /// <inheritdoc />
    public CreateResourceInteractor(FhirRepository repository)
      : base(repository)
    {
    }

    public override async Task<CreateResourceResponse> ExecuteAsync(CreateResourceRequest request)
    {
      try
      {
        var resource = await this.Repository.CreateResourceAsync(request.Resource);
        return new CreateResourceResponse { Code = ResponseCode.Success, LogicalId = resource.Id, VersionId = resource.VersionId };
      }
      catch (Exception)
      {
        return new CreateResourceResponse { Code = ResponseCode.Failure };
      }
    }
  }
}