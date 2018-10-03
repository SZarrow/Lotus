using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Schedule.Tests
{
    public class LocalJobForCreate : LotusJob
    {
        protected override Task Execute()
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM-dd HH:mm:ss:ffff")} -> {this.GetType().Name} Executed.");
            });
        }
    }
}
