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

        private readonly IUserController _userController;
        private readonly IStatsController _statsController;
        private readonly ITournamentController _tournamentController;

        public Router(IUserController userController, IStatsController statsController, ITournamentController tournamentController)
        {
            _identityProvider = new IdentityProvider(userController);
            _routeParser = new IdRouteParser();
            _userController = userController;
            _statsController = statsController;
            _tournamentController = tournamentController;
        }

        public IRouteCommand? Resolve(HttpRequest request)
        {
            var isMatch = (string path, string route) => _routeParser.IsMatch(path, "/" + route + "/{id}");
            var parseId = (string path, string route) => _routeParser.ParseParameters(path, "/" + route + "/{id}")["id"];
            var checkBody = (string? payload) => payload ?? throw new InvalidDataException();

            try
            {
                return request switch
                {
                    { Method: HttpMethod.Post, ResourcePath: "/users" } => new RegisterCommand(_userController, _statsController, Deserialize<Credentials>(request.Payload)),
                    { Method: HttpMethod.Post, ResourcePath: "/sessions" } => new LoginCommand(_userController, Deserialize<Credentials>(request.Payload)),
                    { Method: HttpMethod.Put, ResourcePath: var path } when isMatch(path, "users") => new UpdateUserDataCommand(_userController, parseId(path, "users"), GetIdentity(request), Deserialize<UserData>(request.Payload)),
                    { Method: HttpMethod.Get, ResourcePath: var path } when isMatch(path, "users") => new RetrieveUserDataCommand(_userController, parseId(path, "users"), GetIdentity(request)),

                    { Method: HttpMethod.Get, ResourcePath: "/stats" } => new RetrieveUserStatsCommand(GetIdentity(request), _statsController),
                    { Method: HttpMethod.Get, ResourcePath: "/score" } => new RetrieveScoreboardCommand(_statsController, GetIdentity(request)),

                    { Method: HttpMethod.Get, ResourcePath: "/history" } => new RetrieveHistoryCommand(_tournamentController, GetIdentity(request)),
                    { Method: HttpMethod.Get, ResourcePath: "/tournament" } => new RetrieveTournamentStateCommand(_tournamentController, GetIdentity(request)),
                    { Method: HttpMethod.Post, ResourcePath: "/history" } => new AddHistoryEntryCommand(_tournamentController, GetIdentity(request), Deserialize<PayloadEntry>(request.Payload)),

                    // unique feature
                    { Method: HttpMethod.Put, ResourcePath: var path } when isMatch(path, "edit") => new UpdateUserCredentialsCommand(_userController, Deserialize<User>(request.Payload), GetIdentity(request), parseId(path, "edit")),

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
