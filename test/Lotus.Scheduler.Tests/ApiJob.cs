using System;
using System.Threading.Tasks;
using Quartz;
using Xunit;

namespace Lotus.Scheduler.Tests
{
    public class ApiJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
