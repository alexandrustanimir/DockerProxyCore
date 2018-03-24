using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DockerProxyCore
{
	public class AuthorizedRestClient
	{
		private readonly HttpClient _client;
		private readonly HttpClientHandler _clientHandler = new HttpClientHandler();

		public AuthorizedRestClient(IConfiguration configuration)
		{
			var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			certStore.Open(OpenFlags.ReadOnly);
			var certCollection = certStore.Certificates.Find(
				X509FindType.FindByThumbprint,
				configuration["CertificateThumbprint"],
				false);
			// Get the first cert with the thumbprint
			if (certCollection.Count > 0)
			{
				var cert = certCollection[0];
				_clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
				_clientHandler.SslProtocols = SslProtocols.Tls12;
				// disable this when using production certificate
				_clientHandler.ServerCertificateCustomValidationCallback = (message, c, chain, errors) => { return true; };
				_clientHandler.ClientCertificates.Add(cert);
			}

			_client = new HttpClient(_clientHandler)
			{
				BaseAddress = new Uri(configuration["BaseApiAddress"])
			};
		}

		public Task<HttpResponseMessage> PostJsonAsync(string uri, HttpContent data)
		{
			return InvokeAsync(
				client => client.PostAsync(uri, data));
		}

		private Task<HttpResponseMessage> InvokeAsync(Func<HttpClient, Task<HttpResponseMessage>> operation)
		{
			if (operation == null)
				throw new ArgumentNullException(nameof(operation));

			return operation(_client);
		}
	}
}