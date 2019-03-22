namespace Pact.Fhir.Api.Services
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Microsoft.Extensions.Configuration;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Services;

  public class AppConfigSystemInformation : ISystemInformation
  {
    public AppConfigSystemInformation(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    /// <inheritdoc />
    public async Task<SystemInformation> GetSystemInformationAsync()
    {
      var systemConfiguration = this.Configuration.GetSection("SystemInformation");

      return new SystemInformation
               {
                 Language = systemConfiguration.GetValue<string>("Language"),
                 Url = systemConfiguration.GetValue<string>("Url"),
                 Name = systemConfiguration.GetValue<string>("Name"),
                 Status = (PublicationStatus)Enum.Parse(typeof(PublicationStatus), systemConfiguration.GetValue<string>("Status")),
                 Version = systemConfiguration.GetValue<string>("Version"),
                 Experimental = systemConfiguration.GetValue<bool>("Experimental"),
                 Publisher = systemConfiguration.GetValue<string>("Publisher"),
                 Software =
                   new CapabilityStatement.SoftwareComponent
                     {
                       Name = systemConfiguration.GetValue<string>("Name"), Version = systemConfiguration.GetValue<string>("Version")
                     },
                 Format = new List<string> { systemConfiguration.GetValue<string>("Format") },
                 Contact = new List<ContactDetail>
                             {
                               new ContactDetail
                                 {
                                   Name = systemConfiguration.GetValue<string>("Publisher"),
                                   Telecom = new List<ContactPoint>
                                               {
                                                 new ContactPoint(
                                                   ContactPoint.ContactPointSystem.Email,
                                                   ContactPoint.ContactPointUse.Work,
                                                   systemConfiguration.GetValue<string>("Email"))
                                               }
                                 }
                             },
                 Kind = CapabilityStatement.CapabilityStatementKind.Instance,
               };
    }
  }
}