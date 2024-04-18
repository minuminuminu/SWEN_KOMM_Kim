using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class Tournament
    {
        public DateTime StartTime { get; set; }
        public ConcurrentBag<TournamentEntry> Entries { get; set; }

        public Tournament()
        {
            Entries = new ConcurrentBag<TournamentEntry>();
            StartTime = DateTime.Now;
        }
    }
}
