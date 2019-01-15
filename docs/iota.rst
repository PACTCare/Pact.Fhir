Working with IOTA and FHIR
============
If you are not familiar with tangle.net (https://github.com/Felandil/tangle-.net) and IOTA you might want to read a little about it before starting here.

Working with resources
--------------
In the IOTA context, all resources are stored on as restricted MAM streams where each resource has its own stream.
Resources are assigned the first 64 chars of the corresponding MAM root as a logical/version id upon creation or alteration.

Keys and Channels
--------------
Since access to the MAM streams is restricted, the repository has to keep track of all channels.
To get you started the below linked in memory solution should be enough. Anyway you will want to write your own implementation that stores such information in a trusted data storage.

See: https://github.com/PACTCare/Pact.Fhir/blob/develop/Pact.Fhir.Iota.Tests/Services/InMemoryResourceTracker.cs

Serialization
--------------
The resource payload is stored as a serialized JSON or XML depending which serializer implementation you favour. If JSON/XML is not what you are looking for, you can implement your own serializer by impelemting the IFhirTryteSerializer interface.

Code example
--------------
The IOTA FHIR repository is coupled to the tangle.net IOTA REST implementation.

See: https://github.com/PACTCare/Pact.Fhir/blob/develop/Pact.Fhir.Iota.Tests/Utils/IotaResourceProvider.cs


.. code-block:: c
  var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer(), new InMemoryResourceTracker());
  var creationInteractor = new CreateResourceInteractor(repository);
  var response = await creationInteractor.ExecuteAsync(new CreateResourceRequest { Resource = resource });

  var readInteractor = new ReadResourceInteractor(repository);
  var response = await readInteractor.ExecuteAsync(new ReadResourceRequest { ResourceId = createdResource.Id });