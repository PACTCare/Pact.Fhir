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
                 Language = systemInformation.Language, //"en-EN",
                 Url = systemInformation.Url, //"metadata",
                 Name = systemInformation.Name, //"PACT Care Fhir API",
                 Status = systemInformation.Status, // PublicationStatus.Active,
                 Version = systemInformation.Version, // "0.0.1",
                 Experimental = systemInformation.Experimental, // true,
                 Publisher = systemInformation.Publisher, // "PACT Care BV",
                 Contact = systemInformation.Contact,
                   //new List<ContactDetail>
                   //  {
                   //    new ContactDetail
                   //      {
                   //        Name = "PACT Care BV",
                   //        Telecom = new List<ContactPoint>
                   //                    {
                   //                      new ContactPoint(
                   //                        ContactPoint.ContactPointSystem.Email,
                   //                        ContactPoint.ContactPointUse.Work,
                   //                        "info@pact.care")
                   //                    }
                   //      }
                   //  },
                 Kind = systemInformation.Kind, //CapabilityStatement.CapabilityStatementKind.Instance,
                 Software = systemInformation.Software, //  new CapabilityStatement.SoftwareComponent { Name = "PACT Fhir", Version = "0.0.1" },
                 FhirVersion = "3.0",
                 AcceptUnknown = CapabilityStatement.UnknownContentCode.Extensions,
                 Format = systemInformation.Format, // new List<string> { "json" },
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
                                                    },
                                                  new CapabilityStatement.SystemInteractionComponent
                                                    {
                                                      Code = CapabilityStatement.SystemRestfulInteraction.Transaction
                                                    },
                                                }
                              }
                          }
               };
    }
  }
}