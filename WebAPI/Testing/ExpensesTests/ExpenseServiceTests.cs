using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels.DTO;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Infrastructure;

namespace Testing.ExpensesTests
{
    public class ExpenseServiceTests
    {
        private ExpensesService _expensesService;
        private Mock<IUnitOfWork> _unitOfWork;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _expensesService = new ExpensesService(_unitOfWork.Object);
        }

        [Test]
        public async Task AddExpensesAsync_Success()
        {
            var expensesEntity = new ExpensesEntity
            {
                UserID = 1,
                Amount = 100,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Test",
                ExpenseType = "Test",
                IsActive = true,
                IsRecurring = false,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "test",
                ModifiedBy = "test"
            };

            _unitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync(expensesEntity);

            var result = await _expensesService.AddExpensesAsync(expensesEntity);

            Assert.IsInstanceOf<ExpensesEntity>(result);
            Assert.AreEqual(expensesEntity, result);
        }


        [Test]
        public async Task DeleteExpenses_Success()
        {
            int expenseId = 1;
            var deletedExpense = new ExpensesEntity();

            _unitOfWork.Setup(u => u.Expenses.DeleteExpenses(expenseId)).ReturnsAsync(deletedExpense);

            var result = await _expensesService.DeleteExpenses(expenseId);

            Assert.IsNotNull(result);
            Assert.AreEqual(deletedExpense, result);
        }

