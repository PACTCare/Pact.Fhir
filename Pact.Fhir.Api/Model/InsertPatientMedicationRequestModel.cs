namespace Pact.Fhir.Api.Model
{
  /// <summary>
  /// The insert patient medication request model.
  /// </summary>
  public class InsertPatientMedicationRequestModel
  {
    /// <summary>
    /// Gets or sets the medication root.
    /// </summary>
    public string MedicationRoot { get; set; }

    /// <summary>
    /// Gets or sets the patient id.
    /// </summary>
    public int PatientId { get; set; }
  }
}