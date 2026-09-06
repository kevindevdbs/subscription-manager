using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Application.UseCases.Customers;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Application.UseCases.Plans;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Infrastructure.Data;
using SubscriptionManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();

builder.Services.AddScoped<CreateContractHandler>();
builder.Services.AddScoped<CreatePlanHandler>();
builder.Services.AddScoped<CreateCustomerHandler>();
builder.Services.AddScoped<GenerateMonthlyInvoicesHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
