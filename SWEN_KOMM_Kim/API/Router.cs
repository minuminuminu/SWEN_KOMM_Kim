using Newtonsoft.Json;
using SWEN_KOMM_Kim.API.Routing.StatsCommands;
using SWEN_KOMM_Kim.API.Routing.TournamentCommands;
using SWEN_KOMM_Kim.API.Routing.UserCommands;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.HttpServer.Request;
using SWEN_KOMM_Kim.HttpServer.Routing;
using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpMethod = SWEN_KOMM_Kim.HttpServer.Request.HttpMethod;

namespace SWEN_KOMM_Kim.API
{
    internal class Router
    {
        private readonly IdentityProvider _identityProvider;
        private readonly IdRouteParser _routeParser;

        private readonly IUserManager _userManager;
        private readonly IStatsManager _statsManager;
        private readonly ITournamentManager _tournamentManager;

        public Router(IUserManager userManager, IStatsManager statsManager, ITournamentManager tournamentManager)
        {
            _identityProvider = new IdentityProvider(userManager);
            _routeParser = new IdRouteParser();
            _userManager = userManager;
            _statsManager = statsManager;
            _tournamentManager = tournamentManager;
        }

        public IRouteCommand? Resolve(HttpRequest request)
        {
            var isMatch = (string path) => _routeParser.IsMatch(path, "/users/{id}");
            var parseId = (string path) => _routeParser.ParseParameters(path, "/users/{id}")["id"];
            var checkBody = (string? payload) => payload ?? throw new InvalidDataException();

            try
            {
                return request switch
                {
                    { Method: HttpMethod.Post, ResourcePath: "/users" } => new RegisterCommand(_userManager, _statsManager, Deserialize<Credentials>(request.Payload)),
                    { Method: HttpMethod.Post, ResourcePath: "/sessions" } => new LoginCommand(_userManager, Deserialize<Credentials>(request.Payload)),
                    { Method: HttpMethod.Put, ResourcePath: var path } when isMatch(path) => new UpdateUserDataCommand(_userManager, parseId(path), GetIdentity(request), Deserialize<UserData>(request.Payload)),
                    { Method: HttpMethod.Get, ResourcePath: var path } when isMatch(path) => new RetrieveUserDataCommand(_userManager, parseId(path), GetIdentity(request)),

                    { Method: HttpMethod.Get, ResourcePath: "/stats" } => new RetrieveUserStatsCommand(GetIdentity(request), _statsManager),
                    { Method: HttpMethod.Get, ResourcePath: "/score" } => new RetrieveScoreboardCommand(_statsManager, GetIdentity(request)),

                    { Method: HttpMethod.Get, ResourcePath: "/history" } => new RetrieveHistoryCommand(_tournamentManager, GetIdentity(request)),
                    { Method: HttpMethod.Get, ResourcePath: "/tournament" } => new RetrieveTournamentStateCommand(_tournamentManager, GetIdentity(request)),
                    { Method: HttpMethod.Post, ResourcePath: "/history" } => new AddHistoryEntryCommand(_tournamentManager, GetIdentity(request), Deserialize<PayloadEntry>(request.Payload)),

                    _ => null
                };
            }
            catch (InvalidDataException)
            {
                return null;
            }
        }

        private T Deserialize<T>(string? body) where T : class
        {
            var data = body is not null ? JsonConvert.DeserializeObject<T>(body) : null;
            return data ?? throw new InvalidDataException();
        }

        private User GetIdentity(HttpRequest request)
        {
            return _identityProvider.GetIdentityForRequest(request) ?? throw new RouteNotAuthenticatedException();
        }
    }
}
