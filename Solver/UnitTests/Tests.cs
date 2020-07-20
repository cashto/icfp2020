using IcfpUtils;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solver;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestModulate()
        {
            Assert.AreEqual("010", Common.Modulate(0));
            Assert.AreEqual("01100001", Common.Modulate(1));
            Assert.AreEqual("10100001", Common.Modulate(-1));
            Assert.AreEqual("01101111", Common.Modulate(15));
            Assert.AreEqual("0111000010000", Common.Modulate(16));
            Assert.AreEqual("1101100001110110001000", Common.Modulate(Common.Parse("ap ap cons 1 ap ap cons 2 nil")));
            Assert.AreEqual(
                "1101100001111101100010110110001100110110010000",
                Common.Modulate(Common.Parse("ap ap cons 1 ap ap cons ap ap cons 2 ap ap cons 3 nil ap ap cons 4 nil")));

            Console.WriteLine(Common.Modulate(Common.Parse("ap ap cons 0 nil")));
            Console.WriteLine(Common.Demodulate("110110000111011111100001000001011010010100").Item1);
            Console.WriteLine(Common.Demodulate("110110000111011111100001000001011100010100").Item1);
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
            //TestEvaluate("ap ap mul 0 x0", "0");
            //TestEvaluate("ap ap mul x0 0", "0");
            //TestEvaluate("ap ap mul 1 x0", "x0");
            //TestEvaluate("ap ap mul x0 1", "x0");
            TestEvaluate("ap ap div 4 2", "2");
            TestEvaluate("ap ap div 4 3", "1");
            TestEvaluate("ap ap div 4 4", "1");
            TestEvaluate("ap ap div 4 5", "0");
            TestEvaluate("ap ap div 5 2", "2");
            TestEvaluate("ap ap div 6 -2", "-3");
            TestEvaluate("ap ap div 5 -3", "-1");
            TestEvaluate("ap ap div -5 3", "-1");
            TestEvaluate("ap ap div -5 -3", "1");
            //TestEvaluate("ap ap div x0 1", "x0");
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
            //TestEvaluate("ap ap ap b inc dec x0", "x0");
            TestEvaluate("ap ap t 1 5", "1");
            TestEvaluate("ap ap t t i", "t");
            TestEvaluate("ap ap t t ap inc 5", "t");
            TestEvaluate("ap ap t ap inc 5 t", "6");
            TestEvaluate("ap i 1", "1");
            TestEvaluate("ap i i", "i");
            TestEvaluate("ap i add", "add");
            // TestEvaluate("ap ap eq x0 x0", "t"); // TODO
            TestEvaluate("ap i ap add 1", "ap add 1");
            TestEvaluate("ap car ap ap cons x0 x1", "x0");
            TestEvaluate("ap cdr ap ap cons x0 x1", "x1");
            TestEvaluate("ap isnil nil", "t");
            TestEvaluate("ap isnil ap ap cons x0 x1", "f");
            //TestEvaluate("ap dec ap ap add x0 1", "x0");
            //TestEvaluate("ap s t", "f");
        }

        [TestMethod]
        public void TestPower2()
        {
            var symbols = new Dictionary<string, LispNode>()
            {
                { "pwr2", Common.Parse("ap ap s ap ap c ap eq 0 1 ap ap b ap mul 2 ap ap b pwr2 ap add -1") }
            };

            TestEvaluate("ap pwr2 8", "256", symbols);
        }

        [TestMethod]
        public void TestTreeToList()
        {
            var tree = Lisp.Parse("((cons 0) ((cons ((cons 0) ((cons ((cons 0) nil)) ((cons 0) ((cons nil) nil))))) ((cons ((cons ((cons ((cons -1) -3)) ((cons ((cons 0) -3)) ((cons ((cons 1) -3)) ((cons ((cons 2) -2)) ((cons ((cons -2) -1)) ((cons ((cons -1) -1)) ((cons ((cons 0) -1)) ((cons ((cons 3) -1)) ((cons ((cons -3) 0)) ((cons ((cons -1) 0)) ((cons ((cons 1) 0)) ((cons ((cons 3) 0)) ((cons ((cons -3) 1)) ((cons ((cons 0) 1)) ((cons ((cons 1) 1)) ((cons ((cons 2) 1)) ((cons ((cons -2) 2)) ((cons ((cons -1) 3)) ((cons ((cons 0) 3)) ((cons ((cons 1) 3)) nil))))))))))))))))))))) ((cons ((cons ((cons -7) -3)) ((cons ((cons -8) -2)) nil))) ((cons nil) nil)))) nil)))").Children.First();
            //var tree = Program.Parse("ap ap cons 42 nil");
            //var tree = Program.Parse("ap ap cons 42 ap ap cons ap ap cons 13 ap ap cons 101 nil nil");
            //Console.WriteLine(Program.TreeToJson(tree));
        }

        [TestMethod]
        public void TestFlatten()
        {
            var test =
                new LispNode() {
                    new LispNode("0"),
                    new LispNode("12345678"),
                    new LispNode() {
                        new LispNode("1"),
                        new LispNode("2"),
                        new LispNode() {
                            new LispNode("3"),
                            new LispNode("4")
                        }
                    }
                };

            var notFlat = Common.Unflatten(test);
            var flat = Common.Flatten(notFlat);
            Console.WriteLine(test);
            Console.WriteLine(notFlat);
            Console.WriteLine(flat);
            Assert.AreEqual(test.ToString(), flat.ToString());
        }

        [TestMethod]
        public void TestSolver()
        {
            var gameResponse = Lisp.Parse("(1 1 (256 1 (448 1 64) (16 128) ()) (0 (16 128) (((1 0 (-14 0) (0 0) (10 10 10 1) 0 64 1) ()) ((0 1 (14 0) (0 0) (1 2 1 1) 0 64 1) ()))))")[0];
            var response = Program.MakeCommandsRequest("123", gameResponse);
            Console.WriteLine(Common.Flatten(response));
        }

        private void TestEvaluate(string fn, string reference, Dictionary<string, LispNode> symbols = null)
        {
/*
            var actual = Program.Evaluate(Program.Parse(fn), symbols ?? new Dictionary<string, LispNode>(), substitutions: true);
            var expected = Program.Parse(reference);
            Assert.IsNotNull(Program.Match(expected, actual), $"fn [{fn}] expected [{expected}] actual [{Program.Serialize(actual)}]");
*/
        }
    }
}
