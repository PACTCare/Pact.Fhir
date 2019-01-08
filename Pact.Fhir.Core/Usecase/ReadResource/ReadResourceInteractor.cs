namespace Pact.Fhir.Core.Usecase.ReadResource
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see https://www.hl7.org/fhir/http.html#read
  /// </summary>
  public class ReadResourceInteractor : UsecaseInteractor<ReadResourceRequest, ReadResourceResponse>
  {
    /// <inheritdoc />
    public ReadResourceInteractor(FhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override Task<ReadResourceResponse> ExecuteAsync(ReadResourceRequest request)
    {
      return null;
    }
  }
}