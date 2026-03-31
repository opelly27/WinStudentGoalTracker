using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.Services;

public static class ProgressReportBuilder
{
    // *****************************************************************
    // Builds a markdown document from a StudentProgressReportResponse.
    // Returns the complete markdown string ready for download.
    // *****************************************************************
    public static string BuildMarkdown(StudentProgressReportResponse report, DateTime fromDate, DateTime toDate)
    {
        var lines = new List<string>();

        var fromDisplay = fromDate.ToString("MMMM d, yyyy");
        var toDisplay = toDate.ToString("MMMM d, yyyy");

        lines.Add("# Student Progress Report");
        lines.Add("");
        lines.Add($"**Student:** {report.StudentIdentifier}");
        lines.Add($"**Report Period:** {fromDisplay} – {toDisplay}");
        lines.Add("");
        lines.Add("---");

        if (report.Goals.Count == 0)
        {
            lines.Add("");
            lines.Add("*No progress events found in the selected date range.*");
            return string.Join("\n", lines);
        }

        var goalIndex = 0;
        foreach (var goal in report.Goals)
        {
            goalIndex++;
            lines.Add("");
            lines.Add($"## {goalIndex}. {goal.Category}");
            if (!string.IsNullOrWhiteSpace(goal.Description))
            {
                lines.Add("");
                lines.Add(goal.Description);
            }
            lines.Add("");

            foreach (var ev in goal.ProgressEvents)
            {
                var eventDate = ev.CreatedAt?.ToString("MMMM d, yyyy") ?? "Unknown date";
                lines.Add($"### {eventDate}");
                lines.Add("");
                lines.Add(ev.Content ?? "");

                if (!string.IsNullOrWhiteSpace(ev.BenchmarkNames))
                {
                    lines.Add("");
                    lines.Add($"**Benchmarks:** {ev.BenchmarkNames}");
                }
                lines.Add("");
            }

            lines.Add("---");
        }

        return string.Join("\n", lines);
    }
}
