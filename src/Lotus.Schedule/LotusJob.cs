using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Lotus.Schedule
{
    public abstract class LotusJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Execute();
        }

        protected abstract Task Execute();
    }
}
