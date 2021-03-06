﻿namespace Pact.Fhir.Core.Usecase.ReadResourceHistory
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  using Exception = System.Exception;

  public class ReadResourceHistoryInteractor : UsecaseInteractor<ReadResourceHistoryRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public ReadResourceHistoryInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(ReadResourceHistoryRequest request)
    {
      try
      {
        var resources = await this.Repository.ReadResourceHistoryAsync(request.ResourceId);
        if (resources.Count == 0 || resources.First().ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        return new ResourceResponse
                 {
                   Code = ResponseCode.Success,
                   Resource = new Bundle
                                {
                                  Entry = new List<Bundle.EntryComponent>(
                                    resources.Select(
                                      r => new Bundle.EntryComponent
                                             {
                                               Resource = r,
                                               Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST },
                                               Response = new Bundle.ResponseComponent { LastModified = r.Meta.LastUpdated }
                                             })),
                                  Type = Bundle.BundleType.History
                                }
                 };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ResourceResponse { Code = ResponseCode.ResourceNotFound, Exception = exception };
      }
      catch (Exception exception)
      {
        return new ResourceResponse { Code = ResponseCode.Failure, Exception = exception };
      }
    }
  }
}
