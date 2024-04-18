using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SWEN_KOMM_Kim.BLL.Controllers
{
    internal class TournamentController : ITournamentController
    {
        private readonly ITournamentDao _tournamentDao;
        private readonly IStatsController _statsController;

        private readonly ConcurrentDictionary<string, Timer> tournamentTimers = new ConcurrentDictionary<string, Timer>();

        public TournamentController(ITournamentDao tournamentDao, IStatsController statsController)
        {
            _tournamentDao = tournamentDao;
            _statsController = statsController;
        }

        public List<TournamentEntry> GetUserHistory(string username)
        {
            List<TournamentEntry> entries = _tournamentDao.GetUserHistory(username);

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
                if (summarizedEntries.ContainsKey(entry.Username))
                {
                    summarizedEntries[entry.Username].Count += entry.Count;
                    summarizedEntries[entry.Username].Duration += entry.Duration;
                }
                else
                {
                    summarizedEntries[entry.Username] = new TournamentEntry(entry.Count, entry.Duration, entry.Username);
                }
            }

            return summarizedEntries.Values.ToList();
        }

        private List<string> GetUsersWithHighestCount(List<TournamentEntry> entries)
        {
            List<string> usersWithHighestCount = new List<string>();
            int highestCount = 0;

            foreach (var entry in entries)
            {
                if (entry.Count > highestCount)
                {
                    highestCount = entry.Count;
                }
            }

            foreach (var entry in entries)
            {
                if (entry.Count == highestCount)
                {
                    usersWithHighestCount.Add(entry.Username);
                }
            }

            return usersWithHighestCount;
        }

        public TournamentState GetTournamentStateByUsername(string username)
        {
            if (!_tournamentDao.IsUserInTournament(username))
            {
                throw new NoContentException();
            }

            var tournamentName = _tournamentDao.GetTournamentNameByUsername(username);
            var tournament = _tournamentDao.GetTournamentByName(tournamentName);

            List<TournamentEntry> summarizedEntries = SummarizeEntries(tournament);
            List<string> leadingUsers = GetUsersWithHighestCount(summarizedEntries);

            TournamentState tournamentState = new TournamentState(summarizedEntries.Count, leadingUsers, tournament.StartTime);
            
            return tournamentState;
        }

        public void HandleTournamentEntry(TournamentEntry entry, string tournamentName)
        {
            if (_tournamentDao.DoesTournamentExist(tournamentName))
            {
                _tournamentDao.InsertEntryInMemoryDB(entry, tournamentName);
                _tournamentDao.InsertHistoryEntry(entry);
            }
            else
            {
                Tournament tournament = new();
                tournament.Entries.Add(entry);

                _tournamentDao.InsertTournamentInMemoryDB(tournament, tournamentName);
                _tournamentDao.InsertHistoryEntry(entry);

                // start timer
                //int tournamentDurationInMilliseconds = 2 * 60 * 1000; // 2 minuten
                int tournamentDurationInMilliseconds = 10 * 1000; // 10 secs
                Timer timer = new Timer(TournamentFinishedCallback, tournamentName, tournamentDurationInMilliseconds, Timeout.Infinite);
                tournamentTimers.TryAdd(tournamentName, timer);

                Console.WriteLine($"Tournament '{tournamentName}' has started.\n");
            }
        }

        private void TournamentFinishedCallback(object state)
        {
            string tournamentName = (string)state;
            var tournament = _tournamentDao.GetTournamentByName(tournamentName);

            // tournament end logging and logic here
            List<TournamentEntry> summarizedEntries = SummarizeEntries(tournament);
            List<TournamentEntry> sortedEntries = summarizedEntries.OrderByDescending(e => e.Count).ToList();

            List<TournamentEntry> winners = new List<TournamentEntry>();
            List<TournamentEntry> otherParticipants = new List<TournamentEntry>();

            int highestCount = sortedEntries.First().Count;

            foreach (var entry in sortedEntries)
            {
                if (entry.Count == highestCount)
                {
                    winners.Add(entry);
                }
                else
                {
                    otherParticipants.Add(entry);
                }
            }

            if(winners.Count == 1)
            {
                _statsController.IncreaseStats(2, winners.First().Count, winners.First().Username + "-sebToken");
            }
            else
            {
                foreach (var winner in winners)
                {
                    _statsController.IncreaseStats(1, winner.Count, winner.Username + "-sebToken");
                }
            }

            foreach (var participant in otherParticipants)
            {
                _statsController.DecreaseStats(participant.Count, participant.Username + "-sebToken");
            }

            foreach (var entry in sortedEntries)
            {
                Console.WriteLine($"User '{entry.Username}' has count '{entry.Count}' and duration '{entry.Duration}'.");
            }

            // remove tournament from db and timer clean up
            _tournamentDao.RemoveTournamentFromMemoryDB(tournamentName);
            tournamentTimers[tournamentName].Dispose();
            tournamentTimers.TryRemove(tournamentName, out _);

            Console.WriteLine($"\nTournament '{tournamentName}' has ended.\n");
        }
    }
}
