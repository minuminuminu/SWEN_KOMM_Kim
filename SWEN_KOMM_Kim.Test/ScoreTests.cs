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
    internal class ScoreTests
    {
        private Mock<IStatsDao> _mockStatsDao;
        private IStatsController _statsController;

        [SetUp]
        public void Setup()
        {
            _mockStatsDao = new Mock<IStatsDao>();
            _statsController = new StatsController(_mockStatsDao.Object);
        }

        [Test]
        public void CreateUserStatsEntry_Success()
        {
            string authToken = "someAuthToken";

            _statsController.CreateUserStatsEntry(authToken);

            _mockStatsDao.Verify(dao => dao.CreateUserStatsEntry(authToken), Times.Once);
        }

        [Test]
        public void IncreaseStats_Success()
        {
            int elo = 50;
            int pushups = 10;
            string authToken = "someAuthToken";

            _statsController.IncreaseStats(elo, pushups, authToken);

            _mockStatsDao.Verify(dao => dao.IncreaseStats(elo, pushups, authToken), Times.Once);
        }

        [Test]
        public void RetrieveUserStats_UserNotFound()
        {
            string authToken = "nonExistingAuthToken";
            _mockStatsDao.Setup(dao => dao.RetrieveUserStats(authToken)).Returns((UserStats?)null);

            Assert.Throws<UserNotFoundException>(() => _statsController.RetrieveUserStats(authToken));
        }

        [Test]
        public void RetrieveUserStats_Success()
        {
            string authToken = "existingAuthToken";
            var expectedStats = new UserStats("testUser", 100, 0);
            _mockStatsDao.Setup(dao => dao.RetrieveUserStats(authToken)).Returns(expectedStats);

            var result = _statsController.RetrieveUserStats(authToken);

            Assert.That(expectedStats, Is.EqualTo(result));
        }
    }
}
