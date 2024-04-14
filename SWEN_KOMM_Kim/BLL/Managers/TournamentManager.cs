﻿using SWEN_KOMM_Kim.BLL.Interfaces;
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
        private readonly IStatsManager _statsManager;

        private readonly Dictionary<string, Timer> tournamentTimers = new Dictionary<string, Timer>();

        public TournamentManager(ITournamentDao tournamentDao, IStatsManager statsManager)
        {
            _tournamentDao = tournamentDao;
            _statsManager = statsManager;
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
                    usersWithHighestCount.Add(entry.AuthToken);
                }
            }

            return usersWithHighestCount;
        }

        public TournamentState GetTournamentStateByAuthToken(string authToken)
        {
            if (!_tournamentDao.IsUserInTournament(authToken))
            {
                throw new NoContentException();
            }

            var tournamentName = _tournamentDao.GetTournamentNameByAuthToken(authToken);
            var tournament = _tournamentDao.GetTournamentByName(tournamentName);

            List<TournamentEntry> summarizedEntries = SummarizeEntries(tournament);
            List<string> leadingUsersAuthToken = GetUsersWithHighestCount(summarizedEntries);

            TournamentState tournamentState = new TournamentState(summarizedEntries.Count, leadingUsersAuthToken, tournament.StartTime);
            
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
                tournament.StartTime = DateTime.Now;

                _tournamentDao.InsertTournamentInMemoryDB(tournament, tournamentName);
                _tournamentDao.InsertHistoryEntry(entry);

                // start timer
                //int tournamentDurationInMilliseconds = 2 * 60 * 1000;
                //Timer timer = new Timer(TournamentTimerElapsed, tournamentName, tournamentDurationInMilliseconds, Timeout.Infinite);
                Timer timer = new Timer(TournamentFinishedCallback, tournamentName, 10000, Timeout.Infinite);
                tournamentTimers.Add(tournamentName, timer);

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
                _statsManager.IncreaseStats(2, winners.First().Count, winners.First().AuthToken);
            }
            else
            {
                foreach (var winner in winners)
                {
                    _statsManager.IncreaseStats(1, winner.Count, winner.AuthToken);
                }
            }

            foreach (var participant in otherParticipants)
            {
                _statsManager.DecreaseStats(participant.Count, participant.AuthToken);
            }

            foreach (var entry in sortedEntries)
            {
                Console.WriteLine($"User '{entry.AuthToken}' has count '{entry.Count}' and duration '{entry.Duration}'.");
            }

            // remove tournament from db and timer clean up
            _tournamentDao.RemoveTournamentFromMemoryDB(tournamentName);
            tournamentTimers[tournamentName].Dispose();
            tournamentTimers.Remove(tournamentName);

            Console.WriteLine($"\nTournament '{tournamentName}' has ended.\n");
        }
    }
}
