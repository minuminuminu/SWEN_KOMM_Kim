using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.DAL.Interfaces
{
    internal interface IStatsDao
    {
        bool CreateUserStatsEntry(string authToken);
        bool IncreaseStats(int elo, int pushups, string authToken);
        bool DecreaseStats(int pushups, string authToken);
        UserStats? RetrieveUserStats(string authToken);
        List<UserStats> RetrieveScoreboard();
    }
}
