Working with the Core
============
Right now the core is more or less a code skeleton, that will grow over time.
The documentation will be expanded as needed.

The intend is, that interactors know all needed business rules to handle a FHIR operation. Therefore they will orchestrate all actions needed.
Given that they are not tightly coupled to a data source, interactors are able to work with different ones. It is up to you to either use the IOTA implementation or implement your own FhirRepository.