namespace Pact.Fhir.Core.Usecase.GetCapabilities
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;

  public class GetCapabilitiesInteractor
  {
    public GetCapabilitiesInteractor(IFhirRepository repository, ISystemInformation systemInformation)
    {
      this.Repository = repository;
      this.SystemInformation = systemInformation;
    }

    private ISystemInformation SystemInformation { get; }

    private IFhirRepository Repository { get; }

    public async Task<CapabilityStatement> ExecuteAsync()
    {
      var systemInformation = await this.SystemInformation.GetSystemInformationAsync();

      return new CapabilityStatement
               {
                 Language = systemInformation.Language,
                 Url = systemInformation.Url,
                 Name = systemInformation.Name,
                 Status = systemInformation.Status,
                 Version = systemInformation.Version,
                 Experimental = systemInformation.Experimental,
                 Publisher = systemInformation.Publisher,
                 Contact = systemInformation.Contact,
                 Kind = systemInformation.Kind,
                 Software = systemInformation.Software,
                 FhirVersion = "3.0",
                 AcceptUnknown = CapabilityStatement.UnknownContentCode.Extensions,
                 Format = systemInformation.Format,
                 Rest = new List<CapabilityStatement.RestComponent>
                          {
                            new CapabilityStatement.RestComponent
                              {
                                Mode = CapabilityStatement.RestfulCapabilityMode.Server,
                                Resource = this.Repository.GetCapabilities(),
                                Interaction = new List<CapabilityStatement.SystemInteractionComponent>
                                                {
                                                  new CapabilityStatement.SystemInteractionComponent
                                                    {
                                                      Code = CapabilityStatement.SystemRestfulInteraction.Batch
                                                    }
                                                }
                              }
                          }
               };
    }
  }
}