using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using V22048Game.Elements;

namespace V22048Test
{
    [TestClass]
    public class ComplexNumberTesting
    {
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1 + 2i")]
        public void TestConvertToString1()
        {
            GameValue z = new GameValue(1, 2);

            Assert.AreEqual("1+2i", z.ToString());
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1 + i")]
        public void TestConvertToString2()
        {
            GameValue z = new GameValue(1, 1);

            Assert.AreEqual("1+i", z.ToString());
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "-1 - 2i")]
        public void TestConvertToString3()
        {
            GameValue z = new GameValue(-1, -2);

            Assert.AreEqual("-1-2i", z.ToString());
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1.455 - 2.037i")]
        public void TestConvertToString4()
        {
            GameValue z = new GameValue(1.455, -2.037);

            Assert.AreEqual("1.46-2.04i", z.ToString());
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1.2")]
        public void TestConvertToString5()
        {
            GameValue z = new GameValue(1.2, 0);

            Assert.AreEqual("1.2", z.ToString());
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1")]
        [TestProperty("Power", "1")]
        public void TestRaiseToPower1()
        {
            GameValue z = new GameValue(1, 0);
            z = z.Power(1);
            Assert.AreEqual(z, GameValue.unit);
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "1")]
        [TestProperty("Power", "2")]
        public void TestRaiseToPower2()
        {
            GameValue z = new GameValue(1, 0);
            z = z.Power(2);
            Assert.AreEqual(z, GameValue.unit);
        }
        [TestMethod]
        [TestCategory("ComplexNumbers")]
        [TestProperty("Value", "-1")]
        [TestProperty("Power", "2")]
        public void TestRaiseToPower3()
        {
            GameValue z = new GameValue(-1, 0);
            z = z.Power(2);
            Assert.IsTrue(z.Equals(GameValue.unit));
        }
    }
}
