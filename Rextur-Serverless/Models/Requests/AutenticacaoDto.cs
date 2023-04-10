using System.Text.Json.Serialization;

namespace Rextur_Serverless.Models.Requests;
public class AutenticacaoDto
{
    [JsonPropertyName("username")]
    public string Username { get; private set; }

    [JsonPropertyName("password")]
    public string Password { get; private set; }

    public AutenticacaoDto(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
