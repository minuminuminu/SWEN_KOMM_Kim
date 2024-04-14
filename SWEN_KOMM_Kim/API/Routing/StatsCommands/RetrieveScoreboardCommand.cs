using Newtonsoft.Json;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.HttpServer.Response;
using SWEN_KOMM_Kim.HttpServer.Routing;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SWEN_KOMM_Kim.API.Routing.StatsCommands
{
    internal class RetrieveScoreboardCommand : IRouteCommand
    {
        private readonly IStatsManager _statsManager;
        private readonly User _user;

        public RetrieveScoreboardCommand(IStatsManager statsManager, User user)
        {
            _statsManager = statsManager;
            _user = user;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            List<UserStats> scoreboard = _statsManager.RetrieveScoreboard();
            var jsonPayload = JsonConvert.SerializeObject(scoreboard);
            response = new HttpResponse(StatusCode.Ok, jsonPayload);

            return response;
        }
    }
}
