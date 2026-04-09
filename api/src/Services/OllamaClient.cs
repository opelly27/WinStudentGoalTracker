using System.Text.Json.Serialization;

namespace WinStudentGoalTracker.Services;

public class OllamaClient
{
    private readonly HttpClient _httpClient;
    public readonly string Model;

    public OllamaClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        Model = config["Ollama:Model"] ?? "gpt-oss:20b";
    }

    public async Task<string> ChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var requestBody = new OllamaChatRequest
        {
            Model = Model,
            Messages = [new OllamaChatMessage { Role = "user", Content = prompt }],
            Format = "json",
            Stream = false
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync("/api/chat", requestBody, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null)
        {
            throw new OllamaUnavailableException(
                $"The Ollama provider could not be reached at {_httpClient.BaseAddress}. Ensure the service is running.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Ollama service returned {(int)response.StatusCode}: {errorBody}",
                null,
                response.StatusCode);
        }

        var chatResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken)
            ?? throw new System.Text.Json.JsonException("Ollama returned an empty response.");

        return chatResponse.Message.Content;
    }

    #region Exceptions

    public class OllamaUnavailableException(string message, Exception? inner = null)
        : Exception(message, inner);

    #endregion

    #region Ollama API DTOs

    private class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OllamaChatMessage> Messages { get; set; } = [];

        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    private class OllamaChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaChatMessage Message { get; set; } = new();
    }

    #endregion
}
