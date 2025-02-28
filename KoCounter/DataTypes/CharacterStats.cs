using System.Collections.Generic;

namespace KoCounter.DataTypes
{
    public class CharacterStats
    {
        string _characterName = "";
        Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        public Dictionary<string, Session> Sessions { get => _sessions; set => _sessions = value; }
        public string CharacterName { get => _characterName; set => _characterName = value; }
    }
}
