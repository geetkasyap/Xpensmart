using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xpensmart.Controllers;
using Microsoft.Extensions.Configuration;

namespace Testing.BudgetsTests
{
    public class BudgetControllerTests
    {
        private BudgetController _budgetController;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IConfiguration> _configuration;
        private Mock<ILogger<BudgetController>> _budgetLogger;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _configuration = new Mock<IConfiguration>();
            _budgetLogger = new Mock<ILogger<BudgetController>>();

            _budgetController = new BudgetController(_unitOfWork.Object, _configuration.Object, _budgetLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _budgetController.Dispose();
        }

        [Test]
        public async Task AddBudget_Success()
        {
            var budgetEntity = new BudgetsEntity
            {
                UserID = 1,
                Amount = 1000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Frequency = "Monthly",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "test",
                ModifiedBy = "test"
            };

            _unitOfWork.Setup(u => u.Budget.AddBudget(It.IsAny<BudgetsEntity>())).Returns(budgetEntity);

            var result = await _budgetController.AddBudget(budgetEntity);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task AddBudget_NullEntity_Failure()
        {
            BudgetsEntity budgetEntity = null;

            var result = await _budgetController.AddBudget(budgetEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        public async Task GetBudgetDetails_ReturnsExpectedResults(int budgetID, int userID)
        {
            var JanBudget = new BudgetsEntity
            {
                BudgetID = 1,
                UserID = 1,
                Amount = 1000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                IsActive = true,
                Frequency = "Monthly",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "geet123@example.invalid",
                ModifiedBy = "geet123@example.invalid"
            };
            var FebBudget = new BudgetsEntity
            {
                BudgetID = 2,
                UserID = 1,
                Amount = 2000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                IsActive = true,
                Frequency = "Monthly",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "geet123@example.invalid",
                ModifiedBy = "geet123@example.invalid"
            };
            var expectedBudgetList = new List<BudgetsEntity> { JanBudget, FebBudget };

            if (budgetID == 0)
            {
                _unitOfWork.Setup(u => u.Budget.GetBudgetDetails()).Returns(expectedBudgetList);
                var result = await _budgetController.GetBudgetDetails(budgetID, userID);
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                var actualList = okResult.Value as List<BudgetsEntity>;
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedBudgetList.Count, actualList.Count);
            }
            else
            {
                _unitOfWork.Setup(u => u.Budget.GetBudgetDetails(budgetID)).Returns(JanBudget);
                var result = await _budgetController.GetBudgetDetails(budgetID, userID);
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.AreEqual(JanBudget, okResult.Value);
            }
        }

        [Test]
        [TestCase(1, 5000)]
        public async Task UpdateBudget_Success(int budgetID, int amount)
        {
            var updatedBudget = new BudgetsEntity
            {
                BudgetID = budgetID,
                UserID = 1,
                Amount = amount,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                IsActive = true,
                Frequency = "Monthly",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "geet123@example.invalid",
                ModifiedBy = "geet123@example.invalid"
            };

            _unitOfWork.Setup(u => u.Budget.UpdateBudget(budgetID, amount)).Returns(updatedBudget);
            var result = await _budgetController.UpdateBudget(budgetID, amount);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task UpdateBudget_Exception_Failure()
        {
            int budgetId = 0;
            decimal newAmount = 1500;
            _unitOfWork.Setup(u => u.Budget.UpdateBudget(budgetId, newAmount)).Throws(new Exception());
            var result = await _budgetController.UpdateBudget(budgetId, newAmount);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GetBudgetDetails_Exception_Failure()
        {
            _unitOfWork.Setup(u => u.Budget.GetBudgetDetails(1)).Throws(new Exception("Database error"));
            var result = await _budgetController.GetBudgetDetails(1, 0);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
        [Test]
        public async Task GetBudgetsByUser_Success()
        {
            int userId = 1;
            var budgetList = new List<BudgetsEntity>
    {
        new BudgetsEntity { BudgetID = 1, Amount = 1000 },
        new BudgetsEntity { BudgetID = 2, Amount = 2000 }
    };
            _unitOfWork.Setup(u => u.Budget.GetBudgetsByUser(userId)).Returns(budgetList);

            var result = await _budgetController.GetBudgetsByUser(userId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(budgetList, okResult.Value);
        }

        [Test]
        public async Task GetBudgetsByUser_Exception_Failure()
        {
            int userId = 1;
            _unitOfWork.Setup(u => u.Budget.GetBudgetsByUser(userId)).Throws(new Exception("Database error"));
            var result = await _budgetController.GetBudgetsByUser(userId);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
        [Test]
        public async Task DeleteBudgets_Success()
        {
            int budgetId = 1;
            _unitOfWork.Setup(u => u.Budget.DeleteBudget(budgetId)).Returns(new BudgetsEntity());
            var result = await _budgetController.DeleteBudgets(budgetId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteBudgets_Exception_Failure()
        {
            int budgetId = 0;
            _unitOfWork.Setup(u => u.Budget.DeleteBudget(budgetId)).Throws(new Exception());
            var result = await _budgetController.DeleteBudgets(budgetId);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
    }
}