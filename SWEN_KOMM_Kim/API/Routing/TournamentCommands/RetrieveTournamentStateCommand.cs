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
    internal class RetrieveTournamentStateCommand : IRouteCommand
    {
        private readonly ITournamentController _tournamentController;
        private readonly User _user;

        public RetrieveTournamentStateCommand(ITournamentController tournamentController, User user)
        {
            _tournamentController = tournamentController;
            _user = user;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                TournamentState tournamentState = _tournamentController.GetTournamentStateByAuthToken(_user.Token);
                //List<string> test = new List<string> { "admin-sebToken", "kienboec-sebToken" };
                //TournamentState tournamentState = new TournamentState(3, test, DateTime.Now);
                var jsonPayload = JsonConvert.SerializeObject(tournamentState);
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
