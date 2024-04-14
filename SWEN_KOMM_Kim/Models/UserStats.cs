using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class UserStats
    {
        public string Name { get; set; }
        public int Elo {  get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public UserStats(string name, int elo, int wins, int losses)
        {
            Name = name;
            Elo = elo;
            Wins = wins;
            Losses = losses;
        }
    }
}
