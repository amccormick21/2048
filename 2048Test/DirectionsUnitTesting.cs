using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048Game;

namespace _2048Test
{
    [TestClass]
    public class DirectionsUnitTesting
    {
        [TestMethod]
        [TestCategory("Direction Opposite Testing")]
        [TestCategory("Function Testing")]
        public void TestGetOpposite1()
        {
            DragDirection Direction = DragDirections.Opposite(DragDirection.Left);

            Assert.AreEqual(DragDirection.Right, Direction);
        }

        [TestMethod]
        [TestCategory("Direction Opposite Testing")]
        [TestCategory("Function Testing")]
        public void TestGetOpposite2()
        {
            DragDirection Direction = DragDirections.Opposite(DragDirection.Down);

            Assert.AreEqual(DragDirection.Up, Direction);
        }
    }
}
