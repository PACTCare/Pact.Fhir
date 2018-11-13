namespace Pact.Fhir.Core.Services.Cache
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Usecase.GetResource;

  using Tangle.Net.Entity;

  /// <summary>
  /// The cache job queue.
  /// </summary>
  public class CacheJobQueue
  {
    /// <summary>
    /// The job queue.
    /// </summary>
    private static Queue<Hash> jobQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheJobQueue"/> class.
    /// </summary>
    /// <param name="resourceCache">
    /// The resource cache.
    /// </param>
    /// <param name="resourceInteractor">
    /// The resource Interactor.
    /// </param>
    public CacheJobQueue(IResourceCache resourceCache, GetResourceInteractor resourceInteractor)
    {
      this.ResourceCache = resourceCache;
      this.ResourceInteractor = resourceInteractor;
    }

    /// <summary>
    /// The job queue.
    /// </summary>
    public static Queue<Hash> Queue => jobQueue ?? (jobQueue = new Queue<Hash>());

    /// <summary>
    /// Gets the resource cache.
    /// </summary>
    private IResourceCache ResourceCache { get; }

    /// <summary>
    /// Gets the patient register.
    /// </summary>
    private GetResourceInteractor ResourceInteractor { get; }

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
            var jobRoot = Queue.Dequeue();
            var resourceResponse = this.ResourceInteractor.ExecuteAsync<DomainResource>(new GetResourceRequest { Root = jobRoot }).Result;
            this.ResourceCache.SetResource(resourceResponse.Resource, jobRoot);
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