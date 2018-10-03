using System;
using System.IO;

namespace Lotus.Schedule.Tests
{
    class Program
    {
        static void Main(String[] args)
        {
            String apiJobsConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apijobs.config");
            ServiceFactory.RunService(new ServiceDescription()
            {
                Description = "Api任务集合",
                DisplayName = "LotusApiService",
                ServiceName = "LotusApiService"
            }, apiJobsConfigFile);

            Console.ReadKey();
        }
    }
}
