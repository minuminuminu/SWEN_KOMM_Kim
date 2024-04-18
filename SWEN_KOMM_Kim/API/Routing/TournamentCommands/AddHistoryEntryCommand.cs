using SWEN_KOMM_Kim.BLL.Interfaces;
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
    internal class AddHistoryEntryCommand : IRouteCommand
    {
        private readonly ITournamentController _tournamentController;
        private readonly User _user;
        private readonly PayloadEntry _payloadEntry;

        public AddHistoryEntryCommand(ITournamentController tournamentController, User user, PayloadEntry payloadEntry)
        {
            _tournamentController = tournamentController;
            _payloadEntry = payloadEntry;
            _user = user;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            TournamentEntry entry = new(_payloadEntry.Count, _payloadEntry.DurationInSeconds, _user.Username);
            _tournamentController.HandleTournamentEntry(entry, _payloadEntry.Name);
            response = new HttpResponse(StatusCode.Ok);

            return response;
        }
    }
}
