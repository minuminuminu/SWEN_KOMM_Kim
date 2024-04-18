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

namespace SWEN_KOMM_Kim.API.Routing.TournamentCommands
{
    internal class RetrieveHistoryCommand : IRouteCommand
    {
        private readonly ITournamentController _tournamentController;
        private readonly User _user;

        public RetrieveHistoryCommand(ITournamentController tournamentController, User user)
        {
            _tournamentController = tournamentController;
            _user = user;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                List<TournamentEntry> entries = _tournamentController.GetUserHistory(_user.Token);
                var jsonPayload = JsonConvert.SerializeObject(entries);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (NoContentException)
            {
                response = new HttpResponse(StatusCode.NoContent);
            }

            return response;
        }
    }
}
