using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoreModels.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext() { }

        public virtual DbSet<UsersEntity> UsersEntities { get; set; }
        public virtual DbSet<CategoriesEntity> CategoriesEntites { get; set; }
        public virtual DbSet<ExpensesEntity> ExpensesEntities { get; set; }
        public virtual DbSet<BudgetsEntity> BudgetsEntities { get; set; }
        public virtual DbSet<NotificationsEntity> NotificationsEntities { get; set; }
        public virtual DbSet<CategoryReport> ReportsEntities { get; set; }
        public virtual DbSet<SharedAccountsEntity> SharedAccountsEntities { get; set; }
        public virtual DbSet<CurrencyExchangeEntity> CurrencyExchangeEntities { get; set; }
        public virtual DbSet<ExpenseProjectionEntity> ExpenseProjectionEntities { get; set; }
        public virtual DbSet<CategoryKeywordsEntity> CategoryKeywordsEntity { get; set; }
        public virtual DbSet<GroupMembersEntity> GroupMembersEntities { get; set; }
        public virtual DbSet<GroupExpensesEntity> GroupExpensesEntities { get; set; }
        public virtual DbSet<AggregateBudgetEntity> AggregateBudgetEntities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SharedAccountsEntity>()
                .HasOne(sa => sa.SentRequest)
                .WithMany(sa => sa.SentRequest)
                .HasForeignKey(sa => sa.UserID1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SharedAccountsEntity>()
                .HasOne(sa => sa.ReceivedRequest)
                .WithMany(sa => sa.ReceivedRequest)
                .HasForeignKey(sa => sa.UserID2)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
