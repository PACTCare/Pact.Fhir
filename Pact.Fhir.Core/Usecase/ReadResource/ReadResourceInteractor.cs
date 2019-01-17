namespace Pact.Fhir.Core.Usecase.ReadResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see https://www.hl7.org/fhir/http.html#read
  /// </summary>
  public class ReadResourceInteractor : UsecaseInteractor<ReadResourceRequest, ReadResourceResponse>
  {
    /// <inheritdoc />
    public ReadResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ReadResourceResponse> ExecuteAsync(ReadResourceRequest request)
    {
      try
      {
        return new ReadResourceResponse { Code = ResponseCode.Success, Resource = await this.Repository.ReadResourceAsync(request.ResourceId) };
      }
      catch (ResourceNotFoundException)
      {
        return new ReadResourceResponse { Code = ResponseCode.ResourceNotFound };
      }
      catch (Exception)
      {
        return new ReadResourceResponse { Code = ResponseCode.Failure };
      }
    }
  }
}