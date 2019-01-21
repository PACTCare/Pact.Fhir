namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

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
      catch (UnsupportedResourceException)
      {
        return new CreateResourceResponse { Code = ResponseCode.UnsupportedResource };
      }
      catch (FormatException)
      {
        return new CreateResourceResponse { Code = ResponseCode.UnprocessableEntity };
      }
      catch (Exception)
      {
        return new CreateResourceResponse { Code = ResponseCode.Failure };
      }
    }
  }
}