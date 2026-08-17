using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace WinStudentGoalTracker.Services;

/// <summary>
/// LLM chat client backed by OpenRouter's OpenAI-compatible chat completions API.
/// </summary>
public class OpenRouterClient
{
    private const string DefaultBaseUrl = "https://openrouter.ai/api/v1/";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string? _referer;
    private readonly string? _title;
    public readonly string Model;

    public OpenRouterClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        Model = config["OpenRouter:Model"] ?? "openai/gpt-4o-mini";

        // Matches the project's .env convention, falling back to appsettings.
        _apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
            ?? config["OpenRouter:ApiKey"]
            ?? string.Empty;

        // Optional attribution headers OpenRouter uses for app rankings.
        _referer = config["OpenRouter:SiteUrl"];
        _title = config["OpenRouter:SiteName"];

        // Self-sufficient if registered with a bare AddHttpClient<OpenRouterClient>().
        // The trailing slash matters: without it, Uri resolution drops the /api/v1 segment.
        var baseUrl = _httpClient.BaseAddress?.ToString() ?? config["OpenRouter:BaseUrl"] ?? DefaultBaseUrl;
        _httpClient.BaseAddress = new Uri(baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/");
    }

    public async Task<string> ChatAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new OpenRouterUnavailableException(
                "No OpenRouter API key configured. Set OPENROUTER_API_KEY in .env or OpenRouter:ApiKey in configuration.");

        var requestBody = new OpenRouterChatRequest
        {
            Model = Model,
            Messages = [new OpenRouterChatMessage { Role = "user", Content = prompt }],
            ResponseFormat = new OpenRouterResponseFormat { Type = "json_object" },
            Stream = false
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        if (!string.IsNullOrWhiteSpace(_referer))
            request.Headers.TryAddWithoutValidation("HTTP-Referer", _referer);

        if (!string.IsNullOrWhiteSpace(_title))
            request.Headers.TryAddWithoutValidation("X-Title", _title);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null)
        {
            throw new OpenRouterUnavailableException(
                $"The OpenRouter provider could not be reached at {_httpClient.BaseAddress}. Check network connectivity.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"OpenRouter service returned {(int)response.StatusCode}: {errorBody}",
                null,
                response.StatusCode);
        }

        var chatResponse = await response.Content.ReadFromJsonAsync<OpenRouterChatResponse>(cancellationToken)
            ?? throw new System.Text.Json.JsonException("OpenRouter returned an empty response.");

        // OpenRouter can report upstream provider failures in a 200 body.
        if (chatResponse.Error is not null)
            throw new OpenRouterUnavailableException(
                $"OpenRouter returned an error for model '{Model}': {chatResponse.Error.Message}");

        var content = chatResponse.Choices.FirstOrDefault()?.Message.Content;

        if (string.IsNullOrWhiteSpace(content))
            throw new System.Text.Json.JsonException("OpenRouter returned no message content.");

        return content;
    }

    #region Exceptions

    public class OpenRouterUnavailableException(string message, Exception? inner = null)
        : Exception(message, inner);

    #endregion

    #region OpenRouter API DTOs

    private class OpenRouterChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OpenRouterChatMessage> Messages { get; set; } = [];

        [JsonPropertyName("response_format")]
        public OpenRouterResponseFormat? ResponseFormat { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    private class OpenRouterResponseFormat
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }

    private class OpenRouterChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class OpenRouterChatChoice
    {
        [JsonPropertyName("message")]
        public OpenRouterChatMessage Message { get; set; } = new();
    }

    private class OpenRouterChatResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenRouterChatChoice> Choices { get; set; } = [];

        [JsonPropertyName("error")]
        public OpenRouterError? Error { get; set; }
    }

    private class OpenRouterError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    #endregion
}
