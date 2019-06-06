namespace Pact.Fhir.Iota.Repository
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using RestSharp;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class CachedIotaRestRepository : RestIotaRepository
  {
    /// <inheritdoc />
    public CachedIotaRestRepository(IRestClient client, IPoWService powService = null, string username = null, string password = null)
      : base(client, powService, username, password)
    {
    }

    /// <inheritdoc />
    public CachedIotaRestRepository(IIotaClient client, IPoWService powService, IAddressGenerator addressGenerator = null)
      : base(client, powService, addressGenerator)
    {
    }

    /// <inheritdoc />
    public override Task<List<TransactionTrytes>> GetTrytesAsync(List<Hash> hashes)
    {
      return base.GetTrytesAsync(hashes);
    }

    /// <inheritdoc />
    public override Task StoreTransactionsAsync(IEnumerable<TransactionTrytes> transactions)
    {
      return base.StoreTransactionsAsync(transactions);
    }
  }
}