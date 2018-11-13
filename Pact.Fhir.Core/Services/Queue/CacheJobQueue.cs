namespace Pact.Fhir.Core.Services.Queue
{
  using System;
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.GetResource;
  using Pact.Fhir.Core.Usecase.GetResourceHistory;

  using Tangle.Net.Entity;

  /// <summary>
  /// The repository job queue.
  /// </summary>
  public class CacheJobQueue
  {
    /// <summary>
    /// The job queue.
    /// </summary>
    private static Queue<Tuple<Hash, int>> jobQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheJobQueue"/> class.
    /// </summary>
    /// <param name="resourceRepository">
    /// The resource repository.
    /// </param>
    /// <param name="resourceInteractor">
    /// The resource Interactor.
    /// </param>
    public CacheJobQueue(IResourceRepository resourceRepository, GetResourceHistoryInteractor resourceInteractor)
    {
      this.ResourceRepository = resourceRepository;
      this.ResourceInteractor = resourceInteractor;
    }

    /// <summary>
    /// The job queue.
    /// </summary>
    public static Queue<Tuple<Hash, int>> Queue => jobQueue ?? (jobQueue = new Queue<Tuple<Hash, int>>());

    /// <summary>
    /// Gets the resource repository.
    /// </summary>
    private IResourceRepository ResourceRepository { get; }

    /// <summary>
    /// Gets the patient register.
    /// </summary>
    private GetResourceHistoryInteractor ResourceInteractor { get; }

    /// <summary>
    /// The workload.
    /// </summary>
    public void Workload()
    {
      while (true)
      {
        try
        {
          for (var i = 0; i < Queue.Count; i++)
          {
            var job = Queue.Dequeue();
            var resourceResponse = this.ResourceInteractor.ExecuteAsync<DomainResource>(new GetResourceRequest { Root = job.Item1 }).Result;

            for (var j = 0; j < resourceResponse.History.Count; j++)
            {
              this.ResourceRepository.SaveResource(resourceResponse.History[j], job.Item1, j, job.Item2);
            }
          }
        }
        catch
        {
          // ignored
        }
      }
    }
  }
}