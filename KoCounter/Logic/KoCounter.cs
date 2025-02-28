using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using KoCounter;
using KoCounter.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KoCounter.Logic
{
    public class KoCounter : IDisposable
    {
        string _sessionId;
        DateTime _sessionStart;
        DateTime _sessionEnd;
        private Dictionary<string, Session> _currentPlayerSessionList;
        private Session _currentSession;
        private bool _wasKnockedOut;
        int _knockoutStreak;

        public Session CurrentSession { get => _currentSession; set => _currentSession = value; }
        public int KnockoutStreak { get => _knockoutStreak; set => _knockoutStreak = value; }

        public KoCounter()
        {
            Plugin.ChatGui.ChatMessage += ChatGui_ChatMessage;
            Plugin.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
            Plugin.Framework.Update += Framework_Update;
            Plugin.ClientState.Login += ClientState_Login;
            Initialize();
        }

        private void ClientState_Login()
        {
            Initialize();
        }

        void Initialize()
        {
            if (Plugin.ClientState.LocalPlayer != null)
            {
                string name = Plugin.ClientState.LocalPlayer.Name.TextValue;
                if (!Plugin.Configuration.Characters.ContainsKey(name))
                {
                    Plugin.Configuration.Characters[name] = new CharacterStats() { CharacterName = name };
                }
                ClientState_TerritoryChanged(Plugin.ClientState.TerritoryType);
            }
        }

        private void Framework_Update(IFramework framework)
        {
            if (Plugin.ClientState.IsLoggedIn && Plugin.ClientState.IsPvPExcludingDen && Plugin.Configuration.Enabled)
            {
                if (Plugin.ClientState.LocalPlayer != null)
                {
                    if (Plugin.ClientState.LocalPlayer.IsDead)
                    {
                        if (!_wasKnockedOut)
                        {
                            _wasKnockedOut = true;
                            _knockoutStreak = 0;
                        }
                    }
                    else if (_wasKnockedOut)
                    {
                        _wasKnockedOut = false;
                    }
                }
            }
        }

        private void ClientState_TerritoryChanged(ushort obj)
        {
            if (_currentSession != null)
            {
                _currentSession.SessionEnd = DateTime.Now;
                _currentSession = null;
                Plugin.Configuration.Save();
            }

            if (Plugin.ClientState.IsPvP || obj == 250)
            {
                _sessionStart = DateTime.Now;
                _sessionId = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                _currentPlayerSessionList = Plugin.Configuration.Characters[Plugin.ClientState.LocalPlayer.Name.TextValue].Sessions;
                if (!_currentPlayerSessionList.ContainsKey(_sessionId))
                {
                    _currentPlayerSessionList[_sessionId] = new DataTypes.Session() { SessionId = _sessionId, SessionStart = _sessionStart, };
                    _currentSession = _currentPlayerSessionList[_sessionId];
                    _currentSession.TerritoryId = obj;
                }
            }
        }

        public void DebugIncrement()
        {
            string name = "";
            string message = "You have defeated Debug Person";
            ProcessChat((Dalamud.Game.Text.XivChatType)2874, 0, name, message);
        }
        private void ChatGui_ChatMessage(Dalamud.Game.Text.XivChatType type, int timestamp,
            ref Dalamud.Game.Text.SeStringHandling.SeString sender,
            ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
        {
            ProcessChat(type, timestamp, sender.TextValue, message.TextValue);
        }

        private void ProcessChat(Dalamud.Game.Text.XivChatType type, int timestamp,
            string sender,
            string message)
        {
            if (Plugin.Configuration.Enabled)
            {
                try
                {
                    Plugin.PluginLog.Debug((int)type + " " + message);
                    var messageAsString = message.ToString();
                    var tokens = messageAsString.Split(" ");
                    if (Plugin.ClientState.IsPvP)
                    {
                        switch ((int)type)
                        {
                            case 2874:
                                Task.Run(() =>
                                {
                                    _currentSession.Knockouts.Add(new DataTypes.Knockout(tokens[tokens.Length - 2] + " " + tokens[tokens.Length - 1]));
                                    _knockoutStreak++;
                                    if (_knockoutStreak > _currentSession.HighestKnockoutStreak)
                                    {
                                        _currentSession.HighestKnockoutStreak = _knockoutStreak;
                                    }
                                });
                                break;
                            case 4922:
                                Task.Run(() =>
                                {
                                });
                                break;

                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.PluginLog.Warning(e, e.Message);
                }
            }
        }

        public void Dispose()
        {
            Plugin.ChatGui.ChatMessage -= ChatGui_ChatMessage;
        }
    }
}
