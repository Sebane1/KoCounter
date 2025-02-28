using Dalamud.Configuration;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using KoCounter.DataTypes;
using System;
using System.Collections.Generic;

namespace KoCounter;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    bool _enabled = false;
    public Dictionary<string, CharacterStats> Characters { get => _characters; set => _characters = value; }
    public bool Enabled { get => _enabled; set => _enabled = value; }
    public float FontSize { get => fontSize; set => fontSize = value; }

    Dictionary<string, CharacterStats> _characters = new Dictionary<string, CharacterStats>();
    private float fontSize = 1;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
