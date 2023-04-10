using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rextur_Serverless.Models.Requests;
using Rextur_Serverless.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rextur_Serverless;


public static class Rextur
{
	[FunctionName("Rextur")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("C# HTTP trigger function processed a request.");

		string requestDate = req.Query["requestDate"];

		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);
		requestDate ??= data?.requestDate;

		if(string.IsNullOrEmpty(requestDate))
			return new BadRequestObjectResult("This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.");

		string token = await ObterToken(ObterCredenciais(), log);

		var tickets = await GetTickets(requestDate, token, log);

		return new OkObjectResult(tickets);
	}

	private static AutenticacaoDto ObterCredenciais()
	{
		var username = Environment.GetEnvironmentVariable("USUARIO_RESERVAFACIL");
		var password = Environment.GetEnvironmentVariable("SENHA_RESERVAFACIL");

		return new AutenticacaoDto(username, password);
	}

	private static async Task<string> ObterToken(AutenticacaoDto dto, ILogger log)
	{
		HttpClient _httpClient = new()
		{
			BaseAddress = new Uri(Environment.GetEnvironmentVariable("URL_RESERVAFACIL"))
		};

		var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

		try
		{
			var response = await _httpClient.PostAsync("authenticate", content);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new HttpRequestException(message);
			}

			return response.Headers
				.GetValues("Authorization")
				.FirstOrDefault()
				.Replace("Bearer", "").Trim();

		}
		catch (Exception ex)
		{
			log.LogError($"Erro ao chamar API /authenticate - Mensagem de erro: {ex.Message}");
			return string.Empty;
		}

	}

	public static async Task<IEnumerable<TicketsResponse>> GetTickets(string requestDate, string token, ILogger log)
	{
		HttpClient _httpClient = new()
		{
			BaseAddress = new Uri(Environment.GetEnvironmentVariable("URL_RESERVAFACIL"))
		};

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		int idAgencia = Convert.ToInt32(Environment.GetEnvironmentVariable("ID_AGENCIA"));
		bool isCancel = false;

		string url = $"tickets/agencia/{idAgencia}/{isCancel}/{requestDate}";
		
		try
		{

			var response = await _httpClient.GetAsync(url);

			if (!response.IsSuccessStatusCode)
				throw new Exception();

			var tickets = JsonConvert.DeserializeObject<IEnumerable<TicketsResponse>>(await response.Content.ReadAsStringAsync());

			return tickets;
		}
		catch (Exception ex)
		{
			log.LogError($"Erro ao chamar API /tickets/agencia - Mensagem de erro: {ex.Message}");

			return Enumerable.Empty<TicketsResponse>();
		}
	}
}
