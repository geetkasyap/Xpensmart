using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        public IUsersRepository Users { get; set; }
        public IExpensesRepository Expenses { get; set; }
        public IBudgetRepository Budget { get; set; }
        public ISharedAccountRepository sharedAccount { get; set; }
        public IReportRepository Report { get; set; }
        public ICategoryRepository Category { get; set; }
        public INotificationRepository Notification { get; set; }
        public ICategoryKeywordsRepository CategoryKeywords { get; set; }
        public IBudgetSuggestionsRepository BudgetSuggestions { get; set; }
        public IGroupsRepository Groups { get; set; }
        public IAggregateBudgetRepository AggregateBudget { get; set; }

        private readonly ApplicationDbContext _dbcontext;
        public UnitOfWork(ApplicationDbContext dBContext, IUsersRepository usersRepository, IExpensesRepository expensesRepository, IBudgetRepository budgetRepository, 
             ISharedAccountRepository SharedAccount, IReportRepository report, ICategoryRepository category,
            INotificationRepository notification, ICategoryKeywordsRepository categoryKeywords, IBudgetSuggestionsRepository budgetSuggestions, IGroupsRepository groups
            ,IAggregateBudgetRepository aggregateBudget)
        {
            _dbcontext = dBContext;
            Users = usersRepository;
            Expenses = expensesRepository;
            Budget = budgetRepository;
            sharedAccount = SharedAccount;
            Report = report;
            Category = category;
            Notification = notification;
            CategoryKeywords = categoryKeywords;
            BudgetSuggestions = budgetSuggestions;
            AggregateBudget = aggregateBudget;
            Groups = groups;    
        }
        public void Commit()
        {
            _dbcontext.SaveChanges();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
