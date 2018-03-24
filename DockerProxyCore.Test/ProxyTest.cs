using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DockerProxyCore.Test
{
    [TestClass]
    public class ProxyTest
    {
	    private TestServer _server;
	    private HttpClient _client;

		[TestInitialize]
	    public void Init()
	    {
			// Arrange
		    var configuration = new ConfigurationBuilder()
			    .AddJsonFile("appsettings.Development.json", optional: false)
			    .Build();
			_server = new TestServer(new WebHostBuilder()
				.UseConfiguration(configuration)
			    .UseStartup<Startup>());
		    _client = _server.CreateClient();
		}

		[TestMethod]
        public void CreatePaymentRequest_Request200Successfull()
		{
			//given an example of payment request
			string content =
				"{ \"payeePaymentReference\": \"0123456789\", \"callbackUrl\": \"https://example.com/api/swishcb/paymentrequests\", \"payerAlias\": \"4671234768\", \"payeeAlias\": \"1231181189\", \"amount\": \"100\", \"currency\": \"SEK\", \"message\": \"Kingston USB Flash Drive 8 GB\" }";

			// when submiting the request
			var response =_client.PostAsync(_server.BaseAddress + "api/Proxy/", new JsonContent(content)).Result;

			// 
			Assert.IsTrue(response.IsSuccessStatusCode);

		}
    }
}
