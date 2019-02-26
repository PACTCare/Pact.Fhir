namespace Pact.Fhir.Api
{
  using System.Collections.Generic;

  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Iota.SqlLite.Services;

  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      this.InjectDependencies(services);
    }

    private void InjectDependencies(IServiceCollection services)
    {
      var iotaRepository = new RestIotaRepository(
        new FallbackIotaClient(
          new List<string>
            {
              "https://trinity.iota-tangle.io:14265",
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
              "http://node04.iotatoken.nl:14265",
              "http://node05.iotatoken.nl:16265",
            },
          5000),
        new PoWSrvService());

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);
      var fhirRepository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        new SqlLiteResourceTracker(channelFactory, subscriptionFactory, new RijndaelEncryption("somenicekey", "somenicesalt")),
        new RandomChannelCredentialProvider());

      var createInteractor = new CreateResourceInteractor(fhirRepository, new FhirJsonParser());
      var readInteractor = new ReadResourceInteractor(fhirRepository);
      var capabilitiesInteractor = new GetCapabilitiesInteractor(fhirRepository);

      services.AddSingleton(createInteractor);
      services.AddSingleton(readInteractor);
      services.AddSingleton(capabilitiesInteractor);
    }
  }
}