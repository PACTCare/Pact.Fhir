namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;

  /// <summary>
  /// see http://hl7.org/fhir/http.html#create
  /// </summary>
  public class CreateResourceInteractor : UsecaseInteractor<CreateResourceRequest, CreateResourceResponse>
  {
    /// <inheritdoc />
    public CreateResourceInteractor(IFhirRepository repository, FhirResourceParser fhirParser)
      : base(repository)
    {
      this.FhirParser = fhirParser;
    }

    public FhirResourceParser FhirParser { get; }

    public override async Task<CreateResourceResponse> ExecuteAsync(CreateResourceRequest request)
    {
      try
      {
        var requestResource = this.FhirParser.Parse(request.ResourceType, request.ResourceJson);
        var resource = await this.Repository.CreateResourceAsync(requestResource);

        return new CreateResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (UnsupportedResourceException exception)
      {
        return new CreateResourceResponse { Code = ResponseCode.UnsupportedResource, ExceptionMessage = exception.Message };
      }
      catch (FormatException exception)
      {
        return new CreateResourceResponse { Code = ResponseCode.UnprocessableEntity, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new CreateResourceResponse
                 {
                   Code = ResponseCode.Failure, ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}