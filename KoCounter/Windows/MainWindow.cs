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
    private int _previousKnockoutCount;
    private string[] _defeats;
    private int _previousDefeatCount;
    private Knockout _currentDefeat;
    private int _selectedDefeat;

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
                DrawSettings();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Credits"))
            {
                DrawCredits();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawCredits()
    {
        ImGui.Text("Plugin concept made and funded by Aresyne Skye.");
        ImGui.Text("Programmed and designed by Sebby.");
    }

    private void DrawSettings()
    {
        var pluginEnabled = Plugin.Configuration.Enabled;
        var pluginDebug = Plugin.Configuration.DebugCommands;
        var pluginFontSize = Plugin.Configuration.FontSize;
        var pluginTextColour = Plugin.Configuration.Colour;

        var knockouts = Plugin.Configuration.DisplayKnockouts;
        var knockoutStreak = Plugin.Configuration.DisplayKnockoutStreak;
        var bestKnockoutStreak = Plugin.Configuration.DisplayBestKnockoutStreak;
        var defeats = Plugin.Configuration.DisplayDefeats;

        if (ImGui.Checkbox("Tracking Enabled", ref pluginEnabled))
        {
            Plugin.Configuration.Enabled = pluginEnabled;
            Plugin.Configuration.Save();
        }
        if (ImGui.Checkbox("Display Knockouts", ref knockouts))
        {
            Plugin.Configuration.DisplayKnockouts = knockouts;
            Plugin.Configuration.Save();
        }
        if (ImGui.Checkbox("Display Knockout Streak", ref knockoutStreak))
        {
            Plugin.Configuration.DisplayKnockoutStreak = knockoutStreak;
            Plugin.Configuration.Save();
        }
        if (ImGui.Checkbox("Display Best Knockout Streak", ref bestKnockoutStreak))
        {
            Plugin.Configuration.DisplayBestKnockoutStreak = bestKnockoutStreak;
            Plugin.Configuration.Save();
        }
        if (ImGui.Checkbox("Display Defeats", ref defeats))
        {
            Plugin.Configuration.DisplayDefeats = defeats;
            Plugin.Configuration.Save();
        }
        if (ImGui.SliderFloat("Font Size", ref pluginFontSize, 0.1f, 10))
        {
            Plugin.Configuration.FontSize = pluginFontSize;
            Plugin.Configuration.Save();
        }
        if (ImGui.ColorEdit4("Font Colour", ref pluginTextColour))
        {
            Plugin.Configuration.Colour = pluginTextColour;
            Plugin.Configuration.Save();
        }
        if (ImGui.Checkbox("Debug Commands Enabled", ref pluginDebug))
        {
            Plugin.Configuration.DebugCommands = pluginDebug;
            Plugin.Configuration.Save();
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
            _previousKnockoutCount = 0;
        }
        ImGui.TableSetColumnIndex(1);
        DrawKnockouts();
        ImGui.EndTable();
    }
    public void DrawKnockouts()
    {
        if (_currentSession != null)
        {
            if (ImGui.BeginTabBar("Transcripts Overview"))
            {
                if (ImGui.BeginTabItem("Session Overview"))
                {
                    DrawSessionOverview();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Raw Transcripts"))
                {
                    DrawTranscripts();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        else
        {
            ImGui.Text($"Select a character and a session to view past statistics.\r\nIf no sessions exist, start playing some PVP matches!");
        }
    }

    private void DrawTranscripts()
    {
        if (ImGui.BeginTabBar("Transcripts Overview"))
        {
            if (ImGui.BeginTabItem("Related To You"))
            {
                var transcriptItems = _currentSession.RelatedToPlayer.ToArray();
                var index = 0;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                if (ImGui.ListBox("##Related To You", ref index, transcriptItems, transcriptItems.Length, 20))
                {
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Related To Others"))
            {
                var transcriptItems = _currentSession.RelatedToOthers.ToArray();
                var index = 0;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                if (ImGui.ListBox("##Related To Others", ref index, transcriptItems, transcriptItems.Length, 20))
                {
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Everything"))
            {
                var transcriptItems = _currentSession.FullTranscript.ToArray();
                var index = 0;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                if (ImGui.ListBox("##Everything", ref index, transcriptItems, transcriptItems.Length, 20))
                {
                }
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawSessionOverview()
    {
        ImGui.Text("Knockouts This Session: " + _currentSession.Knockouts.Count);
        ImGui.Text("Best Knockout Streak: " + _currentSession.HighestKnockoutStreak);
        ImGui.Text("Defeats: " + _currentSession.Defeats);
        var sessionTime = (_currentSession.SessionEnd - _currentSession.SessionStart).ToString("h':'mm':'ss");
        ImGui.Text($"Session Length: {sessionTime}");
        if (_currentSession.Knockouts.Count > _previousKnockoutCount)
        {
            List<string> newKnockoutList = new List<string>();
            foreach (var knockout in _currentSession.Knockouts)
            {
                newKnockoutList.Add(knockout.RelatedPlayer);
            }
            _knockouts = newKnockoutList.ToArray();
            _previousKnockoutCount = _knockouts.Length;
        }
        if (_currentSession.KnockoutsByOtherPlayer.Count > _previousDefeatCount)
        {
            List<string> newDefeatList = new List<string>();
            foreach (var knockout in _currentSession.KnockoutsByOtherPlayer)
            {
                newDefeatList.Add(knockout.RelatedPlayer);
            }
            _defeats = newDefeatList.ToArray();
            _previousDefeatCount = _defeats.Length;
        }
        if (ImGui.BeginTabBar("Knockout Statistics"))
        {
            if (ImGui.BeginTabItem("Knockouts"))
            {
                if (_currentSession.Knockouts.Count > 0)
                {
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                    if (ImGui.ListBox("##Knockouts", ref _selectedKnockout, _knockouts, _knockouts.Length, Math.Clamp(_knockouts.Length, 0, 10)))
                    {
                        _currentKnockout = _currentSession.Knockouts[_selectedKnockout];
                    }
                    if (_currentKnockout != null)
                    {
                        ImGui.Text("You knocked out " + _currentKnockout.RelatedPlayer + " at " + _currentKnockout.KnockoutDateTime.ToString("MM/dd/yyyy HH:mm"));
                    }
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Defeats"))
            {
                if (_currentSession.KnockoutsByOtherPlayer.Count > 0)
                {
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                    if (ImGui.ListBox("##Defeats", ref _selectedDefeat, _defeats, _defeats.Length, Math.Clamp(_defeats.Length, 0, 10)))
                    {
                        _currentDefeat = _currentSession.KnockoutsByOtherPlayer[_selectedDefeat];
                    }
                    if (_currentDefeat != null)
                    {
                        ImGui.Text("You were defeated by " + _currentDefeat.RelatedPlayer + " at " + _currentDefeat.KnockoutDateTime.ToString("MM/dd/yyyy HH:mm"));
                    }
                }
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }
}
