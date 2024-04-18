using SWEN_KOMM_Kim.Models;
using NUnit.Framework;
using System;
using Moq;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.DAL.DAOs;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.BLL.Controllers;
using SWEN_KOMM_Kim.Exceptions;

namespace SWEN_KOMM_Kim.Test
{
    internal class TournamentTests
    {
        private Mock<ITournamentDao> _mockTournamentDao;
        private Mock<IStatsController> _mockStatsController;
        private TournamentController _tournamentController;

        [SetUp]
        public void Setup()
        {
            _mockTournamentDao = new Mock<ITournamentDao>();
            _mockStatsController = new Mock<IStatsController>();
            _tournamentController = new TournamentController(_mockTournamentDao.Object, _mockStatsController.Object);
        }

        [Test]
        public void GetUserHistory_NoContentExceptionThrown()
        {
            _mockTournamentDao.Setup(x => x.GetUserHistory("invalid-token")).Returns(new List<TournamentEntry>());

            Assert.Throws<NoContentException>(() => _tournamentController.GetUserHistory("invalid-token"));
        }

        [Test]
        public void GetTournamentStateByAuthToken_NoContentExceptionThrown()
        {
            _mockTournamentDao.Setup(x => x.IsUserInTournament("invalid-token")).Returns(false);

            Assert.Throws<NoContentException>(() => _tournamentController.GetTournamentStateByAuthToken("invalid-token"));
        }
    }
}
