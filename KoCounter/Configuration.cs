using Dalamud.Configuration;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using KoCounter.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace KoCounter;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    private bool _enabled = false;
    private bool _counterVisible = false;
    private bool _debugCommands = false;

    private bool _displayKnockouts = true;
    private bool _displayKnockoutStreak = true;
    private bool _displayBestKnockoutStreak = true;
    private bool _displayDefeats = true;

    public Dictionary<string, CharacterStats> Characters { get => _characters; set => _characters = value; }
    public bool Enabled { get => _enabled; set => _enabled = value; }
    public float FontSize { get => _fontSize; set => _fontSize = value; }
    public Vector4 Colour { get => _colour; set => _colour = value; }
    public bool CounterVisible { get => _counterVisible; set => _counterVisible = value; }
    public bool DebugCommands { get => _debugCommands; set => _debugCommands = value; }
    public bool DisplayKnockouts { get => _displayKnockouts; set => _displayKnockouts = value; }
    public bool DisplayKnockoutStreak { get => _displayKnockoutStreak; set => _displayKnockoutStreak = value; }
    public bool DisplayBestKnockoutStreak { get => _displayBestKnockoutStreak; set => _displayBestKnockoutStreak = value; }
    public bool DisplayDefeats { get => _displayDefeats; set => _displayDefeats = value; }

    Dictionary<string, CharacterStats> _characters = new Dictionary<string, CharacterStats>();
    private float _fontSize = 1;
    private Vector4 _colour = new Vector4(1,1, 1, 1);

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
