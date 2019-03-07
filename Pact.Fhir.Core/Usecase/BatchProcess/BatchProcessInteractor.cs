namespace Pact.Fhir.Core.Usecase.BatchProcess
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  public class BatchProcessInteractor : UsecaseInteractor<BatchProcessRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public BatchProcessInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override Task<ResourceResponse> ExecuteAsync(BatchProcessRequest request)
    {
      var responses = new List<Bundle.ResponseComponent>();

      // loop through request bundle resources
      // process bundle entry with given method
      // add response to list

      // construct bundle with request resources and their matching responses

      return null;
    }
  }
}