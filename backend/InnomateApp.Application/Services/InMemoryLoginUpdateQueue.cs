using InnomateApp.Application.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class InMemoryLoginUpdateQueue : ILoginUpdateQueue
    {
        private readonly ConcurrentQueue<int> _queue = new();

        public Task EnqueueAsync(int userId)
        {
            if (userId > 0)
                _queue.Enqueue(userId);

            return Task.CompletedTask;
        }

        public bool TryDequeue(out int userId) => _queue.TryDequeue(out userId);
    }
}
