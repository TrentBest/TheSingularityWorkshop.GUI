namespace BotStudioWebApp;

public sealed record BotStudioSnapshotDto(
    long Sequence,
    DateTimeOffset CapturedAtUtc,
    string Lifecycle,
    string MapName,
    uint GameLoop,
    string ActualRace,
    string ActiveModeId,
    string GamePhase,
    uint Minerals,
    uint Vespene,
    uint FoodUsed,
    uint FoodCap,
    int VisibleFriendlyUnits,
    int VisibleEnemyUnits,
    int VisibleWorkers,
    int BuildJobCount,
    int StrategicJobCount);
