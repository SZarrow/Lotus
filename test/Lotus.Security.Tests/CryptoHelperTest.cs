using System;
using Xunit;

namespace Lotus.Security.Tests
{
    public class CryptoHelperTest
    {
        [Fact]
        public void TestMakeRSAKeyPairs()
        {
            var result = CryptoHelper.MakeRSAKeyPairs();
            Assert.True(result.Success);
        }
    }
}
