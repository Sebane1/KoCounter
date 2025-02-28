using System;
using System.Collections.Generic;

namespace KoCounter.DataTypes
{
    public class Session
    {
        string _sessionId = "";
        ushort territoryId = 0;
        DateTime _sessionStart;
        DateTime _sessionEnd;
        List<Knockout> _knockouts = new List<Knockout>();
        List<Knockout> _knockoutsByOtherPlayer = new List<Knockout>();

        int _highestKnockoutStreak;
        public string SessionId { get => _sessionId; set => _sessionId = value; }
        public DateTime SessionEnd { get => _sessionEnd; set => _sessionEnd = value; }
        public DateTime SessionStart { get => _sessionStart; set => _sessionStart = value; }
        public List<Knockout> Knockouts { get => _knockouts; set => _knockouts = value; }
        public int HighestKnockoutStreak { get => _highestKnockoutStreak; set => _highestKnockoutStreak = value; }
        public ushort TerritoryId { get => territoryId; set => territoryId = value; }
        public List<Knockout> KnockoutsByOtherPlayer { get => _knockoutsByOtherPlayer; set => _knockoutsByOtherPlayer = value; }
    }
}
