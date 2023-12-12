using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Testing.ExpensesTests
{
    public class ExpenseRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public async Task AddExpensesAsync_Success()
        {
            var newExpense = new ExpensesEntity
            {
                UserID = 1,
                Amount = 200,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "new_url",
                Description = "New Test Expense",
                ExpenseType = "Test"
            };

            var result = await _expensesRepository.AddExpensesAsync(newExpense);

            Assert.IsNotNull(result);
            Assert.AreEqual(newExpense.Amount, result.Amount);
            Assert.AreEqual(newExpense.Description, result.Description);
        }

        [Test]
        public async Task AddExpensesAsync_Fail()
        {
            var newExpense = new ExpensesEntity
            {
                UserID = 1,
                Amount = 200,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "new_url",
                Description = "New Test Expense",
                ExpenseType = "Test"
            };
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _expensesRepository.AddExpensesAsync(newExpense));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public async Task DeleteExpenses_Success()
        {
            var expenseId = 1; 
            var result = await _expensesRepository.DeleteExpenses(expenseId);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive); 
        }

        [Test]
        public async Task DeleteExpenses_Fail()
        {
            var expenseId = 10;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _expensesRepository.DeleteExpenses(expenseId));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void GetExpensesDetails_Success()
        {
            var result = _expensesRepository.GetExpensesDetails();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0); 
        }
        [Test]
        public void GetExpensesDetails_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GetExpensesDetails());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetExpensesDetails_ById_Success()
        {
            var expenseId = 4; 
            var result = _expensesRepository.GetExpensesDetails(expenseId);

            Assert.IsNotNull(result);
        }
        [Test]
        public void GetExpensesDetails_ById_Fail()
        {
            var expenseId = 4;
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GetExpensesDetails(expenseId));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void UpdateExpenses_Success()
        {
            var updatedExpense = new ExpensesEntity
            {
                ExpenseID = 7,
                UserID = 2,
                Amount = 150,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "updated_url",
                Description = "Updated Test Expense",
                ExpenseType = "Test"
            };

            var result = _expensesRepository.UpdateExpenses(7, 2, updatedExpense); 

            Assert.IsNotNull(result);
            Assert.AreEqual(updatedExpense.Amount, result.Amount);
            Assert.AreEqual(updatedExpense.Description, result.Description);
        }

        [Test]
        public void UpdateExpenses_Fail()
        {
            var updatedExpense = new ExpensesEntity
            {
                ExpenseID = 8,
                UserID = 2,
                Amount = 150,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "updated_url",
                Description = "Updated Test Expense",
                ExpenseType = "Test"
            };
            mockContext.Setup(m => m.SaveChanges()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _expensesRepository.UpdateExpenses(7, 2, updatedExpense));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GenerateReport_Success()
        {
            var result = _expensesRepository.GenerateReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1); 

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<decimal>(result);
        }

        [Test]
        public void GenerateReport_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GenerateReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetUpcomingExpenses_Success()
        {
            var result = _expensesRepository.GetUpcomingExpenses(2); 

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetUpcomingExpenses_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GetUpcomingExpenses(2));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetUpcomingExpenses_False()
        {
            var result = _expensesRepository.GetUpcomingExpenses(3);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ConvertCurrency_Success()
        {
            var result = _expensesRepository.ConvertCurrency(100, "INR", "USD");
            Assert.IsInstanceOf<decimal>(result);
            Assert.Greater(result, 0);
        }
        [Test]
        public void ConvertCurrency_If_INR()
        {
            var result = _expensesRepository.ConvertCurrency(100, "USD", "INR");
            Assert.IsInstanceOf<decimal>(result);
            Assert.Greater(result, 0);
        }

        [Test]
        public void ConvertCurrency_Fail()
        {
            mockContext.Setup(m => m.Set<CurrencyExchangeEntity>()).Throws(new Exception("Currency conversion rate not found."));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.ConvertCurrency(100, "INR", "USD"));
            Assert.That(ex.Message, Is.EqualTo("Currency conversion rate not found."));
        }

        [Test]
        public void ConvertCurrency_Null()
        {
            var result = _expensesRepository.ConvertCurrency(100, "ABC", "DEF");
            Assert.IsInstanceOf<decimal>(result);
            Assert.Greater(result, 0);
        }


        [Test]
        public async Task AddTransactionsAsync_Success()
        {
            var result = await _expensesRepository.AddTransactionsAsync(DateTime.Now, DateTime.Now.AddMonths(1), "Monthly", 1, 1, 100, "url");

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AddTransactionsAsync_Fail()
        {
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _expensesRepository.AddTransactionsAsync(DateTime.Now, DateTime.Now.AddMonths(1), "Monthly", 1, 1, 100, "url"));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetTransactionsDetails_Success()
        {
            var result = _expensesRepository.GetTransactionsDetails("Monthly Recurring", 1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetTransactionsDetails_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("DB error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GetTransactionsDetails("Monthly Recurring", 1));
            Assert.That(ex.Message, Is.EqualTo("DB error"));
        }

        [Test]
        public void GetTransactionsDetails_ByDate_Success()
        {
            var date = DateTime.Today; 
            var result = _expensesRepository.GetTransactionsDetails(date, "Recurring", 1);

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetTransactionsDetails_ByDate_Fail()
        {
            var date = DateTime.Today;
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("DB error"));
            var ex = Assert.Throws<Exception>(() => _expensesRepository.GetTransactionsDetails(date, "Recurring", 1));
            Assert.That(ex.Message, Is.EqualTo("DB error"));
        }

        [Test]
        public void UpdateTransactions_Success()
        {
            var updatedExpense = new ExpensesEntity
            {
                ExpenseID = 15,
                UserID = 6,
                Amount = 500,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Auto Recurring Expenses",
                ExpenseType = "Monthly Recurring",
                IsActive = true,
                IsRecurring = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "Tester123@example.invalid",
                ModifiedBy = "Tester123@example.invalid"
            };

            var result = _expensesRepository.UpdateTransactions(15, 6, updatedExpense);

            Assert.IsNotNull(result);
        }
        [Test]
        public void UpdateTransactions_NullTransactions_Success()
        {
            try
            {
                var updatedExpense = new ExpensesEntity
                {
                    ExpenseID = 15,
                    UserID = 6,
                    Amount = 500,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Monthly Recurring",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "Tester123@example.invalid",
                    ModifiedBy = "Tester123@example.invalid"
                };

                var result = _expensesRepository.UpdateTransactions(15000, 6, updatedExpense);

                Assert.IsNotNull(result);
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("transactionToUpdate was null"));
            }
           
        }

        [Test]
        public void UpdateTransactions_Null_Success()
        {
            try
            {
                var updatedExpense = new ExpensesEntity
                {
                    ExpenseID = 15,
                    UserID = 600,
                    Amount = 500,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Monthly Recurring",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "Tester123@example.invalid",
                    ModifiedBy = "Tester123@example.invalid"
                };
                var result = _expensesRepository.UpdateTransactions(15, 600, updatedExpense);
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));

            }
        }

        [Test]
        public void UpdateTransactions_Fail()
        {
            var updatedExpense = new ExpensesEntity
            {
                ExpenseID = 15,
                UserID = 6,
                Amount = 500,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Auto Recurring Expenses",
                ExpenseType = "Monthly Recurring",
                IsActive = true,
                IsRecurring = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "Tester123@example.invalid",
                ModifiedBy = "Tester123@example.invalid"
            };

            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _expensesRepository.UpdateTransactions(15, 6, updatedExpense));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void DeleteTransactions_Success()
        {
            var result = _expensesRepository.DeleteTransactions(3); 

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive); 
        }
        [Test]
        public void DeleteTransactions_TransactionNull_Success()
        {
            try
            {
                var result = _expensesRepository.DeleteTransactions(300);
                Assert.IsNotNull(result);
            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("transactionToDelete was null"));
            }
        }
        [Test]
        public void DeleteTransactions_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _expensesRepository.DeleteTransactions(3));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetAllExpensesDetails_Success()
        {
            var result = _expensesRepository.GetAllExpensesDetails(1); 

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetAllExpensesDetails_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _expensesRepository.GetAllExpensesDetails(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public async Task AddBulkExpensesAsync_Success()
        {
            var newExpense = new ExpensesEntity
            {
                ExpenseID = 100,
                UserID = 2,
                Amount = 2000,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Bulk Test Expense",
                ExpenseType = "Bulk",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "vallu123@example.invalid",
                ModifiedBy = "vallu123@example.invalid"

            };

            await _expensesRepository.AddBulkExpensesAsync(2, newExpense);

            var allExpenses = _expensesRepository.GetAllExpensesDetails(2);
            Assert.IsNotNull(allExpenses);
        }
        [Test]
        public async Task AddBulkExpensesAsync_NullUser_Success()
        {
            try
            {
                var newExpense = new ExpensesEntity
                {
                    ExpenseID = 9,
                    UserID = 3,
                    Amount = 2000,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Bulk Test Expense",
                    ExpenseType = "Bulk",
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "vallu123@example.invalid",
                    ModifiedBy = "vallu123@example.invalid"

                };

                await _expensesRepository.AddBulkExpensesAsync(3000, newExpense);

                var allExpenses = _expensesRepository.GetAllExpensesDetails(3);

            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User not found or inactive."));
            }
           }

        [Test]
        public async Task AddBulkExpensesAsync_Fail()
        {
            var newExpense = new ExpensesEntity
            {
                ExpenseID = 9,
                UserID = 3,
                Amount = 2000,
                Date = DateTime.Now,
                CategoryID = 1,
                ReceiptUrl = "url",
                Description = "Bulk Test Expense",
                ExpenseType = "Bulk",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "vallu123@example.invalid",
                ModifiedBy = "vallu123@example.invalid"

            };

            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _expensesRepository.AddBulkExpensesAsync(3, newExpense));
            Assert.That(ex.Message, Is.EqualTo("Database error"));

        }



    }
}

