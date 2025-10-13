using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface ILoginUpdateQueue
    {
        Task EnqueueAsync(int userId);
        bool TryDequeue(out int userId);
    }
}
