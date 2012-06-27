using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inferables.Test
{
    [TestClass]
    public class BindingTest
    {
        [TestMethod]
        public void DefaultBindingCanBeCreated()
        {
            var binding = new Binding();

            var maps = binding.Maps.ToArray();
            var map = maps.FirstOrDefault();

            Assert.IsNotNull(map);
            Assert.AreEqual(maps.Length, 1);
            Assert.AreEqual(map.Value, "* => ~");
        }

        [TestMethod]
        public void CustomBindingCanBeCreated()
        {
            var binding = new Binding("* => ~.Interfaces");
            var maps = binding.Maps.ToArray();
            var map = maps.FirstOrDefault();


            Assert.IsNotNull(map);
            Assert.AreEqual(maps.Length, 1);
            Assert.AreEqual("* => ~.Interfaces", map.Value);
        }


        [TestMethod]
        public void CustomBindingsCanBeCreatedWithShortHand()
        {
            TestShortHand("* => ~", "~");
            TestShortHand("* => ~.Interfaces", "~.Interfaces");
            TestShortHand("* => -", "-");
            TestShortHand("* => -.-", "-.-");
            TestShortHand("* => -.-.Interfaces", "-.-.Interfaces");
            TestShortHand("* => AbsolutePath", "AbsolutePath");
        }


        private void TestShortHand(string expected, string expression)
        {
            var binding = new Binding(expression);
            var maps = binding.Maps.ToArray();
            var map = maps.FirstOrDefault();
            Assert.IsNotNull(map);
            Assert.AreEqual(maps.Length, 1);
            Assert.AreEqual(expected, map.Value);

        }
    }
}
