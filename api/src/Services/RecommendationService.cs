using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Models.ResponseTypes;

namespace WinStudentGoalTracker.Services;

public class RecommendationService
{
    private readonly OllamaClient _ollamaClient;
    private readonly int _maxRetries = 3;

    public RecommendationService(OllamaClient ollamaClient)
    {
        _ollamaClient = ollamaClient;
    }

    // -------------------------------------------------------------------------
    // Goal breakdown
    // -------------------------------------------------------------------------

    public async Task<GoalBreakdownResponse> BreakdownGoalAsync(string goal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(goal))
            throw new ArgumentException("Goal cannot be empty.", nameof(goal));

        var prompt = "You are a task planning assistant. Given a goal, break it down into smaller, actionable subgoals.\n\n" +
            $"Goal: {goal}\n\n" +
            "Respond with ONLY a JSON object in this exact format, no other text:\n" +
            "{\"subgoals\": [\"subgoal 1\", \"subgoal 2\", \"subgoal 3\"]}\n\n" +
            "Be specific and practical. Generate 3-7 subgoals depending on complexity.";

        for (var attempt = 0; attempt < _maxRetries; attempt++)
        {
            try
            {
                var content = await _ollamaClient.ChatAsync(prompt, cancellationToken);

                var parsed = JsonSerializer.Deserialize<OllamaSubgoalsResult>(content)
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

    // -------------------------------------------------------------------------
    // Benchmark recommendation
    // -------------------------------------------------------------------------

    public async Task<BenchmarkRecommendationResponse> RecommendBenchmarkAsync(
        StudentFullProfileResponse profile,
        Guid goalId,
        CancellationToken cancellationToken = default)
    {
        var goal = profile.Goals.FirstOrDefault(g => g.GoalId == goalId)
            ?? throw new ArgumentException($"Goal {goalId} not found in student profile.", nameof(goalId));

        var existingBenchmarks = profile.Benchmarks
            .Where(b => b.GoalId == goalId)
            .ToList();

        var progressEvents = profile.ProgressEvents
            .Where(e => e.GoalId == goalId)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();

        var prompt = BuildBenchmarkPrompt(goal, existingBenchmarks, progressEvents);
        return await FetchBenchmarkRecommendationAsync(prompt, cancellationToken);
    }

    private static string BuildBenchmarkPrompt(
        StudentGoalItem goal,
        List<StudentBenchmarkItem> existingBenchmarks,
        List<ProgressEventWithGoalResponse> progressEvents)
    {
        var sb = new StringBuilder();

        sb.AppendLine("You are an educational specialist assisting with student IEP (Individualized Education Program) goal planning.");
        sb.AppendLine();
        sb.AppendLine("A benchmark is a measurable subgoal or step that takes a student closer to achieving their overall goal.");
        sb.AppendLine();

        sb.AppendLine("## Goal Details");
        sb.AppendLine($"Category: {goal.Category ?? "Not specified"}");
        sb.AppendLine($"Description: {goal.Description ?? "Not specified"}");
        sb.AppendLine($"Baseline: {goal.Baseline ?? "Not specified"}");

        if (existingBenchmarks.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("## Existing Benchmarks");
            sb.AppendLine("The following benchmarks have already been set for this goal. Your recommendation must be distinct from all of these, and should match their style and level of specificity:");
            for (var i = 0; i < existingBenchmarks.Count; i++)
            {
                var bm = existingBenchmarks[i];
                var label = string.IsNullOrWhiteSpace(bm.ShortName) ? $"Benchmark {i + 1}" : bm.ShortName;
                sb.AppendLine($"{i + 1}. [{label}] {bm.Benchmark}");
            }
        }

        if (progressEvents.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("## Recent Progress Notes");
            sb.AppendLine("These notes describe the student's recent progress toward this goal (most recent first):");
            foreach (var evt in progressEvents.Take(5))
            {
                var date = evt.CreatedAt?.ToString("MMM d, yyyy") ?? "Unknown date";
                sb.AppendLine($"- [{date}] {evt.Content}");
            }
        }

        sb.AppendLine();

        if (existingBenchmarks.Count > 0)
        {
            sb.AppendLine("Generate a new benchmark that:");
            sb.AppendLine("- Is measurable and specific");
            sb.AppendLine("- Is clearly distinct from the existing benchmarks listed above");
            sb.AppendLine("- Matches the style and format of the existing benchmarks");
            sb.AppendLine("- Represents a meaningful next step toward the overall goal");
        }
        else
        {
            sb.AppendLine("Generate a first benchmark for this goal that:");
            sb.AppendLine("- Is measurable and specific");
            sb.AppendLine("- Represents an achievable initial step toward the overall goal");
            sb.AppendLine("- Is grounded in the student's baseline performance described above");
        }

        sb.AppendLine();
        sb.AppendLine("Respond with ONLY a JSON object in this exact format, no other text:");
        sb.AppendLine("{\"benchmark\": \"<full benchmark text>\", \"short_name\": \"<brief label, 2-5 words>\"}");

        return sb.ToString();
    }

    private async Task<BenchmarkRecommendationResponse> FetchBenchmarkRecommendationAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < _maxRetries; attempt++)
        {
            try
            {
                var content = await _ollamaClient.ChatAsync(prompt, cancellationToken);

                var parsed = JsonSerializer.Deserialize<OllamaBenchmarkResult>(content)
                    ?? throw new JsonException("LLM response deserialized to null.");

                if (string.IsNullOrWhiteSpace(parsed.Benchmark))
                    throw new JsonException("LLM response contained no valid benchmark text.");

                if (string.IsNullOrWhiteSpace(parsed.ShortName))
                    throw new JsonException("LLM response contained no valid short name.");

                return new BenchmarkRecommendationResponse
                {
                    Benchmark = parsed.Benchmark.Trim(),
                    ShortName = parsed.ShortName.Trim()
                };
            }
            catch (JsonException) when (attempt < _maxRetries - 1)
            {
                // Malformed response from the model — retry
            }
        }

        throw new InvalidOperationException(
            $"Failed to get a valid benchmark recommendation from the LLM after {_maxRetries} attempts.");
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------
    // Private DTOs
    // -------------------------------------------------------------------------

    private class OllamaSubgoalsResult
    {
        [JsonPropertyName("subgoals")]
        public List<JsonElement> Subgoals { get; set; } = [];
    }

    private class OllamaBenchmarkResult
    {
        [JsonPropertyName("benchmark")]
        public string Benchmark { get; set; } = string.Empty;

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = string.Empty;
    }
}
