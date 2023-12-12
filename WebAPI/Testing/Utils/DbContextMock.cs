using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Utils
{
    public class DbContextMock
    {
        public Mock<ApplicationDbContext> mockContext;
        public UsersRepository _usersRepository;
        public ExpensesRepository _expensesRepository;
        public BudgetRepository _budgetRepository;
        public AggregateBudgetRepository _aggregateBudgetRepository;
        public CategoryRepository _categoryRepository;
        public BudgetSuggestionsRepository _budgetSuggestionsRepository;
        public CategoryKeywordsRepository _categoryKeywordsRepository;
        public GroupsRepository _groupsRepository;
        public NotificationRepository _notificationRepository;
        public ReportRepository _reportRepository;
        public SharedAccountRepository _sharedAccountRepository;



        public IQueryable<UsersEntity> _usersEntities;
        public IQueryable<ExpensesEntity> _expensesEntities;
        public IQueryable<BudgetsEntity> _budgetsEntities;

        public IQueryable<AggregateBudgetEntity> _aggregateBudgetEntities;
        public IQueryable<CategoriesEntity> _categoriesEntities;
        public IQueryable<CategoryKeywordsEntity> _categoryKeywordsEntities;
        public IQueryable<CurrencyExchangeEntity> _currencyExchangeEntities;
        public IQueryable<ExpenseProjectionEntity> _expenseProjectionEntities;
        public IQueryable<GroupExpensesEntity> _groupExpensesEntities;
        public IQueryable<GroupMembersEntity> _groupMembersEntities;
        public IQueryable<GroupsEntity> _groupsEntities;
        public IQueryable<NotificationsEntity> _notificationsEntities;
        public IQueryable<SharedAccountsEntity> _sharedAccountsEntities;

        [OneTimeSetUp]
        public void Setup()
        {
            _usersEntities = DataIntializer.GetLoginData();
            _expensesEntities = DataIntializer.GetExpenseData();
            _budgetsEntities = DataIntializer.GetBudgetData();

            _aggregateBudgetEntities = DataIntializer.GetAggregateBudgetData();
            _categoriesEntities = DataIntializer.GetCategoriesData();
            _categoryKeywordsEntities = DataIntializer.GetCategoryKeywordsData();
            _currencyExchangeEntities = DataIntializer.GetCurrencyExchangeData();
            _expenseProjectionEntities = DataIntializer.GetExpenseProjectionData();
            _groupExpensesEntities = DataIntializer.GetGroupExpensesData();
            _groupMembersEntities = DataIntializer.GetGroupMembersData();
            _groupsEntities = DataIntializer.GetGroupData();
            _notificationsEntities = DataIntializer.GetNotificationsData();
            _sharedAccountsEntities = DataIntializer.GetSharedAccountsData();
        }

        [SetUp]
        public void ReinitializeTest()
        {
            var mockUsersData = new Mock<DbSet<UsersEntity>>();
            mockUsersData.As<IQueryable<UsersEntity>>().Setup(a => a.Provider).Returns(_usersEntities.Provider);
            mockUsersData.As<IQueryable<UsersEntity>>().Setup(a => a.Expression).Returns(_usersEntities.Expression);
            mockUsersData.As<IQueryable<UsersEntity>>().Setup(a => a.ElementType).Returns(_usersEntities.ElementType);
            mockUsersData.As<IQueryable<UsersEntity>>().Setup(a => a.GetEnumerator()).Returns(_usersEntities.GetEnumerator());

            var mockExpensesData = new Mock<DbSet<ExpensesEntity>>();
            mockExpensesData.As<IQueryable<ExpensesEntity>>().Setup(a => a.Provider).Returns(_expensesEntities.Provider);
            mockExpensesData.As<IQueryable<ExpensesEntity>>().Setup(a => a.Expression).Returns(_expensesEntities.Expression);
            mockExpensesData.As<IQueryable<ExpensesEntity>>().Setup(a => a.ElementType).Returns(_expensesEntities.ElementType);
            mockExpensesData.As<IQueryable<ExpensesEntity>>().Setup(a => a.GetEnumerator()).Returns(_expensesEntities.GetEnumerator());

            var mockBudgetsData = new Mock<DbSet<BudgetsEntity>>();
            mockBudgetsData.As<IQueryable<BudgetsEntity>>().Setup(a => a.Provider).Returns(_budgetsEntities.Provider);
            mockBudgetsData.As<IQueryable<BudgetsEntity>>().Setup(a => a.Expression).Returns(_budgetsEntities.Expression);
            mockBudgetsData.As<IQueryable<BudgetsEntity>>().Setup(a => a.ElementType).Returns(_budgetsEntities.ElementType);
            mockBudgetsData.As<IQueryable<BudgetsEntity>>().Setup(a => a.GetEnumerator()).Returns(_budgetsEntities.GetEnumerator());

            var mockGroupExpensesData = new Mock<DbSet<GroupExpensesEntity>>();
            mockGroupExpensesData.As<IQueryable<GroupExpensesEntity>>().Setup(a => a.Provider).Returns(_groupExpensesEntities.Provider);
            mockGroupExpensesData.As<IQueryable<GroupExpensesEntity>>().Setup(a => a.Expression).Returns(_groupExpensesEntities.Expression);
            mockGroupExpensesData.As<IQueryable<GroupExpensesEntity>>().Setup(a => a.ElementType).Returns(_groupExpensesEntities.ElementType);
            mockGroupExpensesData.As<IQueryable<GroupExpensesEntity>>().Setup(a => a.GetEnumerator()).Returns(_groupExpensesEntities.GetEnumerator());

            var mockGroupMembersData = new Mock<DbSet<GroupMembersEntity>>();
            mockGroupMembersData.As<IQueryable<GroupMembersEntity>>().Setup(a => a.Provider).Returns(_groupMembersEntities.Provider);
            mockGroupMembersData.As<IQueryable<GroupMembersEntity>>().Setup(a => a.Expression).Returns(_groupMembersEntities.Expression);
            mockGroupMembersData.As<IQueryable<GroupMembersEntity>>().Setup(a => a.ElementType).Returns(_groupMembersEntities.ElementType);
            mockGroupMembersData.As<IQueryable<GroupMembersEntity>>().Setup(a => a.GetEnumerator()).Returns(_groupMembersEntities.GetEnumerator());

            var mockGroupData = new Mock<DbSet<GroupsEntity>>();
            mockGroupData.As<IQueryable<GroupsEntity>>().Setup(a => a.Provider).Returns(_groupsEntities.Provider);
            mockGroupData.As<IQueryable<GroupsEntity>>().Setup(a => a.Expression).Returns(_groupsEntities.Expression);
            mockGroupData.As<IQueryable<GroupsEntity>>().Setup(a => a.ElementType).Returns(_groupsEntities.ElementType);
            mockGroupData.As<IQueryable<GroupsEntity>>().Setup(a => a.GetEnumerator()).Returns(_groupsEntities.GetEnumerator());

            var mockNotificationsData = new Mock<DbSet<NotificationsEntity>>();
            mockNotificationsData.As<IQueryable<NotificationsEntity>>().Setup(a => a.Provider).Returns(_notificationsEntities.Provider);
            mockNotificationsData.As<IQueryable<NotificationsEntity>>().Setup(a => a.Expression).Returns(_notificationsEntities.Expression);
            mockNotificationsData.As<IQueryable<NotificationsEntity>>().Setup(a => a.ElementType).Returns(_notificationsEntities.ElementType);
            mockNotificationsData.As<IQueryable<NotificationsEntity>>().Setup(a => a.GetEnumerator()).Returns(_notificationsEntities.GetEnumerator());

            var mockSharedAccountsData = new Mock<DbSet<SharedAccountsEntity>>();
            mockSharedAccountsData.As<IQueryable<SharedAccountsEntity>>().Setup(a => a.Provider).Returns(_sharedAccountsEntities.Provider);
            mockSharedAccountsData.As<IQueryable<SharedAccountsEntity>>().Setup(a => a.Expression).Returns(_sharedAccountsEntities.Expression);
            mockSharedAccountsData.As<IQueryable<SharedAccountsEntity>>().Setup(a => a.ElementType).Returns(_sharedAccountsEntities.ElementType);
            mockSharedAccountsData.As<IQueryable<SharedAccountsEntity>>().Setup(a => a.GetEnumerator()).Returns(_sharedAccountsEntities.GetEnumerator());

            var mockAggregateBudgetData = new Mock<DbSet<AggregateBudgetEntity>>();
            mockAggregateBudgetData.As<IQueryable<AggregateBudgetEntity>>().Setup(a => a.Provider).Returns(_aggregateBudgetEntities.Provider);
            mockAggregateBudgetData.As<IQueryable<AggregateBudgetEntity>>().Setup(a => a.Expression).Returns(_aggregateBudgetEntities.Expression);
            mockAggregateBudgetData.As<IQueryable<AggregateBudgetEntity>>().Setup(a => a.ElementType).Returns(_aggregateBudgetEntities.ElementType);
            mockAggregateBudgetData.As<IQueryable<AggregateBudgetEntity>>().Setup(a => a.GetEnumerator()).Returns(_aggregateBudgetEntities.GetEnumerator());

            var mockExpenseProjectionData = new Mock<DbSet<ExpenseProjectionEntity>>();
            mockExpenseProjectionData.As<IQueryable<ExpenseProjectionEntity>>().Setup(a => a.Provider).Returns(_expenseProjectionEntities.Provider);
            mockExpenseProjectionData.As<IQueryable<ExpenseProjectionEntity>>().Setup(a => a.Expression).Returns(_expenseProjectionEntities.Expression);
            mockExpenseProjectionData.As<IQueryable<ExpenseProjectionEntity>>().Setup(a => a.ElementType).Returns(_expenseProjectionEntities.ElementType);
            mockExpenseProjectionData.As<IQueryable<ExpenseProjectionEntity>>().Setup(a => a.GetEnumerator()).Returns(_expenseProjectionEntities.GetEnumerator());

            var mockCurrencyExchangeData = new Mock<DbSet<CurrencyExchangeEntity>>();
            mockCurrencyExchangeData.As<IQueryable<CurrencyExchangeEntity>>().Setup(a => a.Provider).Returns(_currencyExchangeEntities.Provider);
            mockCurrencyExchangeData.As<IQueryable<CurrencyExchangeEntity>>().Setup(a => a.Expression).Returns(_currencyExchangeEntities.Expression);
            mockCurrencyExchangeData.As<IQueryable<CurrencyExchangeEntity>>().Setup(a => a.ElementType).Returns(_currencyExchangeEntities.ElementType);
            mockCurrencyExchangeData.As<IQueryable<CurrencyExchangeEntity>>().Setup(a => a.GetEnumerator()).Returns(_currencyExchangeEntities.GetEnumerator());

            var mockCategoryKeywordsData = new Mock<DbSet<CategoryKeywordsEntity>>();
            mockCategoryKeywordsData.As<IQueryable<CategoryKeywordsEntity>>().Setup(a => a.Provider).Returns(_categoryKeywordsEntities.Provider);
            mockCategoryKeywordsData.As<IQueryable<CategoryKeywordsEntity>>().Setup(a => a.Expression).Returns(_categoryKeywordsEntities.Expression);
            mockCategoryKeywordsData.As<IQueryable<CategoryKeywordsEntity>>().Setup(a => a.ElementType).Returns(_categoryKeywordsEntities.ElementType);
            mockCategoryKeywordsData.As<IQueryable<CategoryKeywordsEntity>>().Setup(a => a.GetEnumerator()).Returns(_categoryKeywordsEntities.GetEnumerator());

            var mockCategoriesData = new Mock<DbSet<CategoriesEntity>>();
            mockCategoriesData.As<IQueryable<CategoriesEntity>>().Setup(a => a.Provider).Returns(_categoriesEntities.Provider);
            mockCategoriesData.As<IQueryable<CategoriesEntity>>().Setup(a => a.Expression).Returns(_categoriesEntities.Expression);
            mockCategoriesData.As<IQueryable<CategoriesEntity>>().Setup(a => a.ElementType).Returns(_categoriesEntities.ElementType);
            mockCategoriesData.As<IQueryable<CategoriesEntity>>().Setup(a => a.GetEnumerator()).Returns(_categoriesEntities.GetEnumerator());


            mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(a => a.Set<UsersEntity>()).Returns(mockUsersData.Object);
            mockContext.Setup(a => a.Set<ExpensesEntity>()).Returns(mockExpensesData.Object);
            mockContext.Setup(a => a.Set<BudgetsEntity>()).Returns(mockBudgetsData.Object);
            mockContext.Setup(a => a.Set<AggregateBudgetEntity>()).Returns(mockAggregateBudgetData.Object);
            mockContext.Setup(a => a.Set<CategoriesEntity>()).Returns(mockCategoriesData.Object);
            mockContext.Setup(a => a.Set<CategoryKeywordsEntity>()).Returns(mockCategoryKeywordsData.Object);
            mockContext.Setup(a => a.Set<CurrencyExchangeEntity>()).Returns(mockCurrencyExchangeData.Object);
            mockContext.Setup(a => a.Set<ExpenseProjectionEntity>()).Returns(mockExpenseProjectionData.Object);
            mockContext.Setup(a => a.Set<GroupExpensesEntity>()).Returns(mockGroupExpensesData.Object);
            mockContext.Setup(a => a.Set<GroupMembersEntity>()).Returns(mockGroupMembersData.Object);
            mockContext.Setup(a => a.Set<GroupsEntity>()).Returns(mockGroupData.Object);
            mockContext.Setup(a => a.Set<NotificationsEntity>()).Returns(mockNotificationsData.Object);
            mockContext.Setup(a => a.Set<SharedAccountsEntity>()).Returns(mockSharedAccountsData.Object);



            _usersRepository = new UsersRepository(mockContext.Object);
            _expensesRepository = new ExpensesRepository(mockContext.Object);
            _budgetRepository = new BudgetRepository(mockContext.Object);
            _aggregateBudgetRepository = new AggregateBudgetRepository(mockContext.Object);
            _categoryRepository = new CategoryRepository(mockContext.Object);
            _budgetSuggestionsRepository = new BudgetSuggestionsRepository(mockContext.Object);
            _categoryKeywordsRepository = new CategoryKeywordsRepository(mockContext.Object);
            _groupsRepository = new GroupsRepository(mockContext.Object);
            _notificationRepository = new NotificationRepository(mockContext.Object);
            _reportRepository = new ReportRepository(mockContext.Object);
            _sharedAccountRepository = new SharedAccountRepository(mockContext.Object);
        }
    }
}