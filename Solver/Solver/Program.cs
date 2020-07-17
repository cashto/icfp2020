using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using IcfpUtils;
using System.Reflection.Metadata.Ecma335;
using System.Data;

namespace Solver
{
    public class Program
    {
        static void Main(string[] args)
        {
            var symbols = new Dictionary<string, LispNode>();

            var lines = File.ReadLines(@"C:\Users\cashto\Documents\GitHub\icfp2020\work\galaxy.txt");
            foreach (var line in lines)
            {
                var x = line.Split('=');
                var symbol = x[0].Trim();
                var value = Parse(x[1]);

                symbols[symbol] = value;
            }

            Evaluate(symbols["galaxy"], symbols);
        }

        public static LispNode Evaluate(
            LispNode root,
            Dictionary<string, LispNode> symbols = null)
        {
            while (true)
            {
                var result = EvaluateOne(root);
                if (object.ReferenceEquals(root, result))
                {
                    result = DoSubstitutions(root, symbols);
                    if (object.ReferenceEquals(root, result))
                    {
                        return root;
                    }
                }

                root = result;
            }
        }

        public static LispNode Parse(string s)
        {
            var stack = new List<LispNode>();
            foreach (var token in s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()))
            {
                if (token == "ap")
                {
                    stack.Add(new LispNode() { Type = LispNodeType.Open });
                }
                else
                {
                    stack.Add(new LispNode() { Type = LispNodeType.Token, Text = token });

                    do
                    {
                        if (stack.Count == 1)
                        {
                            return stack.Last();
                        }

                        var node = stack.Last();
                        stack.RemoveAt(stack.Count - 1);

                        stack.Last().Children.Add(node);
                    } while (stack.Last().Children.Count == 2);
                }
            }

            throw new Exception($"Could not parse: {s}");
        }

        static LispNode DoSubstitutions(LispNode root, Dictionary<string, LispNode> symbols)
        {
            if (symbols == null)
            {
                return root;
            }

            if (root.Type == LispNodeType.Token)
            {
                if (root.Text.StartsWith(':'))
                {
                    return symbols[root.Text];
                }
                else
                {
                    return root;
                }
            }
            else
            {
                var ans = new LispNode() { Type = LispNodeType.Open };
                ans.Children.Add(DoSubstitutions(root.Children.First(), symbols));
                ans.Children.Add(DoSubstitutions(root.Children.Last(), symbols));
                return ans;
            }
        }

        static long? TryParseInt(LispNode node)
        {
            if (node.Type != LispNodeType.Token)
            {
                return null;
            }

            long ans;
            if (!long.TryParse(node.Text, out ans))
            {
                return null;
            }

            return ans;
        }

        static LispNode NumericFunc(
            Dictionary<string, LispNode> args,
            string funcName,
            Func<long, long, string> fn)
        {
            var arity = args.Count;
            var arg1Tree = Evaluate(args[":0"]);
            var arg2Tree = arity < 2 ? null : Evaluate(args[":1"]);

            long? arg1 = TryParseInt(arg1Tree);
            long? arg2 = arg2Tree == null ? 0 : TryParseInt(arg2Tree);

            if (arg1.HasValue && arg2.HasValue)
            {
                return new LispNode()
                {
                    Type = LispNodeType.Token,
                    Text = fn(arg1.Value, arg2.Value)
                };
            }

            var ans = new LispNode() { Type = LispNodeType.Open };
            ans.Children.Add(new LispNode() { Type = LispNodeType.Token, Text = funcName });
            ans.Children.Add(arg1Tree);

            if (arg2Tree != null)
            {
                var child = ans;
                ans = new LispNode() { Type = LispNodeType.Open };
                ans.Children.Add(child);
                ans.Children.Add(arg2Tree);
            }

            if (object.ReferenceEquals(arg1Tree, args[":0"]) &&
                (arity == 1 || object.ReferenceEquals(arg2Tree, args[":1"])))
            {
                return null;
            }

            return ans;
        }

