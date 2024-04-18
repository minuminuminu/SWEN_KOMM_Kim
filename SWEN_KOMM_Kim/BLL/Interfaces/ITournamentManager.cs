using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Interfaces
{
    internal interface ITournamentController
    {
        List<TournamentEntry> GetUserHistory(string username);
        TournamentState GetTournamentStateByUsername(string username);
        void HandleTournamentEntry(TournamentEntry entry, string tournamentName);
    }
}
