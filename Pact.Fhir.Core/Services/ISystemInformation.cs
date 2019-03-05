namespace Pact.Fhir.Core.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Entity;

  public interface ISystemInformation
  {
    Task<SystemInformation> GetSystemInformationAsync();
  }
}