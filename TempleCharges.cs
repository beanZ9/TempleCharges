using ExileCore2;
using ExileCore2.PoEMemory.Elements;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace TempleCharges;

public class TempleCharges : BaseSettingsPlugin<TempleChargesSettings>
{
    private bool _initialized = false;
    private TempleConsolePanel _templePanel;

    private int _chargesCount;
    private int _maxCharges;
    private int _previousChargesCount = -1;
    private int _recentlyGained;
    private bool _chargesFull;
    private bool _areaIsTemple;
    private bool _canGrabRooms;

    private Stopwatch _notificationTimer = new Stopwatch();
    private string _output;

    public override bool Initialise() {
        _initialized = true;
        return true;
    }

    public override void AreaChange(AreaInstance area) {
        _templePanel = GameController.IngameState.IngameUi.TempleConsolePanel;
        _areaIsTemple = area.Area.Id.Equals("IncursionTemple");
    }

    public override void Tick() {
        if (!_initialized) {
            Initialise();
        }
        if (_templePanel == null) {
            DebugWindow.LogError("Unable to find temple console panel!");
            return;
        }

        _output = "Temple Crystals: ";
        var chargesText = string.Empty;

        try {
            var rawString = _templePanel.Children[8].Children[1].Children[0].Text;
            LogDebug($"Raw string: {rawString}");
            chargesText = rawString.Split(' ').LastOrDefault();
            LogDebug($"Charges text: {chargesText}");
        } catch (Exception ex) {
            _output += "N/A (Open Temple Console)";
            LogDebug($"Exception getting charges text: {ex.Message}");
            return;
        }

        var charges = chargesText.Split('/');

        if (_previousChargesCount >= 0) {
            _previousChargesCount = _chargesCount;
        }
        
        try {
            _chargesCount = int.Parse(charges[0]);
            _maxCharges = int.Parse(charges[1]);
        } catch (Exception ex) {
            LogDebug($"Exception parsing charges: {ex.Message}");
        }

        if (_previousChargesCount < 0) {
            _previousChargesCount = _chargesCount;
        }

        _chargesFull = charges.Length > 1 && _chargesCount >= _maxCharges;

        try {
            _canGrabRooms = _templePanel.Children[9].Children[2].ChildCount <= 0 &&
                 _chargesCount >= 6 &&
                !(_templePanel.Children[8].Children[0].Children[0].Text.Contains("close temple") ||
                    _areaIsTemple);
        } catch (Exception ex) {
            LogDebug($"Exception getting room cards: {ex.Message}");
        }

        _output += $"{_chargesCount}/{_maxCharges}{(Settings.ShowActive && _canGrabRooms ? $" (Turn in)" : string.Empty)}";
        
        if (_previousChargesCount < _chargesCount) {
            _recentlyGained = _chargesCount - _previousChargesCount;
            _notificationTimer.Restart();
        }
    }

    public override void Render() {
        if (GameController.IngameState.IngameUi.FullscreenPanels.Any(x => x.IsVisible)
        || GameController.IngameState.IngameUi.LargePanels.Any(x => x.IsVisible)
        || GameController.IngameState.IngameUi.OpenLeftPanel.IsVisible
        || GameController.IngameState.IngameUi.OpenRightPanel.IsVisible) {
            return;
        }
        
        if (_notificationTimer.IsRunning) {
            if (_notificationTimer.ElapsedMilliseconds < 10000) {
                _output += $"\nRecently gained: {_recentlyGained}";
            } else {
                _notificationTimer.Reset();
            }
        }

        var size = Graphics.MeasureText(_output);
        var color = _chargesFull ? Color.Red : (_canGrabRooms ? Color.Green : Color.White);

        Graphics.DrawBox(new ExileCore2.Shared.RectangleF(Settings.PositionX - 5, Settings.PositionY, size.X + 10, size.Y), Settings.BackgroundColor);
        Graphics.DrawText(_output, new System.Numerics.Vector2(Settings.PositionX, Settings.PositionY), color);
    }


    private void LogDebug(string msg) {
        if (Settings.Debug) {
            DebugWindow.LogMsg(msg);
        }
    }
}