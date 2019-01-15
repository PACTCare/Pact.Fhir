Working with the Core
============
Right now the core is more or less a code skeleton, that will grow over time.
The documentation will be expanded as needed.

The intend is, that interactors know all needed business rules to handle a FHIR operation. Therefore they will orchestrate all actions needed.
Given that they are not tightly coupled to a data source, interactors are able to work with different ones. It is up to you to either use the IOTA implementation or implement your own FhirRepository.

Implementing your own FhirRepository
----------------------
If you decide to start using the FHIR core with your own data source, what you need to do, is to implement the FhirRepository. 
For the sake of simplicity the documentation will use the in memory solution below:


.. code-block:: python
  public class InMemoryFhirRepository : FhirRepository
  {
    public InMemoryFhirRepository()
    {
      this.Resources = new List<DomainResource>();
    }

    public List<DomainResource> Resources { get; }

    /// <inheritdoc />
    public override async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      var resourceId = Guid.NewGuid().ToString("N");

      this.PopulateMetadata(resource, resourceId, resourceId);
      this.Resources.Add(resource);

      return resource;
    }

    /// <inheritdoc />
    public override async Task<DomainResource> ReadResourceAsync(string id)
    {
      var resource = this.Resources.FirstOrDefault(r => r.Id == id);

      if (resource == null)
      {
        throw new ResourceNotFoundException(id);
      }

      return resource;
    }
  }

Creating a resource
----------------------

.. code-block:: python
    var interactor = new CreateResourceInteractor(new InMemoryFhirRepository());
    var response = await interactor.ExecuteAsync(new CreateResourceRequest { Resource = resource });

Reading a resource
----------------------

.. code-block:: python
    var interactor = new ReadResourceInteractor(new InMemoryFhirRepository());
    var response = await interactor.ExecuteAsync(new ReadResourceRequest { ResourceId = "yourfhirresourcelogicalid" });