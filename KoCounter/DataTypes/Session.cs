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
        int _defeats = 0;
        int _highestKnockoutStreak;
        List<string> _fullTranscript = new List<string>();
        List<string> _relatedToPlayer = new List<string>();
        List<string> _relatedToOthers = new List<string>();
        public string SessionId { get => _sessionId; set => _sessionId = value; }
        public DateTime SessionEnd { get => _sessionEnd; set => _sessionEnd = value; }
        public DateTime SessionStart { get => _sessionStart; set => _sessionStart = value; }
        public List<Knockout> Knockouts { get => _knockouts; set => _knockouts = value; }
        public int HighestKnockoutStreak { get => _highestKnockoutStreak; set => _highestKnockoutStreak = value; }
        public ushort TerritoryId { get => territoryId; set => territoryId = value; }
        public List<Knockout> KnockoutsByOtherPlayer { get => _knockoutsByOtherPlayer; set => _knockoutsByOtherPlayer = value; }
        public int Defeats { get => _defeats; set => _defeats = value; }
        public List<string> FullTranscript { get => _fullTranscript; set => _fullTranscript = value; }
        public List<string> RelatedToPlayer { get => _relatedToPlayer; set => _relatedToPlayer = value; }
        public List<string> RelatedToOthers { get => _relatedToOthers; set => _relatedToOthers = value; }
    }
}
