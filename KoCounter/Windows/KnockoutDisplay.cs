using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KoCounter.DataTypes;
namespace KoCounter.Windows;

public class KnockoutDisplay : Window, IDisposable
{
    private string GoatImagePath;
    private Plugin Plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public KnockoutDisplay(Plugin plugin, string goatImagePath)
        : base("Knockout Display##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(100, 100),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        GoatImagePath = goatImagePath;
        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.SetWindowFontScale(Plugin.Configuration.FontSize);
        if (Plugin.KoCounter.CurrentSession != null)
        {
            ImGui.Text("Knockouts: " + Plugin.KoCounter.CurrentSession.Knockouts.Count);
            ImGui.Text("Knockout Streak: " + Plugin.KoCounter.KnockoutStreak);
            ImGui.Text("Best Knockout Streak: " + Plugin.KoCounter.CurrentSession.HighestKnockoutStreak);
        }
        else
        {
            ImGui.Text("Enter a PVP zone to start tracking a session.");
        }
        ImGui.SetWindowFontScale(1);
    }
}
