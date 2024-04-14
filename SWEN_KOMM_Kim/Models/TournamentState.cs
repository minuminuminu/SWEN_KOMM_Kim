using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class TournamentState
    {
        public int ParticipantCount { get; set; }
        public List<string> LeadingParticipantsAuthToken { get; set; }
        public DateTime StartTime { get; set; }

        public TournamentState(int participantCount, List<string> leadingUsers, DateTime startTime)
        {
            ParticipantCount = participantCount;
            LeadingParticipantsAuthToken = leadingUsers;
            StartTime = startTime;
        }
    }
}
