using System;
using System.Security.Cryptography;
using Xunit;

namespace Lotus.Security.Tests
{
    public class CryptoHelperTest
    {
        private const String PrivateKey = @"MIICXQIBAAKBgQDOzcMzgG1gSQEZRImuJ5qfR0xXQVjWpnkgQLCInGhyJ+O7i5f2aGhXhW+k+7DCL/08D6pcoN6WQF/+vCXOYAoEPzw1HQChhBdoQe7IMARhc+26q/epUpp4GnggjfIR8q78aDNWgzGHMnkUgaUGhtAK3VsAjclSHRFQkUElq17XmwIDAQABAoGBAIw4D6aX6ZFjbo9HXWLsD3b3zNdMw4OnFHG96vR1uIvOaCb9m2fDmxvcqbpfvZWtHDLhHE359XJC69O4lpm7nI3UnDMzMO/SyULhIv1zVVOuOE2GmgK6N7CxoJ5rwdvT1P5NmxO2WtFqXyVYlSOYji1iBdLaPASB5s3ZKB5y7QkBAkEA5xmt52yGD69T5HSi12SX0+PltiMeEbuNhSJ46blctpnrn0/4QXIONRHytoOW7OHGIffvR3nvYwqNMlcWpY3vOwJBAOUV7In3XXSVrL5p//pVZ/FQ5vbYzKzR20BQ+f/7jPGBhe1n0dbUmwrnVFq6ln2NikF2uCN/HiiY6XyD1goa8yECQGuO0heRtNt7+Imtl1S0Zs2hlfotYgNSzU0XfDsboIEEJlvhdmPPV7lvfw1fNVFOy05n/J/Bqp7n/EtfqRSoeJ0CQQC30nvXyXJoVqIiuRP6YAXkEbMDaLv0AQEZ/uBclBFoyTIaajBrXnZ6rV124DpZzPWfyg/ADAS7NthEXdWmjjGBAkAPfZoSFKAFCNrtXPaRpRZ8tNzVQ2zY6cIIRSgzB/NuLKg2C0fLrNwKpxX4h0Z1IlVcICutbrpPevepfN3jkcuf";
        private const String PublicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDOzcMzgG1gSQEZRImuJ5qfR0xXQVjWpnkgQLCInGhyJ+O7i5f2aGhXhW+k+7DCL/08D6pcoN6WQF/+vCXOYAoEPzw1HQChhBdoQe7IMARhc+26q/epUpp4GnggjfIR8q78aDNWgzGHMnkUgaUGhtAK3VsAjclSHRFQkUElq17XmwIDAQAB";

        [Fact]
        public void TestRSAEncryptAndDecryptText()
        {
            String rawText = "I love you, My Girl!";
            var encResult = CryptoHelper.RSAEncrypt(rawText, PublicKey);
            Assert.True(encResult.Success);

            var decResult = CryptoHelper.RSADecrypt(encResult.Value, PrivateKey);
            Assert.True(decResult.Success);
            String decText = decResult.Value;

            Assert.Equal(rawText, decText);
        }

