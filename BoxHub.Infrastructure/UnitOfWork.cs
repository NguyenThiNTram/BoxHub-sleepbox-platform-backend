using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Infrastructure.Data;
using BoxHub.Infrastructure.Repositories;
using BoxHub.Infrastructure.Repositories.Pricings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BoxHubDbContext _db;
        private IDbContextTransaction? _transaction;
        //public IBoxTypePriceLimitRepository BoxTypePriceLimits { get; }
        public UnitOfWork(BoxHubDbContext context)
        {
            _db = context;
            //BoxTypePriceLimits = new BoxTypePriceLimitRepository(context);
        }

        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            _transaction = await _db.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
        }
    }

}
