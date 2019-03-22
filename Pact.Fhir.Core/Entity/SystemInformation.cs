namespace Pact.Fhir.Core.Entity
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public class SystemInformation
  {
    public string Language { get; set; }

    public string Url { get; set; }

    public string Name { get; set; }

    public PublicationStatus? Status { get; set; }

    public string Version { get; set; }

    public bool? Experimental { get; set; }

    public string Publisher { get; set; }

    public List<ContactDetail> Contact { get; set; }

    public CapabilityStatement.SoftwareComponent Software { get; set; }

    public CapabilityStatement.CapabilityStatementKind? Kind { get; set; }

    public IEnumerable<string> Format { get; set; }
  }
}