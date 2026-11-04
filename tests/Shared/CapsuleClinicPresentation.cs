using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Tests.Shared;

/// <summary>
/// Deterministic, test-only Experience state and semantic presentation fixture.
/// The fixture deliberately owns no puzzle rules: callers supply the state snapshot.
/// </summary>
internal sealed record CapsuleClinicSnapshot(
    int Score,
    int Level,
    int Progress,
    string Player,
    string Room,
    bool Connected,
    bool Paused,
    string? Error = null);

internal static class CapsuleClinicPresentation
{
    public static GuiNode Build(CapsuleClinicSnapshot state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var root = GuiBuilders.Panel("experience.capsule-clinic")
            .Property("role", "application")
            .Child("Text", "experience.title", title => title.Text("Capsule Clinic"))
            .Child("Panel", "experience.status", status => status
                .Child("Text", "status.player", player => player.Text($"Player: {state.Player}"))
                .Child("Text", "status.room", room => room.Text($"Room: {state.Room}"))
                .Child("Text", "status.connection", connection => connection.Text(
                    state.Connected ? "Connected" : "Disconnected")))
            .Child("Panel", "experience.metrics", metrics => metrics
                .Child("Text", "metrics.score", score => score.Text($"Score: {state.Score}"))
                .Child("Text", "metrics.level", level => level.Text($"Level: {state.Level}"))
                .Child("Text", "metrics.progress", progress => progress.Text($"Progress: {state.Progress}%")))
            .Child("Panel", "experience.board", board => board
                .Property("role", "grid")
                .Child("Button", "board.cell.0.0", cell => cell.Text("A").Property("aria-label", "Select board cell row 1 column 1").WithOnClick("capsule.cell.select.0.0"))
                .Child("Button", "board.cell.0.1", cell => cell.Text("B").Property("aria-label", "Select board cell row 1 column 2").WithOnClick("capsule.cell.select.0.1"))
                .Child("Button", "board.cell.1.0", cell => cell.Text("C").Property("aria-label", "Select board cell row 2 column 1").WithOnClick("capsule.cell.select.1.0"))
                .Child("Button", "board.cell.1.1", cell => cell.Text("D").Property("aria-label", "Select board cell row 2 column 2").WithOnClick("capsule.cell.select.1.1")))
            .Child("Panel", "experience.preview", preview => preview
                .Child("Text", "preview.heading", heading => heading.Text("Next capsule"))
                .Child("Text", "preview.value", value => value.Text("Red / Blue")))
            .Child("Panel", "experience.controls", controls => controls
                .Child("Button", "control.pause", button => button
                    .Text(state.Paused ? "Resume" : "Pause")
                    .WithOnClick(state.Paused ? "capsule.resume" : "capsule.pause"))
                .Child("Button", "control.restart", button => button.Text("Restart").WithOnClick("capsule.restart"))
                .Child("Button", "control.rematch", button => button.Text("Rematch").WithOnClick("capsule.rematch")))
            .Child("Text", "experience.instructions",
                instructions => instructions.Text("Use arrow keys or a controller to navigate; activate a cell to select it."))
            .Child("Text", "experience.error",
                error => error.Text(state.Error ?? string.Empty))
            .Build();

        return root;
    }
}
