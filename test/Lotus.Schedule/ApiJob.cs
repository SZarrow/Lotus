using System;
using System.Threading.Tasks;
using Quartz;

namespace Lotus.Schedule
{
    public class ApiJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
