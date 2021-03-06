﻿namespace Pact.Fhir.Iota.Tests.Utils
{
  using System.Collections.Generic;

  using Pact.Fhir.Iota.SqlLite.Services;

  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public static class IotaResourceProvider
  {
    public static IIotaRepository Repository =>
      new CachedIotaRestRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWService(new CpuPearlDiver()));
  }
}