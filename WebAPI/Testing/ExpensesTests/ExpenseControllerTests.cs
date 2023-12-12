using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpensmart.Controllers;
using CoreModels;
using CoreModels.DTO;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using DataAccessLayer.Infrastructure;

namespace Testing.ExpensesTests
{
        public class ExpenseControllerTests
        {
            private ExpensesController _expensesController;
            private Mock<CategoryService> _mockCategoryService;
            private Mock<ExpensesService> _mockExpensesService;
            private Mock<IUnitOfWork> _unitOfWork;
            private Mock<IConfiguration> _configuration;
            private Mock<ILogger<ExpensesController>> _expensesLogger;

            [SetUp]
            public void Setup()
            {
                _unitOfWork = new Mock<IUnitOfWork>();
                _configuration = new Mock<IConfiguration>();
                _expensesLogger = new Mock<ILogger<ExpensesController>>();
                _mockCategoryService = new Mock<CategoryService>();
            _mockExpensesService = new Mock<ExpensesService>();
                _expensesController = new ExpensesController(_unitOfWork.Object, _configuration.Object, _expensesLogger.Object);
            }

            [TearDown]
            public void TearDown()
            {
                _expensesController.Dispose();
            }

            [Test]
            public async Task AddExpenses_Success()
            {
                var expensesEntity = new ExpensesEntity
                {
                    UserID = 2,
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

                var result = await _expensesController.AddExpenses(expensesEntity);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

            [Test]
            public async Task AddExpenses_NullEntity_Failure()
            {
                var expensesEntity = null as ExpensesEntity;

                var result = await _expensesController.AddExpenses(expensesEntity);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        [TestCase(1, 1)]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        public async Task GetExpenseDetails_Success(int expenseId, int userID)
        {
            var expectedExpense = new ExpensesEntity
            {
                ExpenseID = 1,
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
            var expectedExpensesList = new List<ExpensesEntity> { expectedExpense };

            if (expenseId == 0 && userID == 0)
            {
                _unitOfWork.Setup(u => u.Expenses.GetExpensesDetails()).Returns(expectedExpensesList);
                var result = await _expensesController.GetExpenseDetails(expenseId, userID);
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                var actualList = okResult.Value as List<ExpensesEntity>;
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedExpensesList.Count, actualList.Count);
                Assert.AreEqual(expectedExpensesList[0], actualList[0]);
            }
            else if (expenseId == 0 && userID != 0)
            {
                _unitOfWork.Setup(u => u.Expenses.GetAllExpensesDetails(userID)).Returns(expectedExpensesList);
                var result = await _expensesController.GetExpenseDetails(expenseId, userID);
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                var actualList = okResult.Value as List<ExpensesEntity>;
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedExpensesList.Count, actualList.Count);
                Assert.AreEqual(expectedExpensesList[0], actualList[0]);
            }
            else
            {
                _unitOfWork.Setup(u => u.Expenses.GetExpensesDetails(expenseId)).Returns(expectedExpense);
                var result = await _expensesController.GetExpenseDetails(expenseId, userID);
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.AreEqual(expectedExpense, okResult.Value);
            }
        }
        [Test]
        public async Task GetExpenseDetails_Failure()
        {
            var expectedExpenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 1, Amount = 100 },
                new ExpensesEntity { ExpenseID = 2, Amount = 200 }
            };
            _unitOfWork.Setup(u => u.Expenses.GetExpensesDetails()).Returns(expectedExpenses);

            var result = await _expensesController.GetExpenseDetails(0, 0);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedExpenses, okResult.Value);
        }

