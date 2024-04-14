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
    internal class StatsManager : IStatsManager
    {
        private readonly IStatsDao _statsDao;

        public StatsManager(IStatsDao statsDao)
        {
            _statsDao = statsDao;
        }

        public void CreateUserStatsEntry(string authToken)
        {
            _statsDao.CreateUserStatsEntry(authToken);
        }

        public void IncreaseStats(int elo, int pushups, string authToken)
        {
            _statsDao.IncreaseStats(elo, pushups, authToken);
        }

        public void DecreaseStats(int pushups, string authToken)
        {
            _statsDao.DecreaseStats(pushups, authToken);
        }

        public UserStats RetrieveUserStats(string authToken)
        {
            var stats = _statsDao.RetrieveUserStats(authToken);

            if(stats == null)
            {
                throw new UserNotFoundException();
            }

            return stats;
        }

        public List<UserStats> RetrieveScoreboard()
        {
            List<UserStats> scoreboard = _statsDao.RetrieveScoreboard();
            List<UserStats> sortedScoreboard = scoreboard.OrderByDescending(e => e.Elo).ToList();

            return sortedScoreboard;
        }
    }
}
