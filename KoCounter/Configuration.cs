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
    public Dictionary<string, CharacterStats> Characters { get => _characters; set => _characters = value; }
    public bool Enabled { get => _enabled; set => _enabled = value; }
    public float FontSize { get => _fontSize; set => _fontSize = value; }
    public Vector4 Colour { get => _colour; set => _colour = value; }

    Dictionary<string, CharacterStats> _characters = new Dictionary<string, CharacterStats>();
    private float _fontSize = 1;
    private Vector4 _colour = new Vector4(1,1, 1, 1);

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
