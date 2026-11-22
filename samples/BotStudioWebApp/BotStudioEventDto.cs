namespace BotStudioWebApp;

public sealed record BotStudioEventDto(
    long Sequence,
    DateTimeOffset CapturedAtUtc,
    string Category,
    string Name,
    string Severity,
    uint GameLoop,
    string Message);
