using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Rextur_Serverless.Models.Responses;

public class ManagementFieldListResponse
{
	[JsonProperty("chave")]
	public string Chave { get; set; }

	[JsonProperty("nomeCampo")]
	public string NomeCampo { get; set; }

	[JsonProperty("valor")]
	public string Valor { get; set; }
}
