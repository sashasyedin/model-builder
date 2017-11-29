using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelBuilder.Tests
{
    [TestClass]
    public class CustomTypeBuilder_CompileResultType
    {
        [TestMethod]
        public void CompileResultType_UnderValidCircumstances_ExpectSuccess()
        {
            // Arrange:
            var properties = new Dictionary<string, Type> { { "prop", typeof(int) } };

            // Act:
            var type = CustomTypeBuilder.CompileResultType("TestClass", properties);

            // Assert:
            Assert.IsNotNull(type);
            Assert.IsNotNull(type.AsType());

            var obj = Activator.CreateInstance(type.AsType());
            var prop = type.GetProperty("prop");
            prop.SetValue(obj, 1, null);

            Assert.AreEqual(1, prop.GetValue(obj, null));
        }
    }
}
