namespace Pact.Fhir.Core.Usecase.BatchProcess
{
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
        try
        {
          responseBundle.Entry.Add(await this.ProcessEntity(entry));
        }
        catch (UnsupportedOperationException)
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = entry.Resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.MethodNotAllowed.ToString() }
              });
        }
        catch (ResourceNotFoundException)
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = entry.Resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.NotFound.ToString() }
              });
        }
        catch
        {
          responseBundle.Entry.Add(
            new Bundle.EntryComponent
              {
                Resource = entry.Resource, Response = new Bundle.ResponseComponent { Status = HttpStatusCode.InternalServerError.ToString() }
              });
        }
      }

      return new ResourceResponse { Code = ResponseCode.Success, Resource = responseBundle };
    }

    private async Task<Bundle.EntryComponent> ProcessEntity(Bundle.EntryComponent entry)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (entry.Request.Method)
      {
        case Bundle.HTTPVerb.GET:
          return new Bundle.EntryComponent
                   {
                     Resource = await this.Repository.ReadResourceAsync(entry.Resource.Id),
                     Response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() }
                   };
        case Bundle.HTTPVerb.POST:
          return new Bundle.EntryComponent
                   {
                     Resource = await this.Repository.CreateResourceAsync(entry.Resource),
                     Response = new Bundle.ResponseComponent { Status = HttpStatusCode.Created.ToString() }
                   };
        case Bundle.HTTPVerb.PUT:
          return new Bundle.EntryComponent
                   {
                     Resource = await this.Repository.UpdateResourceAsync(entry.Resource),
                     Response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() }
                   };
        case Bundle.HTTPVerb.DELETE:
          await this.Repository.DeleteResourceAsync(entry.Resource.Id);
          return new Bundle.EntryComponent
                   {
                     Resource = entry.Resource,
                     Response = new Bundle.ResponseComponent { Status = HttpStatusCode.OK.ToString() }
                   };
        default:
          throw new UnsupportedOperationException(entry.Request.Method.ToString());
      }
    }
  }
}