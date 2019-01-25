namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;

  public class UpdateResourceInteractor : UsecaseInteractor<UpdateResourceRequest, UsecaseResponse>
  {
    /// <inheritdoc />
    public UpdateResourceInteractor(IFhirRepository repository, FhirJsonParser fhirParser)
      : base(repository)
    {
      this.FhirParser = fhirParser;
    }

    private FhirJsonParser FhirParser { get; }

    /// <inheritdoc />
    public override async Task<UsecaseResponse> ExecuteAsync(UpdateResourceRequest request)
    {
      try
      {
        var requestResource = this.FhirParser.Parse<DomainResource>(request.ResourceJson);
        if (requestResource.Id != request.ResourceId)
        {
          return new UsecaseResponse { Code = ResponseCode.IdMismatch };
        }

        var resource = await this.Repository.UpdateResourceAsync(requestResource);

        return new UsecaseResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (FormatException exception)
      {
        return new UsecaseResponse { Code = ResponseCode.UnprocessableEntity, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new UsecaseResponse
                 {
                   Code = ResponseCode.Failure, ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}