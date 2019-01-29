using System;
using System.Collections.Generic;
using System.Text;

namespace Pact.Fhir.Core.Usecase.ReadResourceSummary
{
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  using Exception = System.Exception;

  public class ReadResourceSummaryInteractor : UsecaseInteractor<ReadResourceSummaryRequest, ReadResourceSummaryResponse>
  {
    /// <inheritdoc />
    public ReadResourceSummaryInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ReadResourceSummaryResponse> ExecuteAsync(ReadResourceSummaryRequest request)
    {
      try
      {
        var resources = await this.Repository.ReadResourceSummaryAsync(request.ResourceId);
        if (resources.Count == 0 || resources.First().ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        return new ReadResourceSummaryResponse { Code = ResponseCode.Success, Resources = resources };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ReadResourceSummaryResponse { Code = ResponseCode.ResourceNotFound, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new ReadResourceSummaryResponse
        {
                   Code = ResponseCode.Failure,
                   ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}
