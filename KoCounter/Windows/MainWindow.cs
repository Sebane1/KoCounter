using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KoCounter.DataTypes;
using Lumina.Excel.Sheets;

namespace KoCounter.Windows;

public class MainWindow : Window, IDisposable
{
    private string GoatImagePath;
    private Plugin Plugin;
    private int _selectedCharacter;
    private CharacterStats _currentCharacter;
    private int _selectedSession;
    private Session _currentSession;
    private int _selectedKnockout;
    private string[] _knockouts = new string[0];
    private string _sessionTime;
    private Knockout _currentKnockout;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("Main Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        GoatImagePath = goatImagePath;
        Plugin = plugin;
    }

    public void Dispose() { }
    public override void OnOpen()
    {
        base.OnOpen();
        var values = Plugin.Configuration.Characters;
        string[] characters = values.Keys.ToArray<string>();
        _currentCharacter = values[characters[_selectedCharacter]];
    }
    public override void Draw()
    {
        if (ImGui.BeginTabBar("Knockout Statistics"))
        {
            if (ImGui.BeginTabItem("Knockout Reports"))
            {
                ImGui.BeginTable("##KO Counter", 2);
                ImGui.TableSetupColumn("Characters List", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn("Session Information", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawCharacters();
                ImGui.TableSetColumnIndex(1);
                DrawSessions();
                ImGui.EndTable();

                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Settings"))
            {
                var pluginEnabled = Plugin.Configuration.Enabled;
                var pluginFontSize = Plugin.Configuration.FontSize;
                if (ImGui.Checkbox("Tracking Enabled", ref pluginEnabled))
                {
                    Plugin.Configuration.Enabled = pluginEnabled;
                    Plugin.Configuration.Save();
                }
                if (ImGui.InputFloat("Font Size", ref pluginFontSize))
                {
                    Plugin.Configuration.FontSize = pluginFontSize;
                    Plugin.Configuration.Save();
                }
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    public void DrawCharacters()
    {
        var values = Plugin.Configuration.Characters;
        string[] characters = values.Keys.ToArray<string>();
        ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
        if (ImGui.ListBox("##Characters", ref _selectedCharacter, characters, characters.Length, 25))
        {
            _currentCharacter = values[characters[_selectedCharacter]];
            _currentSession = null;
        }
    }
    public void DrawSessions()
    {
        ImGui.BeginTable("##KO Counter", 2);
        ImGui.TableSetupColumn("Sessions", ImGuiTableColumnFlags.WidthFixed, 150);
        ImGui.TableSetupColumn("Knockout Info", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableHeadersRow();
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        var sessionList = _currentCharacter.Sessions;
        string[] sessionNames = sessionList.Keys.ToArray<string>();
        ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
        if (ImGui.ListBox("##Sessions", ref _selectedSession, sessionNames, sessionNames.Length, 24))
        {
            _currentSession = sessionList[sessionNames[_selectedSession]];
            List<string> newKnockoutList = new List<string>();
            foreach (var knockout in _currentSession.Knockouts)
            {
                newKnockoutList.Add(knockout.PlayerKnockedOut);
            }
            _knockouts = newKnockoutList.ToArray();
            _sessionTime = (_currentSession.SessionEnd - _currentSession.SessionStart).ToString("h':'mm':'ss");
        }
        ImGui.TableSetColumnIndex(1);
        DrawKnockouts();
        ImGui.EndTable();
    }
    public void DrawKnockouts()
    {
        if (_currentSession != null)
        {
            ImGui.Text("Knockouts This Session: " + _currentSession.Knockouts.Count);
            ImGui.Text("Best Knockout Streak: " + _currentSession.HighestKnockoutStreak);
            ImGui.Text($"Session Length: {_sessionTime}");
            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
            if (ImGui.ListBox("##Knockouts", ref _selectedKnockout, _knockouts, _knockouts.Length, Math.Clamp(_knockouts.Length, 0, 10)))
            {
                _currentKnockout = _currentSession.Knockouts[_selectedKnockout];
            }
            if (_currentKnockout != null)
            {
                ImGui.Text("You knocked out " + _currentKnockout.PlayerKnockedOut + " at " + _currentKnockout.KnockoutDateTime.ToString("MM/dd/yyyy HH:mm"));
            }
        }
        else
        {
            ImGui.Text($"Select a character and a session to view past statistics.\r\nIf no sessions exist, start playing some PVP matches!");
        }
    }
}
