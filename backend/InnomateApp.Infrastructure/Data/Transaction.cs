using InnomateApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Data
{
    public class Transaction : ITransaction
    {
        private readonly IDbContextTransaction _efTransaction;

        public Transaction(IDbContextTransaction efTransaction)
        {
            _efTransaction = efTransaction;
        }

        public async Task CommitAsync()
        {
            await _efTransaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _efTransaction.RollbackAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _efTransaction.DisposeAsync();
        }
    }
}
