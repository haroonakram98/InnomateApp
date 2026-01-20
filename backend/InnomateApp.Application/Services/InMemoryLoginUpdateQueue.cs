using InnomateApp.Application.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class InMemoryLoginUpdateQueue : ILoginUpdateQueue
    {
        private readonly ConcurrentQueue<int> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public Task EnqueueAsync(int userId)
        {
            if (userId < 0)
                return Task.CompletedTask;

            _queue.Enqueue(userId);
            _signal.Release();
            return Task.CompletedTask;
        }

        public bool TryDequeue(out int userId)
        {
            if (_signal.Wait(0))
            {
                return _queue.TryDequeue(out userId);
            }

            userId = 0;
            return false;
        }
    }
}
