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

public class ReservaFacilService : IReservaFacilService
{
	private readonly HttpClient _client;

	public ReservaFacilService(IHttpClientFactory httpClientFactory)
	{
		_client = httpClientFactory.CreateClient("ticketsReservafacil");
	}

	public async Task<IEnumerable<TicketsResponse>> GetTickets(string requestDate)
	{
		int idAgencia = Convert.ToInt32(Environment.GetEnvironmentVariable("ID_AGENCIA"));
		bool isCancel = false;

		string url = $"tickets/agencia/{idAgencia}/{isCancel}/{requestDate}";

		var response = await _client.GetAsync(url);

		if (!response.IsSuccessStatusCode)
			throw new HttpRequestException($"Erro ao chamar API /tickets/agencia - ");

		var tickets = JsonConvert.DeserializeObject<IEnumerable<TicketsResponse>>(await response.Content.ReadAsStringAsync());

		return tickets;
	}
}
