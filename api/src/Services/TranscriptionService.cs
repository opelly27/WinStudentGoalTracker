using System.Net.Http.Headers;

namespace WinStudentGoalTracker.Services;

public class TranscriptionService
{
    private readonly HttpClient _httpClient;

    public TranscriptionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> TranscribeAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(audioStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent("whisper-1"), "model");
        content.Add(new StringContent("json"), "response_format");

        var response = await _httpClient.PostAsync("/v1/audio/transcriptions", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Transcription service returned {(int)response.StatusCode}: {errorBody}",
                null,
                response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<TranscriptionResult>(cancellationToken)
            ?? throw new InvalidOperationException("Transcription service returned an empty response.");

        return result.Text;
    }

    private record TranscriptionResult(string Text);
}
