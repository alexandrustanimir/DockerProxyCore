using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DockerProxyCore.Controllers
{
	[Route("api/Proxy")]
	public class ProxyController : Controller
	{
		private readonly AuthorizedRestClient _restClient;

		public ProxyController(IServiceProvider serviceProvider)
		{
			_restClient = serviceProvider.GetService(typeof(AuthorizedRestClient)) as AuthorizedRestClient;
		}

		[HttpPost]
		public async Task<object> Create([FromBody] object content)
		{
			var result = await _restClient.PostJsonAsync("paymentrequests", new JsonContent(content));

			if (result.StatusCode == HttpStatusCode.Created)
				return Ok(content);
			return BadRequest("Invalid request");
		}
	}
}