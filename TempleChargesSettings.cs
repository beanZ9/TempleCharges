using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using System.Drawing;

namespace TempleCharges;

public class TempleChargesSettings : ISettings
{
    //Mandatory setting to allow enabling/disabling your plugin
    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    [Menu("Show Hint", "Show a hint when 6 of your current crystals can be traded in and converted to a set of rooms.\nYou can store 6 addtional charges this way for a total of 66.")]
    public ToggleNode ShowActive { get; set; } = new ToggleNode(true);
    public RangeNode<int> PositionX { get; set; } = new RangeNode<int>(1800, 0, 10000);
    public RangeNode<int> PositionY { get; set; } = new RangeNode<int>(100, 0, 10000);
    public ColorNode BackgroundColor { get; set; } = new ColorNode() { Value = Color.FromArgb(128, 0, 0, 0)};
    public ToggleNode Debug { get; set; } = new ToggleNode(false);
}