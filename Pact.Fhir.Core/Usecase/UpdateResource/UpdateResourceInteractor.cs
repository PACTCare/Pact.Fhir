namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  public class UpdateResourceInteractor : UsecaseInteractor<UpdateResourceRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public UpdateResourceInteractor(IFhirRepository repository, FhirJsonParser fhirParser)
      : base(repository)
    {
      this.FhirParser = fhirParser;
    }

    private FhirJsonParser FhirParser { get; }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(UpdateResourceRequest request)
    {
      try
      {
        var requestResource = this.FhirParser.Parse<DomainResource>(request.ResourceJson);
        if (requestResource.Id != request.ResourceId)
        {
          return new ResourceResponse { Code = ResponseCode.IdMismatch };
        }

        var resource = await this.Repository.UpdateResourceAsync(requestResource);

        return new ResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (FormatException exception)
      {
        return new ResourceResponse { Code = ResponseCode.UnprocessableEntity, ExceptionMessage = exception.Message };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ResourceResponse { Code = ResponseCode.MethodNotAllowed, ExceptionMessage = exception.Message };
      }
      catch (AuthorizationRequiredException exception)
      {
        return new ResourceResponse { Code = ResponseCode.AuthorizationRequired, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new ResourceResponse
        {
                   Code = ResponseCode.Failure, ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}