        public static LispNode IncPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "inc", (a, b) => (a + 1).ToString());
        }

        public static LispNode DecPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "dec", (a, b) => (a - 1).ToString());
        }

        public static LispNode AddPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "add", (a, b) => (a + b).ToString());
        }

        public static LispNode NegPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "neg", (a, b) => (-a).ToString());
        }

        public static LispNode MulPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "mul", (a, b) => (a * b).ToString());
        }

        public static LispNode DivPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "div", (a, b) => (a / b).ToString());
        }

        public static LispNode EqPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "eq", (a, b) => a == b ? "t" : "f");
        }

        public static LispNode LtPatternFunc(Dictionary<string, LispNode> args)
        {
            return NumericFunc(args, "lt", (a, b) => a < b ? "t" : "f");
        }

        public static LispNode SPatternFunc(Dictionary<string, LispNode> args)
        {
            // S : ap ap ap s x0 x1 x2   =   ap ap x0 x2 ap x1 x2
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            var eval2 = Evaluate(args[":2"]);
            newRoot.Children.First().Children.Add(Evaluate(args[":0"]));
            newRoot.Children.First().Children.Add(eval2);
            newRoot.Children.Last().Children.Add(Evaluate(args[":1"]));
            newRoot.Children.Last().Children.Add(eval2);
            return newRoot;
        }

        public static LispNode CPatternFunc(Dictionary<string, LispNode> args)
        {
            // C : ap ap ap c x0 x1 x2   =   ap ap x0 x2 x1
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.First().Children.Add(Evaluate(args[":0"]));
            newRoot.Children.First().Children.Add(Evaluate(args[":2"]));
            newRoot.Children.Add(Evaluate(args[":1"]));
            return newRoot;
        }

        public static LispNode BPatternFunc(Dictionary<string, LispNode> args)
        {
            // B : ap ap ap b x0 x1 x2   =   ap x0 ap x1 x2
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(Evaluate(args[":0"]));
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.Last().Children.Add(Evaluate(args[":1"]));
            newRoot.Children.Last().Children.Add(Evaluate(args[":2"]));
            return newRoot;
        }

        public static LispNode TPatternFunc(Dictionary<string, LispNode> args)
        {
            // t : ap ap t x0 x1   =   x0
            return Evaluate(args[":0"]);
        }

        public static LispNode FPatternFunc(Dictionary<string, LispNode> args)
        {
            // f : ap ap f x0 x1   =   x1
            return Evaluate(args[":1"]);
        }

        public static LispNode IPatternFunc(Dictionary<string, LispNode> args)
        {
            // ap i x0 = x0
            return Evaluate(args[":0"]);
        }

        public static LispNode CarPatternFunc(Dictionary<string, LispNode> args)
        {
            return Evaluate(args[":0"]);
        }

        public static LispNode CdrPatternFunc(Dictionary<string, LispNode> args)
        {
            return Evaluate(args[":1"]);
        }

        public static LispNode IsNilPatternFunc(Dictionary<string, LispNode> args)
        {
            var arg0 = args[":0"];
            return new LispNode()
            {
                Type = LispNodeType.Token,
                Text = arg0.Type == LispNodeType.Token && arg0.Text == "nil" ? "t" : "f"
            };
        }

        public static LispNode ApPatternFunc(Dictionary<string, LispNode> args)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };

            ans.Children.Add(Evaluate(args[":0"]));
            ans.Children.Add(Evaluate(args[":1"]));
            if (object.ReferenceEquals(ans.Children.First(), args[":0"]) &&
                object.ReferenceEquals(ans.Children.Last(), args[":1"]))
            {
                return null;
            }

            return ans;
        }

        class PatternFunc
        {
            public PatternFunc(
                string pattern,
                Func<Dictionary<string, LispNode>, LispNode> func)
            {
                Pattern = Parse(pattern);
                Func = func;
            }

            public LispNode Pattern { get; private set; }
            public Func<Dictionary<string, LispNode>, LispNode> Func { get; private set; }
        }

        static List<PatternFunc> evalList =
            new List<PatternFunc>()
            {
                new PatternFunc("ap inc :0", IncPatternFunc),
                new PatternFunc("ap dec :0", DecPatternFunc),
                new PatternFunc("ap ap add :0 :1", AddPatternFunc),
                new PatternFunc("ap ap mul :0 :1", MulPatternFunc),
                new PatternFunc("ap ap div :0 :1", DivPatternFunc),
                new PatternFunc("ap ap eq :0 :1", EqPatternFunc),
                new PatternFunc("ap ap lt :0 :1", LtPatternFunc),
                new PatternFunc("ap neg :0", NegPatternFunc),
                new PatternFunc("ap ap ap s :0 :1 :2", SPatternFunc),
                new PatternFunc("ap ap ap c :0 :1 :2", CPatternFunc),
                new PatternFunc("ap ap ap b :0 :1 :2", BPatternFunc),
                new PatternFunc("ap ap t :0 :1", TPatternFunc),
                new PatternFunc("ap ap f :0 :1", FPatternFunc),
                new PatternFunc("ap i :0", IPatternFunc),
                new PatternFunc("ap car ap ap cons :0 :1", CarPatternFunc),
                new PatternFunc("ap cdr ap ap cons :0 :1", CdrPatternFunc),
                new PatternFunc("ap isnil :0", IsNilPatternFunc),
                new PatternFunc("ap :0 :1", ApPatternFunc),
            };

        public static LispNode EvaluateOne(LispNode root)
        {
            foreach (var patternFunc in evalList)
            {
                var matches = Match(root, patternFunc.Pattern);
                if (matches != null)
                {
                    var ret = patternFunc.Func(matches);
                    return ret ?? root;
                }
            }

            return root;
        }

        public static Dictionary<string, LispNode> Match(
            LispNode root,
            LispNode pattern)
        {
            var ans = new Dictionary<string, LispNode>();

            if (pattern.Type == LispNodeType.Open)
            {
                if (root.Type != LispNodeType.Open)
                {
                    return null;
                }

                var left = Match(root.Children.First(), pattern.Children.First());
                if (left == null)
                {
                    return null;
                }

                var right = Match(root.Children.Last(), pattern.Children.Last());
                if (right == null)
                {
                    return null;
                }

                foreach (var item in left)
                {
                    ans[item.Key] = item.Value;
                }

                foreach (var item in right)
                {
                    ans[item.Key] = item.Value;
                }

                return ans;
            }
            else if (pattern.Text.StartsWith(':'))
            {
                ans[pattern.Text] = root;
                return ans;
            }
            else
            {
                if (root.Text != pattern.Text)
                {
                    return null;
                }

                return ans;
            }
        }

        public static string Modulate(long data)
        {
            var sb = new StringBuilder();
            if (data == 0)
            {
                return "010";
            }
            else if (data < 0)
            {
                sb.Append("10");
                data = -data;
            }
            else
            {
                sb.Append("01");
            }

            var binaryDigits = Convert.ToString(data, 2);
            foreach (var i in Enumerable.Range(0, 1 + (binaryDigits.Length - 1) / 4))
            {
                sb.Append("1");
            }
            sb.Append("0");

            foreach (var i in Enumerable.Range(0, 3 - (binaryDigits.Length + 3) % 4))
            {
                sb.Append("0");
            }
            sb.Append(binaryDigits);

            return sb.ToString();
        }

        public static Tuple<long, string> Demodulate(string data)
        {
            var isNeg = data.StartsWith("10");
            var width = data.Skip(2).TakeWhile(i => i == '1').Count();
            var binaryDigits = data.Substring(3 + width, width * 4);
            var remainder = data.Substring(3 + width * 5);
            var ans = Convert.ToInt64(binaryDigits, 2);
            return Tuple.Create(isNeg ? -ans : ans, remainder);
        }

        static string Modulate(List<int> data)
        {
            return "";
        }
    }
}
