using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.DAL.Interfaces
{
    internal interface ITournamentDao
    {
        List<TournamentEntry> GetUserHistory(string authToken);
        bool IsUserInTournament(string authToken);
        string? GetTournamentNameByAuthToken(string authToken);
        Tournament? GetTournamentByName(string tournamentName);
        bool InsertHistoryEntry(TournamentEntry entry);
        bool InsertEntryInMemoryDB(TournamentEntry entry, string tournamentName);
        bool InsertTournamentInMemoryDB(Tournament tournament, string tournamentName);
        bool RemoveTournamentFromMemoryDB(string tournamentName);
        bool DoesTournamentExist(string tournamentName);
    }
}
