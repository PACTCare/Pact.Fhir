namespace Pact.Fhir.Core.Usecase.ValidateResource
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;

  public class ValidateResourceInteractor : UsecaseInteractor<ValidateResourceRequest, ValidateResourceResponse>
  {
    /// <inheritdoc />
    public ValidateResourceInteractor(IFhirRepository repository, FhirJsonParser fhirParser)
      : base(repository)
    {
      this.FhirParser = fhirParser;
    }

    private FhirJsonParser FhirParser { get; }

    /// <inheritdoc />
    public override async Task<ValidateResourceResponse> ExecuteAsync(ValidateResourceRequest request)
    {
      try
      {
        var requestResource = this.FhirParser.Parse<Resource>(request.ResourceJson);
        return new ValidateResourceResponse
                 {
                   Code = ResponseCode.Success, ValidationResult = FhirResourceValidator.Validate(requestResource).ToList()
                 };
      }
      catch (Exception)
      {
        return new ValidateResourceResponse
        {
                   Code = ResponseCode.Failure,
                   ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}