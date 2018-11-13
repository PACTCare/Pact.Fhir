namespace Pact.Fhir.Api.Controllers
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Cors;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Authentication;
  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Core.Usecase.GetResource;
  using Pact.Fhir.Core.Usecase.GetResourceHistory;
  using Pact.Fhir.Core.Usecase.GetResourceVersion;

  using Tangle.Net.Entity;

  /// <summary>
  ///   The v 1 controller.
  /// </summary>
  [EnableCors("Development")]
  [BasicAuthentication]
  public class FhirController : Controller
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirController"/> class.
    /// </summary>
    /// <param name="resourceInteractor">
    /// The resource interactor.
    /// </param>
    /// <param name="resourcePresenter">
    /// The resource Presenter.
    /// </param>
    /// <param name="resourceHistoryInteractor">
    /// The resource History Interactor.
    /// </param>
    /// <param name="resourceHistoryPresenter">
    /// The resource History Presenter.
    /// </param>
    /// <param name="resourceVersionInteractor">
    /// The resource Version Interactor.
    /// </param>
    public FhirController(
      GetResourceInteractor resourceInteractor,
      GetResourcePresenter resourcePresenter,
      GetResourceHistoryInteractor resourceHistoryInteractor,
      GetResourceHistoryPresenter resourceHistoryPresenter,
      GetResourceVersionInteractor resourceVersionInteractor)
    {
      this.ResourceInteractor = resourceInteractor;
      this.ResourcePresenter = resourcePresenter;
      this.ResourceHistoryInteractor = resourceHistoryInteractor;
      this.ResourceHistoryPresenter = resourceHistoryPresenter;
      this.ResourceVersionInteractor = resourceVersionInteractor;
    }

    /// <summary>
    /// Gets the resource history interactor.
    /// </summary>
    private GetResourceHistoryInteractor ResourceHistoryInteractor { get; }

    /// <summary>
    /// Gets the resource history presenter.
    /// </summary>
    private GetResourceHistoryPresenter ResourceHistoryPresenter { get; }

    /// <summary>
    /// Gets the resource interactor.
    /// </summary>
    private GetResourceInteractor ResourceInteractor { get; }

    /// <summary>
    /// Gets the resource presenter.
    /// </summary>
    private GetResourcePresenter ResourcePresenter { get; }

    /// <summary>
    /// Gets the resource version interactor.
    /// </summary>
    private GetResourceVersionInteractor ResourceVersionInteractor { get; }

    /// <summary>
    /// The load medication data.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/medicationadministration/{root}")]
    public async Task<IActionResult> LoadMedicationData([FromRoute] string root)
    {
      var response = await this.ResourceInteractor.ExecuteAsync<MedicationAdministration>(new GetResourceRequest { Root = new Hash(root) });

      return this.ResourcePresenter.Present(response);
    }


    /// <summary>
    /// The load medication data.
    /// </summary>
    /// <param name="resourceId">
    /// The resource Id.
    /// </param>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/medicationadministration/{resourceId}/_history/{root}")]
    public async Task<IActionResult> LoadMedicationDataVersion([FromRoute] string resourceId, [FromRoute] string root)
    {
      var response = await this.ResourceVersionInteractor.ExecuteAsync<MedicationAdministration>(
                       new GetResourceVersionRequest { Root = new Hash(root), ResourceRoot = new Hash(resourceId) });

      return this.ResourcePresenter.Present(response);
    }

    /// <summary>
    /// The load medication history.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="System.Threading.Tasks.Task"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/medicationadministration/{root}/_history")]
    public async Task<IActionResult> LoadMedicationHistory([FromRoute] string root)
    {
      var response = await this.ResourceHistoryInteractor.ExecuteAsync<MedicationAdministration>(new GetResourceRequest { Root = new Hash(root) });

      return this.ResourceHistoryPresenter.Present(response);
    }

    /// <summary>
    /// The load patient data.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/patient/{root}")]
    public async Task<IActionResult> LoadPatientData([FromRoute] string root)
    {
      var response = await this.ResourceInteractor.ExecuteAsync<Patient>(new GetResourceRequest { Root = new Hash(root) });

      return this.ResourcePresenter.Present(response);
    }

    /// <summary>
    /// The load patient data.
    /// </summary>
    /// <param name="resourceId">
    /// The resource Id.
    /// </param>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/patient/{resourceId}/_history/{root}")]
    public async Task<IActionResult> LoadPatientDataVersion([FromRoute] string resourceId, [FromRoute] string root)
    {
      var response = await this.ResourceVersionInteractor.ExecuteAsync<Patient>(
                       new GetResourceVersionRequest { ResourceRoot = new Hash(resourceId), Root = new Hash(root) });

      return this.ResourcePresenter.Present(response);
    }

    /// <summary>
    /// The load patient history.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="System.Threading.Tasks.Task"/>.
    /// </returns>
    [HttpGet]
    [Route("fhir/v1/patient/{root}/_history")]
    public async Task<IActionResult> LoadPatientHistory([FromRoute] string root)
    {
      var response = await this.ResourceHistoryInteractor.ExecuteAsync<Patient>(new GetResourceRequest { Root = new Hash(root) });

      return this.ResourceHistoryPresenter.Present(response);
    }
  }
}