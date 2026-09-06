using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Filters;
using SubscriptionManager.Application.UseCases.Contracts;
using SubscriptionManager.Application.UseCases.Customers;
using SubscriptionManager.Application.UseCases.Invoices;
using SubscriptionManager.Application.UseCases.Plans;
using SubscriptionManager.Domain.Repositories;
using SubscriptionManager.Infrastructure.Data;
using SubscriptionManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<ExceptionFilter>());

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

builder.Services.AddScoped<GetCustomerByIdHandler>();
builder.Services.AddScoped<GetPlanByIdHandler>();
builder.Services.AddScoped<GetContractByIdHandler>();
builder.Services.AddScoped<GetInvoicesByContractHandler>();
builder.Services.AddScoped<ListInvoicesHandler>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SubscriptionManager.Api", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
