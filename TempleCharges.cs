using ExileCore2;
using ExileCore2.PoEMemory.Elements;
using System;
using System.Drawing;
using System.Linq;

namespace TempleCharges;

public class TempleCharges : BaseSettingsPlugin<TempleChargesSettings>
{
    private bool _initialized = false;
    private TempleConsolePanel _templePanel;
    private string _output;
    private bool _chargesFull;

    public override bool Initialise() {
        _initialized = true;
        return true;
    }

    public override void AreaChange(AreaInstance area) {
        _templePanel = GameController.IngameState.IngameUi.TempleConsolePanel;
    }

    public override void Tick() {
        if (!_initialized) {
            Initialise();
        }
        if (_templePanel == null) {
            DebugWindow.LogError("Unable to find temple console panel!");
            return;
        }

        var chargesText = "N/A";

        try {
            var rawString = _templePanel.Children[8].Children[1].Children[0].Text;
            LogDebug($"Raw string: {rawString}");
            chargesText = rawString.Split(' ').LastOrDefault();
            LogDebug($"Charges text: {chargesText}");
        } catch (Exception ex) {
            chargesText += " (Open Temple Console)";
            LogDebug($"Exception getting charges text: {ex.Message}");
        }

        var charges = chargesText.Split('/');
        _chargesFull = charges.Length > 1 && charges[0].Equals(charges[1]);

        _output = $"Temple Crystals: {chargesText}";
    }

    public override void Render() {
        if (GameController.IngameState.IngameUi.FullscreenPanels.Any(x => x.IsVisible)
        || GameController.IngameState.IngameUi.LargePanels.Any(x => x.IsVisible)
        || GameController.IngameState.IngameUi.OpenLeftPanel.IsVisible
        || GameController.IngameState.IngameUi.OpenRightPanel.IsVisible) {
            return;
        }
        
        var size = Graphics.MeasureText(_output);
        var color = _chargesFull ? Color.Red : Color.White;

        Graphics.DrawBox(new ExileCore2.Shared.RectangleF(Settings.PositionX - 5, Settings.PositionY, size.X + 10, size.Y), Settings.BackgroundColor);
        Graphics.DrawText(_output, new System.Numerics.Vector2(Settings.PositionX, Settings.PositionY), color);
    }

    private void LogDebug(string msg) {
        if (Settings.Debug) {
            DebugWindow.LogMsg(msg);
        }
    }
}