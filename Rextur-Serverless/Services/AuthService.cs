using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rextur_Serverless.Interfaces;
using Rextur_Serverless.Models.Requests;
using Rextur_Serverless.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rextur_Serverless.Services;

public class AuthService : IAuthService
{
	private readonly HttpClient _client;

	public AuthService(IHttpClientFactory httpClientFactory)
	{
		_client = httpClientFactory.CreateClient("authReservaFacil");
	}

	public async Task<string> ObterToken()
	{
		var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ObterCredenciais()), Encoding.UTF8, "application/json");

		var response = await _client.PostAsync("authenticate", content);

		if (response.StatusCode != HttpStatusCode.OK)
		{
			string message = await response.Content.ReadAsStringAsync();
			throw new HttpRequestException($"Erro ao chamar API /authenticate - {message} - ");
		}

		return response.Headers
			.GetValues("Authorization")
			.FirstOrDefault()
			.Replace("Bearer", "").Trim();
	}

	private static AutenticacaoDto ObterCredenciais()
	{
		var username = Environment.GetEnvironmentVariable("USUARIO_RESERVAFACIL");
		var password = Environment.GetEnvironmentVariable("SENHA_RESERVAFACIL");

		return new AutenticacaoDto(username, password);
	}
}
