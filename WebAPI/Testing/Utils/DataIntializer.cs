using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Testing.Utils
{
    public class DataIntializer
    {
        public static IQueryable<UsersEntity> GetLoginData()
        {
            IQueryable<UsersEntity> usersEntities = new List<UsersEntity>
            {
                new UsersEntity
                {
                    UserID=1,
                    UserName="Geet",
                    Email="geet123@example.invalid",
                    Password="Test@1234",
                    FullName="Geet Kasyap",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=2,
                    UserName="Kasyap",
                    Email="kasyap123@example.invalid",
                    Password="Sample@1234",
                    FullName="Kasyap",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=3,
                    UserName="Vallu",
                    Email="vallu123@example.invalid",
                    Password="Vallu@123",
                    FullName="Vallu",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=4,
                    UserName="Tester",
                    Email="tester123@example.invalid",
                    Password="tester@123",
                    FullName="Tester",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=5,
                    UserName="EncryptedUser",
                    Email="encryptedUser123@example.invalid",
                    Password="7Mni12yAJKRZvaGrn8q/A4a+5CesYJ25embz2VcTwJk=",
                    FullName="EncryptedUser",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=6,
                    UserName="RecurringHead",
                    Email="rc123@example.invalid",
                    Password="rc@123",
                    FullName="RC",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },
                new UsersEntity
                {
                    UserID=7,
                    UserName="RRR",
                    Email="rrr123@example.invalid",
                    Password="rrr@123",
                    FullName="RRR",
                    GroupID=0,
                    IsActive=true,
                    BudgetsEntities=new List<BudgetsEntity>(),
                    ExpenseProjectionEntities=new List<ExpenseProjectionEntity>(),
                    ExpensesEntities=new List<ExpensesEntity>(),
                    NotificationsEntities=new List<NotificationsEntity>(),
                    ReportsEntities=new List<CategoryReport>(),
                    SentRequest=new List<SharedAccountsEntity>(),
                    ReceivedRequest=new List<SharedAccountsEntity>(),
                    GroupMembersEntities=new List<GroupMembersEntity>(),
                },


            }.AsQueryable();
            return usersEntities;
        }

        public static IQueryable<ExpensesEntity> GetExpenseData()
        {
            IQueryable<ExpensesEntity> expensesEntities = new List<ExpensesEntity>
            {
                new ExpensesEntity
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
                },
                new ExpensesEntity
                {
                    ExpenseID = 2,
                    UserID = 1,
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "UpComing",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new ExpensesEntity
                {
                    ExpenseID = 3,
                    UserID = 1,
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Monthly Recurring",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "geet123@example.invalid",
                    ModifiedBy = "geet123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 4,
                    UserID = 1,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "cakes",
                    ExpenseType = "carving",
                    IsActive = true,
                    IsRecurring = false,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new ExpensesEntity
                {
                    ExpenseID = 5,
                    UserID = 1,
                    Amount = 1000,
                    Date =DateTime.Today,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Recurring",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Today,
                    ModifiedOn = DateTime.Today,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new ExpensesEntity
                {
                    ExpenseID = 6,
                    UserID = 2,
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "UpComing",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new ExpensesEntity
                {
                    ExpenseID = 7,
                    UserID = 2,
                    Amount = 100,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "chill",
                    IsActive = true,
                    IsRecurring = false,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new ExpensesEntity
                {
                    ExpenseID = 8,
                    UserID = 4,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "chill",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "kasyap123@example.invalid",
                    ModifiedBy = "kasyap123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 9,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 10,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 2,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 11,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 3,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 12,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 3,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID = 13,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 3,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "Need To Be Paid",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID =14,
                    UserID = 5,
                    Amount = 1000,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Test",
                    ExpenseType = "UpComing",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "encryptedUser123@example.invalid",
                    ModifiedBy = "encryptedUser123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID =15,
                    UserID = 6,
                    Amount = 1000,
                    Date =DateTime.Today,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Recurring",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Today,
                    ModifiedOn = DateTime.Today,
                    CreatedBy = "rc123@example.invalid",
                    ModifiedBy = "rc123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID =16,
                    UserID = 7,
                    Amount = 1000,
                    Date =DateTime.Today,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Credited",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Today,
                    ModifiedOn = DateTime.Today,
                    CreatedBy = "rc123@example.invalid",
                    ModifiedBy = "rc123@example.invalid"
                },
                new ExpensesEntity
                {
                    ExpenseID =17,
                    UserID = 7,
                    Amount = 1000,
                    Date =DateTime.Today,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "Credited",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Today,
                    ModifiedOn = DateTime.Today,
                    CreatedBy = "rc123@example.invalid",
                    ModifiedBy = "rc123@example.invalid"
                },
                 new ExpensesEntity
                {
                    ExpenseID =18,
                    UserID = 7,
                    Amount = 1000,
                    Date =DateTime.Today,
                    CategoryID = 1,
                    ReceiptUrl = "url",
                    Description = "Auto Recurring Expenses",
                    ExpenseType = "One-Type",
                    IsActive = true,
                    IsRecurring = true,
                    CreatedOn = DateTime.Today,
                    ModifiedOn = DateTime.Today,
                    CreatedBy = "rc123@example.invalid",
                    ModifiedBy = "rc123@example.invalid"
                },



            }.AsQueryable();
            return expensesEntities;
        }

        public static IQueryable<BudgetsEntity> GetBudgetData()
        {
            IQueryable<BudgetsEntity> budgetsEntities = new List<BudgetsEntity>
            {
                new BudgetsEntity
                {
                    BudgetID = 1,
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
                },
                new BudgetsEntity
                {
                    BudgetID = 2,
                    UserID = 2,
                    Amount = 2000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly",
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new BudgetsEntity
                {
                    BudgetID = 3,
                    UserID = 3,
                    Amount = 2000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly",
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new BudgetsEntity
                {
                    BudgetID = 4,
                    UserID = 4,
                    Amount = 2500,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly",
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new BudgetsEntity
                {
                    BudgetID = 5,
                    UserID = 1,
                    Amount = 2500,
                    StartDate = DateTime.Now.AddMonths(-1),
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly",
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                },
                new BudgetsEntity
                {
                    BudgetID = 6,
                    UserID = 1,
                    Amount = 2500,
                    StartDate = DateTime.Now.AddMonths(-1),
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly",
                    IsActive = false,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "test",
                    ModifiedBy = "test"
                }

            }.AsQueryable();
            return budgetsEntities;
        }

        public static IQueryable<AggregateBudgetEntity> GetAggregateBudgetData()
        {
            IQueryable<AggregateBudgetEntity> aggregateBudgetEntities = new List<AggregateBudgetEntity>
            {
                new AggregateBudgetEntity
                {
                    CategoryId=1,
                    MinimumPercentage = 0.25m,
                    MaximumPercentage = 0.45m,
                    DesiredPercentage = 0.35m,
                },
            }.AsQueryable();
            return aggregateBudgetEntities;
        }

        public static IQueryable<CategoriesEntity> GetCategoriesData()
        {
            IQueryable<CategoriesEntity> categoriesEntities = new List<CategoriesEntity>
            {
                new CategoriesEntity
                {
                    CategoryID=1,
                    Name="Housing",
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                },
            }.AsQueryable();
            return categoriesEntities;
        }

        public static IQueryable<CategoryKeywordsEntity> GetCategoryKeywordsData()
        {
            IQueryable<CategoryKeywordsEntity> categoryKeywordsEntities = new List<CategoryKeywordsEntity>
            {
                new CategoryKeywordsEntity
                {
                    CategoryKeywordID=1,
                    CategoryID=1,
                    Keyword="Housing",
                },
                new CategoryKeywordsEntity
                {
                    CategoryKeywordID=2,
                    CategoryID=2,
                    Keyword="Finance",
                },
                new CategoryKeywordsEntity
                {
                    CategoryKeywordID=3,
                    CategoryID=3,
                    Keyword="Emergency",
                },
            }.AsQueryable();
            return categoryKeywordsEntities;
        }

        public static IQueryable<CurrencyExchangeEntity> GetCurrencyExchangeData()
        {
            IQueryable<CurrencyExchangeEntity> currencyExchangeEntities = new List<CurrencyExchangeEntity>
            {
                new CurrencyExchangeEntity
                {
                    ExchangeID=1,
                    FromCurrency="USD",
                    ToCurrency="INR",
                    Rate=86.60m,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn= DateTime.Now,
                },
            }.AsQueryable();
            return currencyExchangeEntities;
        }

        public static IQueryable<ExpenseProjectionEntity> GetExpenseProjectionData()
        {
            IQueryable<ExpenseProjectionEntity> expenseProjectionEntities = new List<ExpenseProjectionEntity>
            {
                new ExpenseProjectionEntity
                {
                    ProjectionID=1,
                    UserID=1,
                    ProjectedIncome=10000.00m,
                    ProjectedExpense=8000.00m,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="geet123@example.invalid",
                    ModifiedBy="geet123@example.invalid",
                },
            }.AsQueryable();
            return expenseProjectionEntities;
        }

        public static IQueryable<GroupExpensesEntity> GetGroupExpensesData()
        {
            IQueryable<GroupExpensesEntity> groupExpensesEntities = new List<GroupExpensesEntity>
            {
                new GroupExpensesEntity
                {
                     GroupExpenseID=1,
                     GroupID=1,
                    ExpenseID=1,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="geet123@example.invalid",
                    ModifiedBy="geet123@example.invalid",
                },
            }.AsQueryable();
            return groupExpensesEntities;
        }

        public static IQueryable<GroupMembersEntity> GetGroupMembersData()
        {
            IQueryable<GroupMembersEntity> groupMembersEntities = new List<GroupMembersEntity>
            {
                new GroupMembersEntity
                {
                    GroupMemberID=1,
                    GroupID=1,
                    UserID=1,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="geet123@example.invalid",
                    ModifiedBy="geet123@example.invalid",
                },
                 new GroupMembersEntity
                {
                    GroupMemberID=5,
                    GroupID=5,
                    UserID=5,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="encryptedUser123@example.invalid",
                    ModifiedBy="encryptedUser123@example.invalid"
                },
            }.AsQueryable();
            return groupMembersEntities;
        }

        public static IQueryable<GroupsEntity> GetGroupData()
        {
            IQueryable<GroupsEntity> groupsEntities = new List<GroupsEntity>
            {
                new GroupsEntity
                {
                    GroupID=1,
                    GroupName="Goa Trip",
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="geet123@example.invalid",
                    ModifiedBy="geet123@example.invalid",
                },
                new GroupsEntity
                {
                    GroupID=5,
                    GroupName="Fail Trip",
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="encryptedUser123@example.invalid",
                    ModifiedBy="encryptedUser123@example.invalid"
                },
            }.AsQueryable();
            return groupsEntities;
        }

        public static IQueryable<NotificationsEntity> GetNotificationsData()
        {
            IQueryable<NotificationsEntity> notificationsEntities = new List<NotificationsEntity>
            {
                new NotificationsEntity
                {
                    NotificationID=1,
                    UserID=1,
                    Type="Request",
                    Message="Hey add me as your partner",
                    IsActive=true,
                    ReadMessage=false,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now, 
                },
                new NotificationsEntity
                {
                    NotificationID=2,
                    UserID=5,
                    Type="Request",
                    Message="Hey add me as your partner",
                    IsActive=true,
                    ReadMessage=false,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                },
            }.AsQueryable();
            return notificationsEntities;
        }

        public static IQueryable<SharedAccountsEntity> GetSharedAccountsData()
        {
            IQueryable<SharedAccountsEntity> sharedAccountsEntities = new List<SharedAccountsEntity>
            {
                new SharedAccountsEntity
                {
                    SharedAccountID=1,
                    UserID1=1,
                    UserID2=2,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="geet123@example.invalid",
                    ModifiedBy="geet123@example.invalid",
                },
                new SharedAccountsEntity
                {
                    SharedAccountID=2,
                    UserID1=4,
                    UserID2=5,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="tester123@example.invalid",
                    ModifiedBy="tester123@example.invalid",
                },
                new SharedAccountsEntity
                {
                    SharedAccountID=3,
                    UserID1=4,
                    UserID2=2,
                    IsActive=true,
                    CreatedOn=DateTime.Now,
                    ModifiedOn=DateTime.Now,
                    CreatedBy="tester123@example.invalid",
                    ModifiedBy="tester123@example.invalid",
                },
            }.AsQueryable();
            return sharedAccountsEntities;
        }
    }
}