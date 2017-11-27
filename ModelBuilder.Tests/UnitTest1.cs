using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelBuilder.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var class1 = new Class1();
            var zero = class1.ReturnZero();
            
            Assert.AreEqual(0, zero);
        }
    }
}