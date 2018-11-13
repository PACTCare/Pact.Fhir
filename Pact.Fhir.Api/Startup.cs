namespace Pact.Fhir.Api
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Repository.MamStorage;
  using Pact.Fhir.Core.Serializer;
  using Pact.Fhir.Core.Services.Cache;
  using Pact.Fhir.Core.Usecase.GetResource;
  using Pact.Fhir.Core.Usecase.GetResourceHistory;
  using Pact.Fhir.Core.Usecase.GetResourceVersion;

  using RestSharp;

  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  /// <summary>
  /// The startup.
  /// </summary>
  public class Startup
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">
    /// The configuration.
    /// </param>
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// The configure.
    /// </summary>
    /// <param name="app">
    /// The app.
    /// </param>
    /// <param name="env">
    /// The env.
    /// </param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();
      app.UseCors("Development");
    }

    /// <summary>
    /// The configure services.
    /// </summary>
    /// <param name="services">
    /// The services.
    /// </param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddCors(
        options => options.AddPolicy("Development", builder => { builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod(); }));

      this.InjectDependencies(services);
    }

    /// <summary>
    /// The inject dependencies.
    /// </summary>
    /// <param name="services">
    /// The services.
    /// </param>
    private void InjectDependencies(IServiceCollection services)
    {
      var fhirRepository = new TangleFhirPatientRepository(
        new RestIotaRepository(
          new FallbackIotaClient(
            new List<string>
              {
                "https://peanut.iotasalad.org:14265",
                "http://node04.iotatoken.nl:14265",
                "http://node05.iotatoken.nl:16265",
                "https://nodes.thetangle.org:443",
                "http://iota1.heidger.eu:14265",
                "https://nodes.iota.cafe:443",
                "https://potato.iotasalad.org:14265",
                "https://durian.iotasalad.org:14265",
                "https://turnip.iotasalad.org:14265",
                "https://nodes.iota.fm:443",
                "https://tuna.iotasalad.org:14265",
                "https://iotanode2.jlld.at:443",
                "https://node.iota.moe:443",
                "https://wallet1.iota.town:443",
                "https://wallet2.iota.town:443",
                "http://node03.iotatoken.nl:15265",
                "https://node.iota-tangle.io:14265",
                "https://pow4.iota.community:443",
                "https://dyn.tangle-nodes.com:443",
                "https://pow5.iota.community:443",
              },
            5000),
          new PoWSrvService()),
        new FhirJsonTryteSerializer(),
        new MemoryCacheStatefulMam(),
        NetMode.Testnet);

      var fhirSerializer = new FhirJsonSerializer();
      var resourceCache = new InMemoryResourceCache();

      var resourceInteractor = new GetResourceInteractor(fhirRepository, resourceCache);

      services.AddTransient(_ => resourceInteractor);
      services.AddTransient(resourcePresenter => new GetResourcePresenter(fhirSerializer));

      services.AddTransient(resourceHistoryInteractor => new GetResourceHistoryInteractor(fhirRepository));
      services.AddTransient(resourceHistoryPresenter => new GetResourceHistoryPresenter(fhirSerializer));

      services.AddTransient(resourceVersionInteractor => new GetResourceVersionInteractor(fhirRepository, resourceCache));

      new TaskFactory().StartNew(() => new CacheJobQueue(resourceCache, resourceInteractor).Workload());
    }
  }
}