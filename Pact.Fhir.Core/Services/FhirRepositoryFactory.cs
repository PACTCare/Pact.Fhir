namespace Pact.Fhir.Core.Services
{
  using System;
  using System.Reflection;

  using Pact.Fhir.Core.Repository;

  public static class FhirRepositoryFactory
  {
    private static readonly object LockObject = new object();

    public static FhirRepository Create(string assemblyName, string typeName)
    {
      lock (LockObject)
      {
        var assembly = Assembly.LoadFrom(assemblyName);
        if (!(assembly.CreateInstance(typeName) is FhirRepository repository))
        {
          throw new Exception($"Can not instantiate IFhirRepository: {assemblyName}/{typeName}");
        }

        return repository;
      }
    }
  }
}