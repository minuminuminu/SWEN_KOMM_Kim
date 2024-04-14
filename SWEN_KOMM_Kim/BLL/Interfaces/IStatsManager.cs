using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Interfaces
{
    internal interface IStatsManager
    {
        void CreateUserStatsEntry(string authToken);
        UserStats RetrieveUserStats(string authToken);
        List<UserStats> RetrieveScoreboard();
    }
}
