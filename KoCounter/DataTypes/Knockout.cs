using System;

namespace KoCounter.DataTypes
{
    public class Knockout
    {
        DateTime _knockoutDateTime;
        string _relatedPlayer;

        public DateTime KnockoutDateTime { get => _knockoutDateTime; set => _knockoutDateTime = value; }
        public string RelatedPlayer { get => _relatedPlayer; set => _relatedPlayer = value; }

        public Knockout(string playerKnockedOut)
        {
            _knockoutDateTime = DateTime.Now;
            _relatedPlayer = playerKnockedOut;
        }
    }
}
