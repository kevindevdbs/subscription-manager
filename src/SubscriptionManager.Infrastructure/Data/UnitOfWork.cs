using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Domain.Exceptions;
using SubscriptionManager.Domain.Repositories;

namespace SubscriptionManager.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private const int DuplicateKeyInIndex = 2601;
    private const int UniqueConstraintViolation = 2627;

    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            throw new ConflictException("Já existe um registro com esses dados.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
            && sqlException.Number is DuplicateKeyInIndex or UniqueConstraintViolation;
    }
}
