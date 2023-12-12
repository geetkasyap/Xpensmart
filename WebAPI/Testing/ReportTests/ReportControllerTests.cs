using ApplicationLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using CoreModels.Entities;
using CoreModels.DTO;
using DataAccessLayer.Infrastructure;

namespace Testing.ReportTests
{
    public class ReportControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ReportService> _mockService;
        private Mock<ILogger<ReportController>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private ReportController _reportController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<ReportController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockService = new Mock<ReportService>();
            _reportController = new ReportController(_mockUnitOfWork.Object, null, _mockConfiguration.Object, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _reportController.Dispose();
        }


        [Test]
        public async Task GenerateTotalExpenseReport_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            decimal expectedAmount = 1000;
            _mockUnitOfWork.Setup(u => u.Report.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId)).Returns(expectedAmount);

            var result = _reportController.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId) as OkObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GenerateTotalExpenseReport_ReturnsBadRequest_OnException()
        {

            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId)).Throws(new Exception("Test exception"));


            var result = _reportController.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId) as BadRequestObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GenerateAverageExpenseReport_ReturnsSuccess_WhenValidRequest()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            decimal expectedAmount = 500;
            _mockUnitOfWork.Setup(u => u.Report.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId)).Returns(expectedAmount);

            var result = _reportController.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId) as OkObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GenerateAverageExpenseReport_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId)).Throws(new Exception("Test exception"));

            var result = _reportController.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId) as BadRequestObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetTopThreeRecentExpenses_ReturnsSuccess_WhenDataExists()
        {

            int userId = 1;
            var recentExpenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 1, Amount = 100 },
                new ExpensesEntity { ExpenseID = 2, Amount = 200 }
            };
            _mockUnitOfWork.Setup(u => u.Report.GetTopThreeRecentExpenses(userId)).Returns(recentExpenses);


            var result = _reportController.GetTopThreeRecentExpenses(userId) as OkObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

        }

        [Test]
        public async Task GetTopThreeRecentExpenses_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GetTopThreeRecentExpenses(userId)).Throws(new Exception("Test exception"));

            var result = _reportController.GetTopThreeRecentExpenses(userId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }


        [Test]
        public async Task GetUpcomingExpenses_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            var upcomingExpenses = new List<ExpensesEntity>
    {
        new ExpensesEntity { ExpenseID = 1, Amount = 100 },
        new ExpensesEntity { ExpenseID = 2, Amount = 200 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetUpcomingExpenses(userId)).Returns(upcomingExpenses);

            var result = _reportController.GetUpcomingExpenses(userId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetUpcomingExpenses_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GetUpcomingExpenses(userId)).Throws(new Exception("Test exception"));

            var result = _reportController.GetUpcomingExpenses(userId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetRemainingExpenses_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            var remainingExpenses = new List<ExpensesEntity>
    {
        new ExpensesEntity { ExpenseID = 1, Amount = 100 },
        new ExpensesEntity { ExpenseID = 2, Amount = 200 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetRemainingExpenses(userId)).Returns(remainingExpenses);

            var result = _reportController.GetRemainingExpenses(userId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetRemainingExpenses_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GetRemainingExpenses(userId)).Throws(new Exception("Test exception"));

            var result = _reportController.GetRemainingExpenses(userId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetExpensesByDates_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            var expenses = new List<ExpensesEntity>
    {
        new ExpensesEntity { ExpenseID = 1, Amount = 100 },
        new ExpensesEntity { ExpenseID = 2, Amount = 200 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetExpensesByDates(userId, startDate, endDate)).Returns(expenses);

            var result = _reportController.GetExpensesByDates(userId, startDate, endDate) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetExpensesByDates_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            _mockUnitOfWork.Setup(u => u.Report.GetExpensesByDates(userId, startDate, endDate)).Throws(new Exception("Test exception"));

            var result = _reportController.GetExpensesByDates(userId, startDate, endDate) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetBudgetByDates_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            decimal expectedBudget = 500;
            _mockUnitOfWork.Setup(u => u.Report.GetBudgetByDates(userId, startDate, endDate)).Returns(expectedBudget);

            var result = _reportController.GetBudgetByDates(userId, startDate, endDate) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetBudgetByDates_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            _mockUnitOfWork.Setup(u => u.Report.GetBudgetByDates(userId, startDate, endDate)).Throws(new Exception("Test exception"));

            var result = _reportController.GetBudgetByDates(userId, startDate, endDate) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetCategoryExpensesByDates_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            var expenses = new List<ExpensesEntity>
    {
        new ExpensesEntity { ExpenseID = 1, Amount = 100 },
        new ExpensesEntity { ExpenseID = 2, Amount = 200 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetCategoryExpensesByDates(userId, startDate, endDate, categoryId)).Returns(expenses);

            var result = _reportController.GetCategoryExpensesByDates(userId, startDate, endDate, categoryId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetCategoryExpensesByDates_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GetCategoryExpensesByDates(userId, startDate, endDate, categoryId)).Throws(new Exception("Test exception"));

            var result = _reportController.GetCategoryExpensesByDates(userId, startDate, endDate, categoryId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetCategorySummary_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            var summary = new List<CategoryExpense>
    {
        new CategoryExpense { CategoryId = 1, CategoryName = "Food", Amount = 100 },
        new CategoryExpense { CategoryId = 2, CategoryName = "Transport", Amount = 200 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetCategorySummary(userId, startDate, endDate)).Returns(summary);

            var result = _reportController.GetCategorySummary(userId, startDate, endDate) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetCategorySummary_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            _mockUnitOfWork.Setup(u => u.Report.GetCategorySummary(userId, startDate, endDate)).Throws(new Exception("Test exception"));

            var result = _reportController.GetCategorySummary(userId, startDate, endDate) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetCategoryBudgets_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            var budgets = new List<CategoryBudget>
    {
        new CategoryBudget { CategoryId = categoryId, BudgetAmount = 500, Amount = 300, Percentage = 60 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetCategoryBudgets(userId, startDate, endDate, categoryId)).Returns(budgets);

            var result = _reportController.GetCategoryBudgets(userId, startDate, endDate, categoryId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetCategoryBudgets_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            _mockUnitOfWork.Setup(u => u.Report.GetCategoryBudgets(userId, startDate, endDate, categoryId)).Throws(new Exception("Test exception"));

            var result = _reportController.GetCategoryBudgets(userId, startDate, endDate, categoryId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetMonthlyBudgets_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            var monthlyBudgets = new List<MonthlyBudgets>
    {
        new MonthlyBudgets { Month = "2023-09", Amount = 1000 }
    };
            _mockUnitOfWork.Setup(u => u.Report.GetMonthlyBudgetsByDateRange(userId, startDate, endDate)).Returns(monthlyBudgets);

            var result = _reportController.GetMonthlyBudgets(userId, startDate, endDate) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetMonthlyBudgets_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            _mockUnitOfWork.Setup(u => u.Report.GetMonthlyBudgetsByDateRange(userId, startDate, endDate)).Throws(new Exception("Test exception"));

            var result = _reportController.GetMonthlyBudgets(userId, startDate, endDate) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task ProjectExpenses_ReturnsSuccess_WhenValidRequest()
        {
            try
            {
                var request = new ExpenseProjectionRequest { UserId = 1, Income = 5000 };
                var projection = new ExpenseProjection { UserId = 1, ProjectedIncome = 5000, Categories = new List<CategoryBudgetProjection>() };
                var categories = new List<CategoriesEntity> { new CategoriesEntity { CategoryID = 1, Name = "Test", IsActive = true } };
                var previousMonthExpenses = new List<ExpensesEntity> { new ExpensesEntity { ExpenseID = 1, UserID = 1, Date = DateTime.Now, Amount = 100, CategoryID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now } };

                _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Returns(categories);
                _mockUnitOfWork.Setup(u => u.Report.GetExpensesByDates(1, DateTime.Now, DateTime.Now)).Returns(previousMonthExpenses);
                _mockUnitOfWork.Setup(u => u.Report.GetCategoryExpensesByDates(1, DateTime.Now, DateTime.Now, 1)).Returns(previousMonthExpenses);
                var result = await _reportController.ProjectExpenses(request) as OkObjectResult;
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User  not found."));
            }
        }

        [Test]
        public async Task ProjectExpenses_ReturnsBadRequest_OnException()
        {
            var request = new ExpenseProjectionRequest { UserId = 1, Income = 5000 };
            var projection = new ExpenseProjection { UserId = 1, ProjectedIncome = 5000, Categories = new List<CategoryBudgetProjection>() };
            var categories = new List<CategoriesEntity> { new CategoriesEntity { CategoryID = 1, Name = "Test", IsActive = true } };
            var previousMonthExpenses = new List<ExpensesEntity> { new ExpensesEntity { ExpenseID = 1, UserID = 1, Date = DateTime.Now, Amount = 100, CategoryID = 1, IsActive = true, CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now } };

            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Throws(new Exception("Test exception"));
            _mockUnitOfWork.Setup(u => u.Report.GetExpensesByDates(1, DateTime.Now, DateTime.Now)).Throws(new Exception("Test exception"));

            var result = await _reportController.ProjectExpenses(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(400, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetPresentMonthExpenses_ReturnsSuccess_WhenDataExists()
        {
            int userId = 1;
            var presentMonthExpenses = new Dictionary<int, decimal>
    {
        { 1, 100 },
        { 2, 200 }
    };
            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Returns(new List<CategoriesEntity>
    {
        new CategoriesEntity { CategoryID = 1, Name = "Food" },
        new CategoriesEntity { CategoryID = 2, Name = "Transport" }
    });
            _mockUnitOfWork.Setup(u => u.Report.GetCategoryExpensesByDates(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns((int userId, DateTime start, DateTime end, int categoryId) => new List<ExpensesEntity>
                {
            new ExpensesEntity { Amount = 100 },
            new ExpensesEntity { Amount = 200 }
                });

            var result = _reportController.GetPresentMonthExpenses(userId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetPresentMonthExpenses_ReturnsBadRequest_OnException()
        {
            int userId = 1;
            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Throws(new Exception("Test exception"));

            var result = _reportController.GetPresentMonthExpenses(userId) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

    }
}

