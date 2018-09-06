using System;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace Lotus.Logging.Tests
{
    public class LogTest
    {
        private static ILogger _logger = LogManager.GetLogger();

        [Fact]
        public void TestLog()
        {
            String logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            //_logger.Log("Info");
            //_logger.Warn("Warn");
            _logger.Error("Error");
            _logger.Error(new ArgumentNullException("error"), "²ÎÊýÎªkong");

            Thread.Sleep(10 * 1000);

            Assert.True(File.Exists(logFile));

            String[] contents = File.ReadAllLines(logFile, Encoding.UTF8);
            Assert.True(contents != null && contents.Length > 7);
        }
    }
}
