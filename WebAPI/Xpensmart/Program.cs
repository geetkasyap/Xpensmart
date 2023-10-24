using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:8989")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    a =>
    {
        a.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        a.CommandTimeout(1000);
    }
    ));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IExpensesRepository, ExpensesRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<ISharedAccountRepository, SharedAccountRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICategoryKeywordsRepository, CategoryKeywordsRepository>();
builder.Services.AddScoped<IBudgetSuggestionsRepository, BudgetSuggestionsRepository>();
builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
builder.Services.AddScoped<IAggregateBudgetRepository, AggregateBudgetRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile($"Config/ResponseMessages.json", false, true);
var logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.Enrich.FromLogContext()
.CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

