using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Managers
{
    internal class TournamentManager : ITournamentManager
    {
        private readonly ITournamentDao _tournamentDao;

        public TournamentManager(ITournamentDao tournamentDao)
        {
            _tournamentDao = tournamentDao;
        }

        public List<TournamentEntry> GetUserHistory(string authToken)
        {
            List<TournamentEntry> entries = _tournamentDao.GetUserHistory(authToken);

            if(entries.Count == 0)
            {
                throw new NoContentException();
            }

            return entries;
        }

        private List<TournamentEntry> SummarizeEntries(Tournament tournament)
        {
            Dictionary<string, TournamentEntry> summarizedEntries = new Dictionary<string, TournamentEntry>();

            foreach (var entry in tournament.Entries)
            {
                if (summarizedEntries.ContainsKey(entry.AuthToken))
                {
                    summarizedEntries[entry.AuthToken].Count += entry.Count;
                    summarizedEntries[entry.AuthToken].Duration += entry.Duration;
                }
                else
                {
                    summarizedEntries[entry.AuthToken] = new TournamentEntry(entry.Count, entry.Duration, entry.AuthToken);
                }
            }

            return summarizedEntries.Values.ToList();
        }


        public TournamentState GetTournamentStateByAuthToken(string authToken)
        {
            if (!_tournamentDao.IsUserInTournament(authToken))
            {
                throw new NoContentException();
            }

            var tournamentName = _tournamentDao.GetTournamentNameByAuthToken(authToken);
            var tournament = _tournamentDao.GetTournamentByName(tournamentName);


        }
    }
}
