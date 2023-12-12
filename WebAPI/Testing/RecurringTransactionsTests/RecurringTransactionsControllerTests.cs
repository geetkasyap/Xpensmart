using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using CoreModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;
using Microsoft.Extensions.Configuration;

namespace Testing.RecurringTransactionsTests
{
    public class RecurringTransactionsControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<RecurringTransactionsController>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private RecurringTransactionsController _recurringTransactionsController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<RecurringTransactionsController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _recurringTransactionsController = new RecurringTransactionsController(_mockUnitOfWork.Object, _mockConfiguration.Object, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _recurringTransactionsController.Dispose();
        }

        [Test]
        public async Task AddTransactions_ReturnsSuccess_WhenValidRequest()
        {
          
            var request = new ExpensesEntity
            {
                Amount = 100,
                CategoryID = 1,
                ExpenseType = "Recurring",
                ReceiptUrl = "url"
            };
            _mockUnitOfWork.Setup(u => u.Expenses.AddTransactionsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(request);

            var result = await _recurringTransactionsController.AddTransactions(DateTime.Now, DateTime.Now.AddMonths(1), "Recurring", 1, 1, 100, "url") as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task AddTransactions_ReturnsFailure_WhenRequestIsInvalid()
        {
            ExpensesEntity request = null;

            var result = await _recurringTransactionsController.AddTransactions(DateTime.Now, DateTime.Now.AddMonths(1), "Recurring", 1, 1, 100, "url") as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task AddTransactions_ReturnsServerError_OnException()
        {

            _mockUnitOfWork.Setup(u => u.Expenses.AddTransactionsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Throws(new Exception("Test exception"));

            var result = await _recurringTransactionsController.AddTransactions(DateTime.Now, DateTime.Now.AddMonths(1), "Recurring", 1, 1, 100, "url") as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        //[Test]
        //public async Task GetTransactionDetails_ReturnsTransactions_WhenDataExists()
        //{
        //    var transactions = new List<ExpensesEntity>
        //    {
        //        new ExpensesEntity { ExpenseID = 1, Amount = 100, ExpenseType = "Recurring" },
        //        new ExpensesEntity { ExpenseID = 2, Amount = 200, ExpenseType = "Recurring" }
        //    };
        //    _mockUnitOfWork.Setup(u => u.Expenses.GetTransactionsDetails(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
        //        .Returns(transactions);

        //    var result = await _recurringTransactionsController.GetTransactionDetails(DateTime.Now, "Recurring", 1) as OkObjectResult;

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(200, result.StatusCode);
        //    Assert.AreEqual(transactions, result.Value);
        //}

        [Test]
        public async Task GetTransactionDetails_ReturnsServerError_OnException()
        {
            _mockUnitOfWork.Setup(u => u.Expenses.GetTransactionsDetails(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception("Test exception"));

            var result = await _recurringTransactionsController.GetTransactionDetails(DateTime.Now, "Recurring", 1) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Test exception", result.Value.GetType().GetProperty("Error").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateTransaction_ReturnsSuccess_WhenValidId()
        {
            var updatedExpense = new ExpensesEntity { ExpenseID = 1, Amount = 150 };
            _mockUnitOfWork.Setup(u => u.Expenses.UpdateTransactions(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ExpensesEntity>()))
                .Returns(updatedExpense);

            var result = await _recurringTransactionsController.UpdateTransaction(1, updatedExpense, 1) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task UpdateTransaction_ReturnsServerError_OnException()
        {
            var updatedExpense = new ExpensesEntity { ExpenseID = 1, Amount = 150 };
            _mockUnitOfWork.Setup(u => u.Expenses.UpdateTransactions(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ExpensesEntity>()))
                .Throws(new Exception("Test exception"));

            var result = await _recurringTransactionsController.UpdateTransaction(1, updatedExpense, 1) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task DeleteTransactions_ReturnsSuccess_WhenValidId()
        {

            int expenseId = 1;
            _mockUnitOfWork.Setup(u => u.Expenses.DeleteTransactions(expenseId)).Returns(new ExpensesEntity());


            var result = await _recurringTransactionsController.DeleteTransactions(expenseId) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteTransactions_ReturnsServerError_OnException()
        {
            int expenseId = 1;
            _mockUnitOfWork.Setup(u => u.Expenses.DeleteTransactions(expenseId)).Throws(new Exception("Test exception"));

            var result = await _recurringTransactionsController.DeleteTransactions(expenseId) as GenericResponse;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
    }
}