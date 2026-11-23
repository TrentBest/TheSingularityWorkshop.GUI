using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgSemanticControlRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void Text_input_is_manifested_as_a_static_accessible_field_preview()
    {
        var node = GuiBuilders.TextInput(
            "pilot-name",
            new TextInputDefinition("Pilot", "bot.name.changed", "Enter a name", "Required"))
            .Text("Pilot name")
            .Build();

        var document = XDocument.Parse(new SvgGuiRenderer().Render(node));
        var group = document.Descendants(Svg + "g")
            .Single(element => (string?)element.Attribute("data-control-kind") == "text-input");

        Assert.Equal("Pilot name", (string?)group.Attribute("aria-label"));
        Assert.Contains(group.Descendants(Svg + "text"), element => element.Value == "Pilot");
        Assert.Contains(group.Descendants(Svg + "text"), element => element.Value == "Required");
    }

    [Fact]
    public void Select_preview_displays_the_selected_option_label_not_its_internal_id()
    {
        var node = GuiBuilders.Select("mode", new SelectDefinition(
            new[] { new GuiSelectOption("observe", "Observe"), new GuiSelectOption("execute", "Execute") },
            "execute",
            "mode.changed")).Build();

        var document = XDocument.Parse(new SvgGuiRenderer().Render(node));
        var group = document.Descendants(Svg + "g")
            .Single(element => (string?)element.Attribute("data-control-kind") == "select");

        Assert.Contains(group.Descendants(Svg + "text"), element => element.Value == "Execute");
        Assert.DoesNotContain(group.Descendants(Svg + "text"), element => element.Value == "execute");
    }

    [Theory]
    [InlineData(true, "on")]
    [InlineData(false, "off")]
    public void Toggle_preview_communicates_boolean_state(bool isChecked, string expectedState)
    {
        var node = GuiBuilders.Toggle("enabled", new ToggleDefinition(isChecked, "enabled.changed"))
            .Text("Enabled")
            .Build();

        var document = XDocument.Parse(new SvgGuiRenderer().Render(node));
        var group = document.Descendants(Svg + "g")
            .Single(element => (string?)element.Attribute("data-control-kind") == "toggle");

        Assert.Contains(expectedState, (string?)group.Attribute("aria-label"));
    }
}
