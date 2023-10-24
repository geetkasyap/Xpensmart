using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository  Users { get; }
        IExpensesRepository Expenses { get; }
        IBudgetRepository Budget { get; }
        ISharedAccountRepository sharedAccount { get; }
        IReportRepository Report { get; }
        ICategoryRepository Category { get; }
        INotificationRepository Notification { get; }
        ICategoryKeywordsRepository CategoryKeywords { get; }
        IBudgetSuggestionsRepository BudgetSuggestions { get; }
        IGroupsRepository Groups { get; }
        IAggregateBudgetRepository AggregateBudget { get; }
        void Commit();
    }
}