        [Test]
        public async Task UpdateExpenses_Success()
        {
            int expenseId = 1;
            var updatedExpense = new ExpensesEntity { ExpenseID = expenseId, Amount = 150 };
            _unitOfWork.Setup(u => u.Expenses.UpdateExpenses(expenseId, It.IsAny<int>(), updatedExpense)).Returns(updatedExpense);

            var result = await _expensesController.UpdateExpenses(expenseId, updatedExpense, 1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task UpdateExpenses_MismatchedID_Failure()
        {
            int expenseId = 1;
            var updatedExpense = new ExpensesEntity { ExpenseID = 2 }; 

            var result = await _expensesController.UpdateExpenses(expenseId, updatedExpense, 1);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task DeleteExpenses_Success()
        {
            int expenseId = 1;
            var deletedExpense = new ExpensesEntity(); 

            _unitOfWork.Setup(u => u.Expenses.DeleteExpenses(expenseId)).ReturnsAsync(deletedExpense);

            var result = await _expensesController.DeleteExpenses(expenseId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }

        [Test]
        public async Task DeleteExpenses_Exception_Failure()
        {
            int expenseId = 0;

            _unitOfWork.Setup(u => u.Expenses.DeleteExpenses(expenseId)).Throws(new Exception());

            var result = await _expensesController.DeleteExpenses(expenseId);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }

        [Test]
        public async Task GenerateReport_Success()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;
            decimal expectedReport = 100;
            _unitOfWork.Setup(u => u.Expenses.GenerateReport(userId, startDate, endDate, categoryId)).Returns(expectedReport);

            var result = _expensesController.GenerateReport(userId, startDate, endDate, categoryId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedReport, okResult.Value);
        }

        [Test]
        public void GenerateReport_Exception_Failure()
        {
            int userId = 1;
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            int categoryId = 1;

            _unitOfWork.Setup(u => u.Expenses.GenerateReport(userId, startDate, endDate, categoryId)).Throws(new Exception("Report generation error"));

            var result = _expensesController.GenerateReport(userId, startDate, endDate, categoryId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetUpcomingExpenses_Success()
        {
            int userId = 1;
            var expectedUpcomingExpenses = new List<ExpensesEntity>
            {
                new ExpensesEntity { ExpenseID = 1, Amount = 100 },
                new ExpensesEntity { ExpenseID = 2, Amount = 200 }
            };
            _unitOfWork.Setup(u => u.Expenses.GetUpcomingExpenses(userId)).Returns(expectedUpcomingExpenses);

            var result = _expensesController.GetUpcomingExpenses(userId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedUpcomingExpenses, okResult.Value);
        }

        [Test]
        public void GetUpcomingExpenses_Exception_Failure()
        {
            int userId = 1;

            _unitOfWork.Setup(u => u.Expenses.GetUpcomingExpenses(userId)).Throws(new Exception());
            var result = _expensesController.GetUpcomingExpenses(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task ConvertCurrency_Success()
        {
            decimal amount = 100;
            string fromCurrency = "USD";
            string toCurrency = "INR";
            decimal convertedAmount = 85;
            _unitOfWork.Setup(u => u.Expenses.ConvertCurrency(amount, fromCurrency, toCurrency)).Returns(convertedAmount);

            var result = _expensesController.ConvertCurrency(amount, fromCurrency, toCurrency);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(convertedAmount, okResult.Value);
        }

        [Test]
        public void ConvertCurrency_Exception_Failure()
        {
            decimal amount = 100;
            string fromCurrency = "USD";
            string toCurrency = "INR";

            _unitOfWork.Setup(u => u.Expenses.ConvertCurrency(amount, fromCurrency, toCurrency)).Throws(new Exception("Currency conversion error"));

            var result = _expensesController.ConvertCurrency(amount, fromCurrency, toCurrency);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task SplitExpense_Success()
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
            var expense1 = new ExpensesEntity
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

            var expense2 = new ExpensesEntity
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
            var expenseEntries = new List<ExpensesEntity> { expense1, expense2 };

            foreach (var expense in expenseEntries)
            {
                _unitOfWork.Setup(u => u.Expenses.AddExpensesAsync(It.IsAny<ExpensesEntity>())).ReturnsAsync(expense);
            }

            var result = await _expensesController.SplitExpense(userId,categoryId,fileUrl,splitRequest);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.status);
        }


        [Test]
        public async Task SplitExpense_NullRequest_ReturnsFailure()
        {
            ExpenseSplitRequest splitRequest = null;

            var result = await _expensesController.SplitExpense(1, 1, "fileUrl", splitRequest);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.status);
        }
        [Test]
        public async Task BulkImport_ValidFile_ReturnsSuccess()
        {
            
            var userId = 1;
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.csv";
            var fileContent = "CategoryName,Amount,Date,ReceiptUrl,Description,ExpenseType\n" +
                              "Food,10.00,2023-01-01,http://example.com/receipt1,Description1,Type1\n" +
                              "Transport,20.00,2023-01-02,http://example.com/receipt2,Description2,Type2";
            var Expenses = new ExpensesEntity
            {
                UserID = userId,
                Amount =1000,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "edsaflks",
                Description = "afddjksaf",
                ExpenseType = "adsas",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "system",
                ModifiedBy = "system"
            };

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            _unitOfWork.Setup(s => s.Category.GetCategoryByNameAsync("Food"))
                .ReturnsAsync(new CategoriesEntity { CategoryID = 1, Name = "Food" });
            _unitOfWork.Setup(s => s.Category.GetCategoryByNameAsync("Transport"))
                .ReturnsAsync(new CategoriesEntity { CategoryID = 2, Name = "Transport" });
            _configuration.Setup(c => c["GenericMessages:Values:BulkExpenseSuccess"]).Returns("Bulk import successful.");
            var result = await _expensesController.BulkImport(userId, fileMock.Object);
            Assert.False(result.status);

        }
        [Test]
        public async Task BulkImport_EmptyFile_ReturnsNoFileUploadMessage()
        {
            var userId = 1;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            _configuration.Setup(c => c["GenericMessages:Values:NoFileUpload"]).Returns("No file uploaded.");
            var result = await _expensesController.BulkImport(userId, fileMock.Object);
            Assert.False(result.status);
            Assert.AreEqual("No file uploaded.", result.data);
        }

        [Test]
        public async Task BulkImport_InvalidHeaders_ReturnsInvalidHeadersMessage()
        {
            var userId = 1;
            var fileMock = new Mock<IFormFile>();
            var fileContent = "InvalidHeader,Amount,Date\n" +
                              "Food,10.00,2023-01-01";

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            _configuration.Setup(c => c["GenericMessages:Values:InvalidHeaders"]).Returns("Invalid headers.");
            var result = await _expensesController.BulkImport(userId, fileMock.Object);

            Assert.False(result.status);
            Assert.AreEqual("Invalid headers.", result.data);
        }

        [Test]
        public async Task BulkImport_CategoryNotFound_ReturnsCategoryNotFoundMessage()
        {
            var userId = 1;
            var fileMock = new Mock<IFormFile>();
            var fileContent = "CategoryName,Amount,Date\n" +
                              "NonExistentCategory,10.00,2023-01-01";

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            _unitOfWork.Setup(s => s.Category.GetCategoryByNameAsync("NonExistentCategory"))
        .ReturnsAsync((CategoriesEntity)null);
            _configuration.Setup(c => c["GenericMessages:Values:BulkExpenseFail"]).Returns("Bulk expense failed.");

            var result = await _expensesController.BulkImport(userId, fileMock.Object);

            Assert.False(result.status);
            Assert.AreEqual("Category 'NonExistentCategory' not found.", result.data);
        }
    }
}
