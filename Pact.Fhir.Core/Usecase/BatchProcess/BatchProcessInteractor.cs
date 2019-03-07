namespace Pact.Fhir.Core.Usecase.BatchProcess
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Repository;

  public class BatchProcessInteractor : UsecaseInteractor<BatchProcessRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public BatchProcessInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(BatchProcessRequest request)
    {
      var responseBundle = new Bundle { Type = Bundle.BundleType.BatchResponse, };
      responseBundle.PopulateMetadata("bundle-response", "bundle-response");

      foreach (var entry in request.Bundle.Entry)
      {
        var resource = entry.Resource;
        Bundle.ResponseComponent response = null;

        try
        {
          switch (entry.Request.Method)
          {
            case Bundle.HTTPVerb.GET:
              resource = await this.Repository.ReadResourceAsync(entry.Resource.Id);
              response = new Bundle.ResponseComponent { Status = "200 OK" };
              break;
            case Bundle.HTTPVerb.POST:
              resource = await this.Repository.CreateResourceAsync(entry.Resource);
              break;
            case Bundle.HTTPVerb.PUT:
              resource = await this.Repository.UpdateResourceAsync(entry.Resource);
              break;
            case Bundle.HTTPVerb.DELETE:
              await this.Repository.DeleteResourceAsync(entry.Resource.Id);
              break;
            default:
              break;
          }
        }
        catch
        {

        }

        responseBundle.Entry.Add(new Bundle.EntryComponent { Resource = resource, Response = response });
      }

      return new ResourceResponse { Code = ResponseCode.Success, Resource = responseBundle };
    }
  }
}