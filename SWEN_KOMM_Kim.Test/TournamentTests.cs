using SWEN_KOMM_Kim.Models;
using NUnit.Framework;
using System;
using Moq;
using SWEN_KOMM_Kim.DAL.Interfaces;
using SWEN_KOMM_Kim.DAL.DAOs;
using SWEN_KOMM_Kim.BLL.Interfaces;
using SWEN_KOMM_Kim.BLL.Managers;
using SWEN_KOMM_Kim.Exceptions;

namespace SWEN_KOMM_Kim.Test
{
    internal class TournamentTests
    {
        private Mock<ITournamentDao> _mockTournamentDao;
        private Mock<IStatsManager> _mockStatsManager;
        private TournamentManager _tournamentManager;

        [SetUp]
        public void Setup()
        {
            _mockTournamentDao = new Mock<ITournamentDao>();
            _mockStatsManager = new Mock<IStatsManager>();
            _tournamentManager = new TournamentManager(_mockTournamentDao.Object, _mockStatsManager.Object);
        }

        [Test]
        public void GetUserHistory_NoContentExceptionThrown()
        {
            // Arrange
            _mockTournamentDao.Setup(x => x.GetUserHistory("invalid-token")).Returns(new List<TournamentEntry>());

            // Act & Assert
            Assert.Throws<NoContentException>(() => _tournamentManager.GetUserHistory("invalid-token"));
        }

        [Test]
        public void GetTournamentStateByAuthToken_NoContentExceptionThrown()
        {
            // Arrange
            _mockTournamentDao.Setup(x => x.IsUserInTournament("invalid-token")).Returns(false);

            // Act & Assert
            Assert.Throws<NoContentException>(() => _tournamentManager.GetTournamentStateByAuthToken("invalid-token"));
        }
    }
}
