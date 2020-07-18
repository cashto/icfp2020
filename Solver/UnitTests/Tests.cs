using IcfpUtils;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solver;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using System;

namespace UnitTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestModulate()
        {
            Assert.AreEqual("010", Program.Modulate(0));
            Assert.AreEqual("01100001", Program.Modulate(1));
            Assert.AreEqual("10100001", Program.Modulate(-1));
            Assert.AreEqual("01101111", Program.Modulate(15));
            Assert.AreEqual("0111000010000", Program.Modulate(16));
            Assert.AreEqual("1101100001110110001000", Program.Modulate(Program.Parse("ap ap cons 1 ap ap cons 2 nil")));
            Assert.AreEqual(
                "1101100001111101100010110110001100110110010000",
                Program.Modulate(Program.Parse("ap ap cons 1 ap ap cons ap ap cons 2 ap ap cons 3 nil ap ap cons 4 nil")));

            Console.WriteLine(Program.Modulate(Program.Parse("ap ap cons 0 nil")));
            Console.WriteLine(Program.Demodulate("110110000111011111100001000001011010010100").Item1);
            Console.WriteLine(Program.Demodulate("110110000111011111100001000001011100010100").Item1);
        }

        [TestMethod]
        public void TestEvaluations()
        {
            TestEvaluate("ap inc 0", "1");
            TestEvaluate("ap inc 1", "2");
            TestEvaluate("ap inc 2", "3");
            TestEvaluate("ap inc 3", "4");
            TestEvaluate("ap inc 300", "301");
            TestEvaluate("ap inc 301", "302");
            TestEvaluate("ap inc -1", "0");
            TestEvaluate("ap inc -2", "-1");
            TestEvaluate("ap inc -3", "-2");
            TestEvaluate("ap dec 1", "0");
            TestEvaluate("ap dec 2", "1");
            TestEvaluate("ap dec 3", "2");
            TestEvaluate("ap dec 4", "3");
            TestEvaluate("ap dec 1024", "1023");
            TestEvaluate("ap dec 0", "-1");
            TestEvaluate("ap dec -1", "-2");
            TestEvaluate("ap dec -2", "-3");
            TestEvaluate("ap ap add 1 2", "3");
            TestEvaluate("ap ap add 2 1", "3");
            TestEvaluate("ap ap add 0 1", "1");
            TestEvaluate("ap ap add 2 3", "5");
            TestEvaluate("ap ap add 3 5", "8");
            TestEvaluate("ap ap mul 4 2", "8");
            TestEvaluate("ap ap mul 3 4", "12");
            TestEvaluate("ap ap mul 3 -2", "-6");
            TestEvaluate("ap ap div 4 2", "2");
            TestEvaluate("ap ap div 4 3", "1");
            TestEvaluate("ap ap div 4 4", "1");
            TestEvaluate("ap ap div 4 5", "0");
            TestEvaluate("ap ap div 5 2", "2");
            TestEvaluate("ap ap div 6 -2", "-3");
            TestEvaluate("ap ap div 5 -3", "-1");
            TestEvaluate("ap ap div -5 3", "-1");
            TestEvaluate("ap ap div -5 -3", "1");
            TestEvaluate("ap ap eq 0 -2", "f");
            TestEvaluate("ap ap eq 0 -1", "f");
            TestEvaluate("ap ap eq 0 0", "t");
            TestEvaluate("ap ap eq 0 1", "f");
            TestEvaluate("ap ap eq 0 2", "f");
            TestEvaluate("ap ap eq 1 -1", "f");
            TestEvaluate("ap ap eq 1 0", "f");
            TestEvaluate("ap ap eq 1 1", "t");
            TestEvaluate("ap ap eq 1 2", "f");
            TestEvaluate("ap ap eq 1 3", "f");
            TestEvaluate("ap ap eq 2 0", "f");
            TestEvaluate("ap ap eq 2 1", "f");
            TestEvaluate("ap ap eq 2 2", "t");
            TestEvaluate("ap ap eq 2 3", "f");
            TestEvaluate("ap ap eq 2 4", "f");
            TestEvaluate("ap ap eq 19 20", "f");
            TestEvaluate("ap ap eq 20 20", "t");
            TestEvaluate("ap ap eq 21 20", "f");
            TestEvaluate("ap ap eq -19 -20", "f");
            TestEvaluate("ap ap eq -20 -20", "t");
            TestEvaluate("ap ap eq -21 -20", "f");
            TestEvaluate("ap ap lt 0 -1", "f");
            TestEvaluate("ap ap lt 0 0", "f");
            TestEvaluate("ap ap lt 0 1", "t");
            TestEvaluate("ap ap lt 0 2", "t");
            TestEvaluate("ap ap lt 1 0", "f");
            TestEvaluate("ap ap lt 1 1", "f");
            TestEvaluate("ap ap lt 1 2", "t");
            TestEvaluate("ap ap lt 1 3", "t");
            TestEvaluate("ap ap lt 2 1", "f");
            TestEvaluate("ap ap lt 2 2", "f");
            TestEvaluate("ap ap lt 2 3", "t");
            TestEvaluate("ap ap lt 2 4", "t");
            TestEvaluate("ap ap lt 19 20", "t");
            TestEvaluate("ap ap lt 20 20", "f");
            TestEvaluate("ap ap lt 21 20", "f");
            TestEvaluate("ap ap lt -19 -20", "f");
            TestEvaluate("ap ap lt -20 -20", "f");
            TestEvaluate("ap ap lt -21 -20", "t");
            TestEvaluate("ap neg 0", "0");
            TestEvaluate("ap neg 1", "-1");
            TestEvaluate("ap neg -1", "1");
            TestEvaluate("ap neg 2", "-2");
            TestEvaluate("ap neg -2", "2");
            TestEvaluate("ap ap ap s add inc 1", "3");
            TestEvaluate("ap ap ap s mul ap add 1 6", "42");
            TestEvaluate("ap ap ap c add 1 2", "3");
            TestEvaluate("ap ap t 1 5", "1");
            TestEvaluate("ap ap t t i", "t");
            TestEvaluate("ap ap t t ap inc 5", "t");
            TestEvaluate("ap ap t ap inc 5 t", "6");
            TestEvaluate("ap i 1", "1");
            TestEvaluate("ap i i", "i");
            TestEvaluate("ap i add", "add");
            TestEvaluate("ap i ap add 1", "ap add 1");
            TestEvaluate("ap car ap ap cons x0 x1", "x0");
            TestEvaluate("ap cdr ap ap cons x0 x1", "x1");
            TestEvaluate("ap isnil nil", "t");
            TestEvaluate("ap isnil ap ap cons x0 x1", "f");
        }

        [TestMethod]
        public void TestPower2()
        {
            var symbols = new Dictionary<string, LispNode>()
            {
                { "pwr2", Program.Parse("ap ap s ap ap c ap eq 0 1 ap ap b ap mul 2 ap ap b pwr2 ap add -1") }
            };

            TestEvaluate("ap pwr2 3", "8", symbols);
        }

        private void TestEvaluate(string fn, string reference, Dictionary<string, LispNode> symbols = null)
        {
            var actual = Program.Evaluate(Program.Parse(fn), symbols ?? new Dictionary<string, LispNode>());
            var expected = Program.Parse(reference);
            Assert.IsNotNull(Program.Match(expected, actual), $"fn [{fn}] expected [{expected}] actual [{Program.Serialize(actual)}]");
        }
    }
}
