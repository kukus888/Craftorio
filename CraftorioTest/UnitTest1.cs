using System;
using Craftorio.Server;

namespace CraftorioTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ByteAdapterTest()
        {
            byte[] byteArr = {0x2F, 0x23, 0x55};
            string s = Craftorio.Server.Controllers.ByteAdapter.ByteArrayToString(byteArr);
            Assert.IsTrue(s == "/#U");
            s = "si@tS¡N2]Of6@45";
            byteArr = Craftorio.Server.Controllers.ByteAdapter.StringToByteArray(s);
            byte[] corrArr = { 115, 105, 64, 116, 83, 199, 78, 50, 93, 79, 102, 54, 64, 52, 53 };
            //compare byte arrays
            if(byteArr.Length == corrArr.Length)
            {
                for(int i = 0; i < corrArr.Length; i++)
                {
                    Assert.IsTrue(corrArr[i] == byteArr[i]);
                }
            }
            else
            {
                Assert.Fail("ByteAdapter.StringToByteArray returns wrong array!");
            }
            Assert.Pass("ByteAdapter OK");
        }
    }
}