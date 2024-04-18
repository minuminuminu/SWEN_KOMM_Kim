using SWEN_KOMM_Kim.API;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.BLL.Controllers;
using SWEN_KOMM_Kim.DAL.DAOs;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.HttpServer;
using System.Net;

namespace SWEN_KOMM_Kim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Host=localhost;Username=minu_seb;Password=minuminuminu;Database=db_seb";

            IUserDao userDao = new UserDao(connectionString);
            IStatsDao statsDao = new StatsDao(connectionString);
            ITournamentDao tournamentDao = new TournamentDao(connectionString);

            IUserController userController = new UserController(userDao);
            IStatsController statsController = new StatsController(statsDao);
            ITournamentController tournamentController = new TournamentController(tournamentDao, statsController);

            var router = new Router(userController, statsController, tournamentController);
            var server = new HttpServer.HttpServer(router, IPAddress.Any, 10001);
            Console.WriteLine("Server is running on port 10001.");
            server.Start();
        }
    }
}
