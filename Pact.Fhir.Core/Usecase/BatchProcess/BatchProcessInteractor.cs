namespace Pact.Fhir.Core.Usecase.BatchProcess
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Exception;
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

        try
        {
          Bundle.ResponseComponent response;

          switch (entry.Request.Method)
          {
            case Bundle.HTTPVerb.GET:
              resource = await this.Repository.ReadResourceAsync(entry.Resource.Id);
              response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() };
              break;
            case Bundle.HTTPVerb.POST:
              resource = await this.Repository.CreateResourceAsync(entry.Resource);
              response = new Bundle.ResponseComponent { Status = HttpStatusCode.Created.ToString() };
              break;
            case Bundle.HTTPVerb.PUT:
              resource = await this.Repository.UpdateResourceAsync(entry.Resource);
              response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() };
              break;
            case Bundle.HTTPVerb.DELETE:
              await this.Repository.DeleteResourceAsync(entry.Resource.Id);
              response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() };
              break;
            default:
              throw new UnsupportedOperationException(entry.Request.Method.ToString());
          }

          responseBundle.Entry.Add(new Bundle.EntryComponent { Resource = resource, Response = response });
        }
        catch (UnsupportedOperationException)
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.MethodNotAllowed.ToString() }
              });
        }
        catch (ResourceNotFoundException)
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.NotFound.ToString() }
              });
        }
        catch
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.InternalServerError.ToString() }
              });
        }
      }

      return new ResourceResponse { Code = ResponseCode.Success, Resource = responseBundle };
    }
  }
}