using Rextur_Serverless.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Rextur_Serverless.Handler;

public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
	private readonly IAuthService _service;

	public HttpClientAuthorizationDelegatingHandler(IAuthService service)
	{
		_service = service;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		string text = await _service.ObterToken();

		if (!string.IsNullOrEmpty(text))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", text);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
