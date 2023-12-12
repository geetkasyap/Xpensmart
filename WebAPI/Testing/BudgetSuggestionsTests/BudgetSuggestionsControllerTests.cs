using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;

namespace Testing.BudgetSuggestionsTests
{
    public class BudgetSuggestionsControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private BudgetSuggestionsController _budgetSuggestionsController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _budgetSuggestionsController = new BudgetSuggestionsController(_mockUnitOfWork.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _budgetSuggestionsController.Dispose();
        }

        [Test]
        [TestCase(1)]
        public async Task GetBudgetSuggestions_ReturnsSuggestions_WhenDataExists(int userId)
        {
            var suggestions = new List<string> { "HEY USER", "WELCOME GEET" };

            var expenses = new List<ExpensesEntity>();
            var budgets = new BudgetsEntity();

            _mockUnitOfWork.Setup(u => u.BudgetSuggestions.GetExpensesByUserIdAsync(userId)).ReturnsAsync(expenses);
            _mockUnitOfWork.Setup(u => u.BudgetSuggestions.CheckPresentBudgets(userId)).Returns(budgets);

            var result = await _budgetSuggestionsController.GetBudgetSuggestions(userId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        //[Test]
        //public async Task GetBudgetSuggestions_ReturnsFailure_WhenNoDataExists()
        //{
        //    int userId = 1;
        //    _mockUnitOfWork.Setup(u => u.BudgetSuggestions.GetBudgetSuggestionsAsync(userId)).ReturnsAsync(new List<string>());

        //    var result = await _budgetSuggestionsController.GetBudgetSuggestions(userId) as OkObjectResult;
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(200, result.StatusCode);
        //    Assert.AreEqual("Failure", result.Value.GetType().GetProperty("status").GetValue(result.Value));
        //    Assert.AreEqual("Insufficient data for suggestions.", result.Value.GetType().GetProperty("data").GetValue(result.Value));
        //}

        //[Test]
        //public async Task GetBudgetSuggestions_ReturnsServerError_OnException()
        //{
        //    int userId = 1;
        //    _mockUnitOfWork.Setup(u => u.BudgetSuggestions.GetBudgetSuggestionsAsync(userId)).ThrowsAsync(new Exception("Test exception"));
        //    var result = await _budgetSuggestionsController.GetBudgetSuggestions(userId) as ObjectResult;

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(500, result.StatusCode);
        //    Assert.AreEqual("Test exception", result.Value.GetType().GetProperty("Error").GetValue(result.Value));
        //}
    }
}