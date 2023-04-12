using Rextur_Serverless.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rextur_Serverless.Interfaces;

public interface IReservaFacilService
{
	Task<IEnumerable<TicketsResponse>> GetTickets(string requestDate);
}
