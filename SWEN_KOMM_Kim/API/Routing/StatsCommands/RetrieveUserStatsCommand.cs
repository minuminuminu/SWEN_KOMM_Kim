using Newtonsoft.Json;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.HttpServer.Response;
using SWEN_KOMM_Kim.HttpServer.Routing;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.API.Routing.StatsCommands
{
    internal class RetrieveUserStatsCommand : IRouteCommand
    {
        private readonly User _user;
        private readonly IStatsManager _statsManager;

        public RetrieveUserStatsCommand(User user, IStatsManager statsManager)
        {
            _user = user;
            _statsManager = statsManager;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var data = _statsManager.RetrieveUserStats(_user.Token);
                var jsonPayload = JsonConvert.SerializeObject(data);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }

            return response;
        }
    }
}