        [Test]
        public void GetExpensesDetails_Success()
        {
            var expectedExpenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 1, Amount = 100 },
                new ExpensesEntity { ExpenseID = 2, Amount = 200 }
            };

            _unitOfWork.Setup(u => u.Expenses.GetExpensesDetails()).Returns(expectedExpenses);

            var result = _expensesService.GetExpensesDetails();

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedExpenses.Count, result.Count);
        }

        [Test]
        public void GetExpensesDetails_ById_Success()
        {
            int expenseId = 1;
            var expectedExpense = new ExpensesEntity { ExpenseID = expenseId, Amount = 100 };

            _unitOfWork.Setup(u => u.Expenses.GetExpensesDetails(expenseId)).Returns(expectedExpense);

            var result = _expensesService.GetExpensesDetails(expenseId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedExpense, result);
        }

        [Test]
        public async Task UpdateExpenses_Success()
        {
            int expenseId = 1;
            var updatedExpense = new ExpensesEntity { ExpenseID = expenseId, Amount = 150 };

            _unitOfWork.Setup(u => u.Expenses.UpdateExpenses(expenseId, It.IsAny<int>(), updatedExpense)).Returns(updatedExpense);

            var result = _expensesService.UpdateExpenses(expenseId, 1, updatedExpense);

            Assert.IsNotNull(result);
            Assert.AreEqual(updatedExpense, result);
        }

        [Test]
        public void GenerateReport_Success()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            decimal expectedReport = 100;

            _unitOfWork.Setup(u => u.Expenses.GenerateReport(userId, startDate, endDate, categoryId)).Returns(expectedReport);

            var result = _expensesService.GenerateReport(userId, startDate, endDate, categoryId);

            Assert.AreEqual(expectedReport, result);
        }

        [Test]
        public void GetUpcomingExpenses_Success()
        {
            int userId = 1;
            var expectedUpcomingExpenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 1, Amount = 100 },
                new ExpensesEntity { ExpenseID = 2, Amount = 200 }
            };

            _unitOfWork.Setup(u => u.Expenses.GetUpcomingExpenses(userId)).Returns(expectedUpcomingExpenses);

            var result = _expensesService.GetUpcomingExpenses(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUpcomingExpenses.Count, result.Count);
        }

        [Test]
        public void ConvertCurrency_Success()
        {
            decimal amount = 100;
            string fromCurrency = "USD";
            string toCurrency = "INR";
            decimal convertedAmount = 85;

            _unitOfWork.Setup(u => u.Expenses.ConvertCurrency(amount, fromCurrency, toCurrency)).Returns(convertedAmount);

            var result = _expensesService.ConvertCurrency(amount, fromCurrency, toCurrency);

            Assert.AreEqual(convertedAmount, result);
        }

        [Test]
        public async Task ImportExpensesAsync_Success()
        {
            var expenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { UserID = 1, Amount = 100, Date = DateTime.Now, CategoryID = 1 },
                new ExpensesEntity { UserID = 1, Amount = 200, Date = DateTime.Now, CategoryID = 2 }
            };

            _unitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync((ExpensesEntity e) => e);

            await _expensesService.ImportExpensesAsync(expenses);

            _unitOfWork.Verify(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>()), Times.Exactly(expenses.Count));
        }

        [Test]
        public async Task SplitExpenseAsync_Success()
        {
            var splitRequest = new ExpenseSplitRequest
            {
                UserNames = new List<string> { "Geet", "Kasyap" },
                TotalAmount = 100,
                NumberOfMembers = 2
            };

            int userId = 1;
            int categoryId = 1;
            string fileUrl = "fileUrl";
            decimal individualShare = splitRequest.TotalAmount / splitRequest.NumberOfMembers;

            var username = _unitOfWork.Setup(u => u.Users.GetEmailByUserID(userId)).Returns("geet123@example.invalid");
            _unitOfWork.Setup(u => u.Users.GetUserByUserNameAsync("Geet")).ReturnsAsync(
                new UsersEntity 
                {
                UserID = 1,
                UserName = "Geet",
                Email = "geet123@example.invalid",
                Password = "Test@1234",
                FullName = "Geet Kasyap",
                GroupID = 0,
                IsActive = true,
                BudgetsEntities = new List<BudgetsEntity>(),
                ExpenseProjectionEntities = new List<ExpenseProjectionEntity>(),
                ExpensesEntities = new List<ExpensesEntity>(),
                NotificationsEntities = new List<NotificationsEntity>(),
                ReportsEntities = new List<CategoryReport>(),
                SentRequest = new List<SharedAccountsEntity>(),
                ReceivedRequest = new List<SharedAccountsEntity>(),
                GroupMembersEntities = new List<GroupMembersEntity>(),
            });
            _unitOfWork.Setup(u => u.Users.GetUserByUserNameAsync("Kasyap")).ReturnsAsync(
                new UsersEntity 
                {
                UserID = 2,
                UserName = "Kasyap",
                Email = "kasyap123@example.invalid",
                Password = "Sample@1234",
                FullName = "Kasyap",
                GroupID = 0,
                IsActive = true,
                BudgetsEntities = new List<BudgetsEntity>(),
                ExpenseProjectionEntities = new List<ExpenseProjectionEntity>(),
                ExpensesEntities = new List<ExpensesEntity>(),
                NotificationsEntities = new List<NotificationsEntity>(),
                ReportsEntities = new List<CategoryReport>(),
                SentRequest = new List<SharedAccountsEntity>(),
                ReceivedRequest = new List<SharedAccountsEntity>(),
                GroupMembersEntities = new List<GroupMembersEntity>(),
            });
            var GeetExpense = new ExpensesEntity
            {
                UserID = 1,
                Amount = individualShare,
                Date = DateTime.Now,
                CategoryID = categoryId,
                ReceiptUrl = "fileurl",
                Description = $"Split expense by {username}",
                ExpenseType = "Split",
                IsActive = true,
                IsRecurring = false,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "geet123@example.invalid",
                ModifiedBy = "geet123@example.invalid"
            };

            var KasyapExpense = new ExpensesEntity
            {
                UserID = 2,
                Amount = individualShare,
                Date = DateTime.Now,
                CategoryID = categoryId,
                ReceiptUrl = "fileurl",
                Description = $"Split expense by {username}",
                ExpenseType = "Split",
                IsActive = true,
                IsRecurring = false,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "kasyap123@example.invalid",
                ModifiedBy = "kasyap123@example.invalid"
            };
            var expenseEntries = new List<ExpensesEntity> { GeetExpense, KasyapExpense };

            foreach (var expense in expenseEntries)
            {
                _unitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync(expense);
            }

            var result = await _expensesService.SplitExpenseAsync(splitRequest, userId, categoryId, fileUrl);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

    }
}