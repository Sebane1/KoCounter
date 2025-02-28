using System;

namespace KoCounter.DataTypes
{
    public class Knockout
    {
        DateTime _knockoutDateTime;
        string _playerKnockedOut;

        public DateTime KnockoutDateTime { get => _knockoutDateTime; set => _knockoutDateTime = value; }
        public string PlayerKnockedOut { get => _playerKnockedOut; set => _playerKnockedOut = value; }

        public Knockout(string playerKnockedOut)
        {
            _knockoutDateTime = DateTime.Now;
            _playerKnockedOut = playerKnockedOut;
        }
    }
}
