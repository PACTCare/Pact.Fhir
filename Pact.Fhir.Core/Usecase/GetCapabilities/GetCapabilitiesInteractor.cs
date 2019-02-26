namespace Pact.Fhir.Core.Usecase.GetCapabilities
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  public class GetCapabilitiesInteractor
  {
    public GetCapabilitiesInteractor(IFhirRepository repository)
    {
      this.Repository = repository;
    }

    private IFhirRepository Repository { get; }

    public CapabilityStatement Execute()
    {
      return new CapabilityStatement
               {
                 Language = "en-EN",
                 Url = "metadata",
                 Name = "PACT Care Fhir API",
                 Status = PublicationStatus.Active,
                 Version = "0.0.1",
                 Experimental = true,
                 Publisher = "PACT Care BV",
                 Contact =
                   new List<ContactDetail>
                     {
                       new ContactDetail
                         {
                           Name = "PACT Care BV",
                           Telecom = new List<ContactPoint>
                                       {
                                         new ContactPoint(
                                           ContactPoint.ContactPointSystem.Email,
                                           ContactPoint.ContactPointUse.Work,
                                           "info@pact.care")
                                       }
                         }
                     },
                 Kind = CapabilityStatement.CapabilityStatementKind.Instance,
                 Software = new CapabilityStatement.SoftwareComponent { Name = "PACT Fhir", Version = "0.0.1" },
                 FhirVersion = "3.0",
                 AcceptUnknown = CapabilityStatement.UnknownContentCode.Extensions,
                 Format = new List<string> { "json" },
                 Rest = new List<CapabilityStatement.RestComponent>
                          {
                            new CapabilityStatement.RestComponent
                              {
                                Mode = CapabilityStatement.RestfulCapabilityMode.Server,
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
                                                },
                                Resource = this.Repository.GetCapabilities()
                              }
                          }
               };
    }
  }
}