        [Fact]
        public void TestRSAEncryptAndDecryptWithBigText()
        {
            String rawText = @"MIICXgIBAAKBgQC0xP5HcfThSQr43bAMoopbzcCyZWE0xfUeTA4Nx4PrXEfDvybJ
EIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQiCLjeZ3HtlRKld+9htAZtHFZ
osV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHhqoOmjXaCv58CSRAlAQIDAQAB
AoGBAJtDgCwZYv2FYVk0ABw6F6CWbuZLUVykks69AG0xasti7Xjh3AximUnZLefs
iuJqg2KpRzfv1CM+Cw5cp2GmIVvRqq0GlRZGxJ38AqH9oyUa2m3TojxWapY47zye
PYEjWwRTGlxUBkdujdcYj6/dojNkm4azsDXl9W5YaXiPfbgJAkEA4rlhSPXlohDk
FoyfX0v2OIdaTOcVpinv1jjbSzZ8KZACggjiNUVrSMIICXgIBAAKBgQC0xP5HcfThSQr43bAMoopbzcCyZWE0xfUeTA4Nx4PrXEfDvybJ
EIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQiCLjeZ3HtlRKld+9htAZtHFZ
osV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHhqoOmjXaCv58CSRAlAQIDAQAB
AoGBAJtDgCwZYv2FYVk0ABw6F6CWbuZLUVykks69AG0xasti7Xjh3AximUnZLefs
iuJqg2KpRzfv1CM+Cw5cp2GmIVvRMIICXgIBAAKBgQC0xP5HcfThSQr43bAMoopbzcCyZWE0xfUeTA4Nx4PrXEfDvybJ
EIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQiCLjeZ3HtlRKld+9htAZtHFZ
osV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHhqoOmjXaCv58CSRAlAQIDAQAB
AoGBAJtDgCwZYv2FYVk0ABw6F6CWbuZLUVykks69AG0xasti7Xjh3AximUnZLefs
iuJqg2KpRzfv1CM+Cw5cp2GmIVvRqq0GlRZGxJ38AqH9oyUa2m3TojxWapY47zye
PYEjWwRTGlxUBkdujdcYj6/dojNkm4azsDXl9W5YaXiPfbgJAkEA4rlhSPXlohDk
FoyfX0v2OIdaTOcVpinv1jjbSzZ8KZACggjiNUVrSFV3Y4oWom93K5JLXf2mV0Sy
80mPR5jOdwJBAMwciAk8xyQKpMUGNhFX2jKboAYY1SJCfuUnyXHAPWeHp5xCL2UH
tjryJp/Vx8TgsFTGyWSyIE9R8hSup+32rkcCQBe+EAkC7yQ0np4Z5cql+sfarMMm
4+Z9t8b4N0a+EuyLTyfs5Dtt5JkzkggTeuFRyOoALPJP0K6M3CyMBHwb7WsCQQCi
TM2fCsUO06fRQu8bO1A1janhLz3K0DU24jw8RzCMckHE7pvhKhCtLn+n+MWwtzl/
L9JUT4+BgxeLepXtkolhAkEA2V7er7fnEuL0+kKIjmOm5F3kvMIDh9YC1JwLGSvu
1fnzxK34QwSdxgQRF1dfIKJw73lClQpHZfQxL/2XRG8IoA==qq0GlRZGxJ38AqH9oyUa2m3TojxWapY47zye
PYEjWwRTGlxUBkdujdcYj6/dojNkm4azsDXl9W5YaXiPfbgJAkEA4rlhSPXlohDk
FoyfX0v2OIdaTOcVpinv1jjbSzZ8KZACggjiNUVrSFV3Y4oWom93K5JLXf2mV0Sy
80mPR5jOdwJBAMwciAk8xyQKpMUGNhFX2jKboAYY1SJCfuUnyXHAPWeHp5xCL2UH
tjryJp/Vx8TgsFTGyWSyIE9R8hSupMIICXgIBAAKBgQC0xP5HcfThSQr43bAMoopbzcCyZWE0xfUeTA4Nx4PrXEfDvybJ
EIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQiCLjeZ3HtlRKld+9htAZtHFZ
osV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHhqoOmjXaCv58CSRAlAQIDAQAB
AoGBAJtDgCwZYv2FYVk0ABw6F6CWbuZLUVykks69AG0xasti7Xjh3AximUnZLefs
iuJqg2KpRzfv1CM+Cw5cp2GmIVvRqq0GlRZGxJ38AqH9oyUa2m3TojxWapY47zye
PYEjWwRTGlxUBkdujdcYj6/dojNkm4azsDXl9W5YaXiPfbgJAkEA4rlhSPXlohDk
FoyfX0v2OIdaTOcVpinv1jjbSzZ8KZACggjiNUVrSFV3Y4oWom93K5JLXf2mV0Sy
80mPR5jOdwJBAMwciAk8xyQKpMUGNhFX2jKboAYY1SJCfuUnyXHAPWeHp5xCL2UH
tjryJp/Vx8TgsFTGyWSyIE9R8hSup+32rkcCQBe+EAkC7yQ0np4Z5cql+sfarMMm
4+Z9t8b4N0a+EuyLTyfs5Dtt5JkzkggTeuFRyOoALPJP0K6M3CyMBHwb7WsCQQCi
TM2fCsUO06fRQu8bO1A1janhLz3K0DU24jw8RzCMckHE7pvhKhCtLn+n+MWwtzl/
L9JUT4+BgxeLepXtkolhAkEA2V7er7fnEuL0+kKIjmOm5F3kvMIDh9YC1JwLGSvu
1fnzxK34QwSdxgQRF1dfIKJw73lClQpHZfQxL/2XRG8IoA==+32rkcCQBe+EAkC7yQ0np4Z5cql+sfarMMm
4+Z9t8b4N0a+EuyLTyfs5Dtt5JkzkggTeuFRyOoALPJP0K6M3CyMBHwb7WsCQQCi
TM2fCsUO06fRQu8bO1A1janhLz3K0DU24jw8RzCMckHE7pvhKhCtLn+n+MWwtzl/
L9JUT4+BgxeLepXtkolhAkEA2V7er7fnEuL0+kKIjmOm5F3kvMIDh9YC1JwLGSvu
1fnzxK34QwSdxgQRF1dfIKJw73lClQpHZfQxL/2XRG8IoA==FV3Y4oWom93K5JLXf2mV0Sy
80mPR5jOdwJBAMwciAk8xyQKpMUGNhFX2jKboAYY1SJCfuUnyXHAPWeHp5xCL2UH
tjryJp/Vx8TgsFTGyWSyIE9R8hSup+32rkcCQBe+EAkC7yQ0np4Z5cql+sfarMMm
4+Z9t8b4N0a+EuyLTyfs5Dtt5JkzkggTeuFRyOoALPJP0K6M3CyMBHwb7WsCQQCi
TM2fCsUO06fRQu8bO1A1janhLz3K0DU24jw8RzCMckHE7pvhKhCtLn+n+MWwtzl/
L9JUT4+BgxeLepXtkolhAkEA2V7er7fnEuL0+kKIjmOm5F3kvMIDh9YC1JwLGSvu
1fnzxK34QwSdxgQRF1dfIKJw73lClQpHZfQxL/2XRG8IoA==";

            var encResult = CryptoHelper.RSAEncrypt(rawText, PublicKey);
            Assert.True(encResult.Success);

            var decResult = CryptoHelper.RSADecrypt(encResult.Value, PrivateKey);
            Assert.True(decResult.Success);

            Assert.Equal(rawText, decResult.Value);
        }

        [Fact]
        public void TestMakeAndVerifySign()
        {
            String signContent = "a=1&b=c&d=#s&e=2.0&k=ку";

            var signResult = CryptoHelper.MakeSign(signContent, PrivateKey, HashAlgorithmName.SHA256);
            Assert.True(signResult.Success);

            var verifyResult = CryptoHelper.VerifySign(signResult.Value, signContent, PublicKey, HashAlgorithmName.SHA256);
            Assert.True(verifyResult.Success && verifyResult.Value);
        }
    }
}
