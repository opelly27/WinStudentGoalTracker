using System.Text.Json;
using System.Text.Json.Serialization;
using WinStudentGoalTracker.Models.ResponseTypes;

namespace WinStudentGoalTracker.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly int _maxRetries = 3;

    public OllamaService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _model = config["Ollama:Model"] ?? "gpt-oss:20b";
    }

    public async Task<GoalBreakdownResponse> BreakdownGoalAsync(string goal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(goal))
            throw new ArgumentException("Goal cannot be empty.", nameof(goal));

        var prompt = "You are a task planning assistant. Given a goal, break it down into smaller, actionable subgoals.\n\n" +
            $"Goal: {goal}\n\n" +
            "Respond with ONLY a JSON object in this exact format, no other text:\n" +
            "{\"subgoals\": [\"subgoal 1\", \"subgoal 2\", \"subgoal 3\"]}\n\n" +
            "Be specific and practical. Generate 3-7 subgoals depending on complexity.";

        var requestBody = new OllamaChatRequest
        {
            Model = _model,
            Messages = [new OllamaChatMessage { Role = "user", Content = prompt }],
            Format = "json",
            Stream = false
        };

        for (var attempt = 0; attempt < _maxRetries; attempt++)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/chat", requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"Ollama service returned {(int)response.StatusCode}: {errorBody}",
                    null,
                    response.StatusCode);
            }

            try
            {
                var chatResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken)
                    ?? throw new JsonException("Ollama returned an empty response.");

                var parsed = JsonSerializer.Deserialize<OllamaSubgoalsResult>(chatResponse.Message.Content)
                    ?? throw new JsonException("LLM response deserialized to null.");

                var subgoals = NormalizeSubgoals(parsed.Subgoals);

                if (subgoals.Count == 0)
                    throw new JsonException("LLM response contained no valid subgoals.");

                return new GoalBreakdownResponse
                {
                    Goal = goal,
                    Subgoals = subgoals
                };
            }
            catch (JsonException) when (attempt < _maxRetries - 1)
            {
                // Malformed response from the model — retry
            }
        }

        throw new InvalidOperationException(
            $"Failed to get a valid response from the LLM after {_maxRetries} attempts.");
    }

    private static List<string> NormalizeSubgoals(List<JsonElement> rawSubgoals)
    {
        var normalized = new List<string>();

        foreach (var item in rawSubgoals)
        {
            switch (item.ValueKind)
            {
                case JsonValueKind.String:
                    normalized.Add(item.GetString()!);
                    break;

                case JsonValueKind.Object:
                    string[] preferredKeys = ["description", "task", "subtask", "subgoal", "name", "action"];
                    var added = false;

                    foreach (var key in preferredKeys)
                    {
                        if (item.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
                        {
                            normalized.Add(value.GetString()!);
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                    {
                        foreach (var prop in item.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.String)
                            {
                                normalized.Add(prop.Value.GetString()!);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        return normalized;
    }

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

    private class OllamaSubgoalsResult
    {
        [JsonPropertyName("subgoals")]
        public List<JsonElement> Subgoals { get; set; } = [];
    }

    #endregion
}
