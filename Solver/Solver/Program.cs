using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using IcfpUtils;
using System.Reflection.Metadata.Ecma335;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Net.WebSockets;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

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

            //var errorLine = "((cons 0) ((cons ((cons 0) ((cons ((cons 0) nil)) ((cons 0) ((cons nil) nil))))) ((cons ((cons ((cons ((cons -1) -3)) ((cons ((cons 0) -3)) ((cons ((cons 1) -3)) ((cons ((cons 2) -2)) ((cons ((cons -2) -1)) ((cons ((cons -1) -1)) ((cons ((cons 0) -1)) ((cons ((cons 3) -1)) ((cons ((cons -3) 0)) ((cons ((cons -1) 0)) ((cons ((cons 1) 0)) ((cons ((cons 3) 0)) ((cons ((cons -3) 1)) ((cons ((cons 0) 1)) ((cons ((cons 1) 1)) ((cons ((cons 2) 1)) ((cons ((cons -2) 2)) ((cons ((cons -1) 3)) ((cons ((cons 0) 3)) ((cons ((cons 1) 3)) ((cons (((cons -3) -3) ((c ((b b) ((b :1162) (add ((c ((b b) ((b :1115) cdr))) ((c :1126) cdr)))))) (add 63)))) ((:1126 (((:1127 ((c ((b b) ((b :1115) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) ((c :1126) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) cons) 64)) ((c :1172) ((:1162 -3) -3))))))))))))))))))))))))) ((cons ((:1183 ((:1183 (:1214 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -5) 0))) ((:1162 -3) -3))) ((cons ((:1183 (:1230 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -3) -3))) nil)))) nil)))";
            //var errorLine ="((cons 0) ((cons ((cons 0) ((cons ((cons 0) nil)) ((cons 0) ((cons nil) nil))))) ((cons ((cons ((cons ((cons -1) -3)) ((cons ((cons 0) -3)) ((cons ((cons 1) -3)) ((cons ((cons 2) -2)) ((cons ((cons -2) -1)) ((cons ((cons -1) -1)) ((cons ((cons 0) -1)) ((cons ((cons 3) -1)) ((cons ((cons -3) 0)) ((cons ((cons -1) 0)) ((cons ((cons 1) 0)) ((cons ((cons 3) 0)) ((cons ((cons -3) 1)) ((cons ((cons 0) 1)) ((cons ((cons 1) 1)) ((cons ((cons 2) 1)) ((cons ((cons -2) 2)) ((cons ((cons -1) 3)) ((cons ((cons 0) 3)) ((cons ((cons 1) 3)) ((cons ((:1162 ((add ((c ((b b) ((b :1115) cdr))) ((c :1126) cdr))) -3)) 60)) ((:1126 (((:1127 ((c ((b b) ((b :1115) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) ((c :1126) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) cons) 64)) ((c :1172) ((:1162 -3) -3))))))))))))))))))))))))) ((cons ((:1183 ((:1183 (:1214 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -5) 0))) ((:1162 -3) -3))) ((cons ((:1183 (:1230 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -3) -3))) nil)))) nil)))";
            //var errorLine = "((add ((c ((b b) ((b :1115) cdr))) ((c :1126) cdr))) -3)";
            //var errorLine = "((cons 0) ((cons ((cons 0) ((cons ((cons 0) nil)) ((cons 0) ((cons nil) nil))))) ((cons ((cons ((cons ((cons -1) -3)) ((cons ((cons 0) -3)) ((cons ((cons 1) -3)) ((cons ((cons 2) -2)) ((cons ((cons -2) -1)) ((cons ((cons -1) -1)) ((cons ((cons 0) -1)) ((cons ((cons 3) -1)) ((cons ((cons -3) 0)) ((cons ((cons -1) 0)) ((cons ((cons 1) 0)) ((cons ((cons 3) 0)) ((cons ((cons -3) 1)) ((cons ((cons 0) 1)) ((cons ((cons 1) 1)) ((cons ((cons 2) 1)) ((cons ((cons -2) 2)) ((cons ((cons -1) 3)) ((cons ((cons 0) 3)) ((cons ((cons 1) 3)) ((cons (((:1162 -3) -3) ((c ((b b) ((b :1162) (add ((c ((b b) ((b :1115) cdr))) ((c :1126) cdr)))))) (add 63)))) ((:1126 (((:1127 ((c ((b b) ((b :1115) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) ((c :1126) ((s ((b :1162) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) cons) 64)) ((c :1172) ((:1162 -3) -3))))))))))))))))))))))))) ((cons ((:1183 ((:1183 (:1214 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -5) 0))) ((:1162 -3) -3))) ((cons ((:1183 (:1230 ((((:1204 ((:1162 0) 0)) ((((:1166 -3) -3) 7) 7)) 0) -1))) ((:1162 -3) -3))) nil)))) nil)))";
            //var errorLine = "((cons 0) ((cons ((cons 0) ((cons ((cons 0) nil)) ((cons 0) ((cons nil) nil))))) ((cons ((cons ((cons ((cons -1) -3)) ((cons ((cons 0) -3)) ((cons ((cons 1) -3)) ((cons ((cons 2) -2)) ((cons ((cons -2) -1)) ((cons ((cons -1) -1)) ((cons ((cons 0) -1)) ((cons ((cons 3) -1)) ((cons ((cons -3) 0)) ((cons ((cons -1) 0)) ((cons ((cons 1) 0)) ((cons ((cons 3) 0)) ((cons ((cons -3) 1)) ((cons ((cons 0) 1)) ((cons ((cons 1) 1)) ((cons ((cons 2) 1)) ((cons ((cons -2) 2)) ((cons ((cons -1) 3)) ((cons ((cons 0) 3)) ((cons ((cons 1) 3)) ((c ((b b) ((b cons) ((s ((b cons) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))) ((c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b cons)))) (c ((s ((b b) ((c isnil) nil))) ((c b) ((s ((b c) ((b (b b)) (b :1115)))) (c :1126))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))) ((s ((b cons) ((c ((s ((b b) ((c ((b b) add)) neg))) ((b (s mul)) div))) 7))) ((c div) 7))))))))))))))))))))))))) ((cons ((cons ((cons -7) -3)) ((cons ((cons -8) -2)) nil))) ((cons nil) nil)))) nil)))";
            //Evaluate(Lisp.Parse(GetStream(errorLine)).Children.First(), symbols, substitutions: true, debug: true);

            var state = Parse("nil");
            var vector = Parse("ap ap cons 0 0");
            var protocolResult = Interact(symbols["galaxy"], state, vector, symbols);

            Console.WriteLine("DONE!");
            Console.WriteLine(protocolResult);
        }

        public static MemoryStream GetStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }

        public static string Serialize(LispNode node)
        {
            if (node.Type == LispNodeType.Token)
            {
                return node.Text + " ";
            }
            else
            {
                return "ap " + Serialize(node.Children.First()) + Serialize(node.Children.Last());
            }
        }

        static void DeadCode2(string apiKey)
        { 
            var httpClient = new HttpClient();
            var result = httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://icfpc2020-api.testkontur.ru/aliens/send?apiKey={apiKey}")
                {
                    Content = new StringContent("1101000")
                }).Result;
            Console.WriteLine(Demodulate(result.Content.ReadAsStringAsync().Result).Item1);
        }

        static LispNode Interact(
            LispNode protocol,
            LispNode state,
            LispNode vector,
            Dictionary<string, LispNode> symbols)
        {
            var protocolResultRoot = new LispNode() { Type = LispNodeType.Open };
            protocolResultRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            protocolResultRoot.Children.First().Children.Add(protocol);
            protocolResultRoot.Children.First().Children.Add(state);
            protocolResultRoot.Children.Add(vector);
            return Evaluate(protocolResultRoot, symbols, substitutions: true, debug: true);
        }

        public static LispNode Evaluate(
            LispNode root,
            Dictionary<string, LispNode> symbols,
            bool substitutions = false,
            bool debug = false)
        {
            while (true)
            {
                if (debug)
                {
                    Console.WriteLine("--------");
                    Console.WriteLine(root);
                    Console.WriteLine("--------");
                }

                var newRoot = EvaluateOne(root, symbols);

                if (newRoot == null)
                {
                    if (substitutions)
                    {
                        Console.WriteLine("DoSubstitution");
                        newRoot = DoSubstitution(root, symbols);
                    }
                    
                    if (newRoot == null || object.ReferenceEquals(root, newRoot))
                    {
                        return root;
                    }
                }

                root = newRoot;
            }
        }

        //public static LispNode EvaluateLeft(
        //    LispNode root,
        //    Dictionary<string, LispNode> symbols)
        //{
        //    while (true)
        //    {
        //        var newRoot = EvaluateOne(root, symbols);
        //        if (!object.ReferenceEquals(newRoot, root))
        //        {
        //            return newRoot;
        //        }

        //        if (root.Type == LispNodeType.Token)
        //        {
        //            return oldRoot;
        //        }
        //    }
        //}

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

        static LispNode DoSubstitution(LispNode root, Dictionary<string, LispNode> symbols)
        {
            if (symbols == null)
            {
                return root;
            }

            if (root.Type == LispNodeType.Token)
            {
                if (symbols.ContainsKey(root.Text))
                {
                    return symbols[root.Text]; // DEREF
                }
                else
                {
                    return root;
                }
            }
            else
            {
                var leftTree = DoSubstitution(root.Children.First(), symbols);
                var rightTree = DoSubstitution(root.Children.Last(), symbols);
                
                if (object.ReferenceEquals(leftTree, root.Children.First()) &&
                    object.ReferenceEquals(rightTree, root.Children.Last()))
                {
                    return root;
                }

                var ans = new LispNode() { Type = LispNodeType.Open };
                ans.Children.Add(leftTree);
                ans.Children.Add(rightTree);
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
            Dictionary<string, LispNode> symbols,
            string funcName,
            Func<long, long, string> fn)
        {
            var arity = args.Count;
            var arg1Tree = Evaluate(args["x0"], symbols);
            var arg2Tree = arity < 2 ? null : Evaluate(args["x1"], symbols);

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

            return null;
/*
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

            if (object.ReferenceEquals(arg1Tree, args["x0"]) &&
                (arity == 1 || object.ReferenceEquals(arg2Tree, args["x1"])))
            {
                return null;
            }

            return ans;
*/
        }

        static LispNode IncPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "inc", (a, b) => (a + 1).ToString());
        }

        static LispNode DecPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "dec", (a, b) => (a - 1).ToString());
        }

        static LispNode AddPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "add", (a, b) => (a + b).ToString());
        }

        static LispNode NegPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "neg", (a, b) => (-a).ToString());
        }

        static LispNode MulPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "mul", (a, b) => (a * b).ToString());
        }

        static LispNode DivPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "div", (a, b) => (a / b).ToString());
        }

        static LispNode EqPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            //if (args["x0"].Type == LispNodeType.Token &&
            //    args["x1"].Type == LispNodeType.Token &&
            //    args["x0"].Text == args["x1"].Text)
            //{
            //    return new LispNode() { Type = LispNodeType.Token, Text = "t" };
            //}
            
            if (Match(args["x0"], args["x1"]) != null)
            {
                return new LispNode() { Type = LispNodeType.Token, Text = "t" };
            }

            return NumericFunc(args, symbols, "eq", (a, b) => a == b ? "t" : "f");
        }

        static LispNode LtPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return NumericFunc(args, symbols, "lt", (a, b) => a < b ? "t" : "f");
        }

        static LispNode SPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // S : ap ap ap s x0 x1 x2   =   ap ap x0 x2 ap x1 x2
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            var args2 = Evaluate(args["x2"], symbols);
            //var args2 = args["x2"];
            newRoot.Children.First().Children.Add(args["x0"]);
            newRoot.Children.First().Children.Add(args2);
            newRoot.Children.Last().Children.Add(args["x1"]);
            newRoot.Children.Last().Children.Add(args2);
            return newRoot; 
        }

        static LispNode CPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // C : ap ap ap c x0 x1 x2   =   ap ap x0 x2 x1
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.First().Children.Add(args["x0"]);
            newRoot.Children.First().Children.Add(args["x2"]);
            newRoot.Children.Add(args["x1"]);
            return newRoot;
        }

        static LispNode BPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // B : ap ap ap b x0 x1 x2   =   ap x0 ap x1 x2
            var newRoot = new LispNode() { Type = LispNodeType.Open };
            newRoot.Children.Add(args["x0"]);
            newRoot.Children.Add(new LispNode() { Type = LispNodeType.Open });
            newRoot.Children.Last().Children.Add(args["x1"]);
            newRoot.Children.Last().Children.Add(args["x2"]);
            return newRoot;
        }

        static LispNode TPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // t : ap ap t x0 x1   =   x0
            return args["x0"];
        }

        static LispNode FPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // f : ap ap f x0 x1   =   x1
            return args["x1"];
        }

        static LispNode IPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // ap i x0 = x0
            return args["x0"];
        }

        static LispNode CarPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return args["x0"];
        }

        static LispNode CarNonConsPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            ans.Children.Add(args["x0"]);
            ans.Children.Add(new LispNode() { Type = LispNodeType.Token, Text = "t" });
            return ans;
        }

        static LispNode CdrNonConsPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            ans.Children.Add(args["x0"]);
            ans.Children.Add(new LispNode() { Type = LispNodeType.Token, Text = "f" });
            return ans;
        }

        static LispNode CdrPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return args["x1"];
        }

        static LispNode ZeroPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return new LispNode() { Type = LispNodeType.Token, Text = "0" };
        }

        static LispNode IsNilPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var arg0 = Evaluate(args["x0"], symbols);
            return new LispNode()
            {
                Type = LispNodeType.Token,
                Text = arg0.Type == LispNodeType.Token && arg0.Text == "nil" ? "t" : "f"
            };
        }

        static LispNode ApPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            var arg0 = args["x0"];
            var arg1 = args["x1"];

            ans.Children.Add(Evaluate(arg0, symbols));
            ans.Children.Add(Evaluate(arg1, symbols));

            if (!object.ReferenceEquals(ans.Children.Last(), arg1) ||
                !object.ReferenceEquals(ans.Children.First(), arg0))
            {
                return ans;
            }

            //if (arg1.Type == LispNodeType.Token && arg1.Text.StartsWith(':'))
            //{
            //    return null;
            //}

            //ans.Children.Add(Evaluate(arg1, symbols));
            //if (object.ReferenceEquals(ans.Children.Last(), arg1))
            //{
            //    return null;
            //}

            return null;
        }

        static LispNode RightApPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            var arg0 = args["x0"];
            var arg1 = args["x1"];

            ans.Children.Add(arg0);
            ans.Children.Add(Evaluate(arg1, symbols));
            if (!object.ReferenceEquals(ans.Children.Last(), arg1))
            {
                return ans;
            }

            return null;
        }

        static LispNode DereferencePatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var arg0 = args["x0"];
            if (arg0.Type != LispNodeType.Token)
            {
                return null;
            }

            if (!symbols.ContainsKey(arg0.Text))
            {
                return null;
            }

            return symbols[arg0.Text]; // DEREF
        }

        static LispNode ConsPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var root = new LispNode() { Type = LispNodeType.Open };
            root.Children.Add(new LispNode() { Type = LispNodeType.Open });
            root.Children.First().Children.Add(args["x2"]);
            root.Children.First().Children.Add(args["x0"]);
            root.Children.Add(args["x1"]);
            return root;
        }

        static LispNode NilPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return new LispNode() { Type = LispNodeType.Token, Text = "t" };
        }

        static LispNode ApSTPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            return new LispNode() { Type = LispNodeType.Token, Text = "f" };
        }

        static LispNode NotImplementedPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            throw new NotImplementedException();
        }

        static LispNode MakeIncPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            ans.Children.Add(new LispNode() { Type = LispNodeType.Token, Text = "inc" });
            ans.Children.Add(args["x0"]);
            return ans;
        }

        static LispNode MakeDecPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode() { Type = LispNodeType.Open };
            ans.Children.Add(new LispNode() { Type = LispNodeType.Token, Text = "dec" });
            ans.Children.Add(args["x0"]);
            return ans;
        }

        class PatternFunc
        {
            public PatternFunc(
                string pattern,
                Func<Dictionary<string, LispNode>, Dictionary<string, LispNode>, LispNode> func)
            {
                Pattern = Parse(pattern);
                Func = func;
            }

            public LispNode Pattern { get; private set; }
            public Func<Dictionary<string, LispNode>, Dictionary<string, LispNode>, LispNode> Func { get; private set; }
        }

        static List<PatternFunc> evalList =
            new List<PatternFunc>()
            {
                new PatternFunc("ap ap add x0 0", IPatternFunc),
                new PatternFunc("ap ap add 0 x0", IPatternFunc),
                new PatternFunc("ap ap mul x0 1", IPatternFunc),
                new PatternFunc("ap ap mul 1 x0", IPatternFunc),
                new PatternFunc("ap ap mul x0 0", ZeroPatternFunc),
                new PatternFunc("ap ap mul 0 x0", ZeroPatternFunc),
                new PatternFunc("ap ap div x0 1", IPatternFunc),
                new PatternFunc("ap inc ap dec x0", IPatternFunc),
                new PatternFunc("ap dec ap inc x0", IPatternFunc),
                new PatternFunc("ap ap add x0 1", MakeIncPatternFunc),
                new PatternFunc("ap ap add 1 x0", MakeIncPatternFunc),
                new PatternFunc("ap ap add x0 -1", MakeDecPatternFunc),
                new PatternFunc("ap ap add -1 x0", MakeDecPatternFunc),
                new PatternFunc("ap ap neg neg x0", IPatternFunc),

                new PatternFunc("ap s t", ApSTPatternFunc),
                new PatternFunc("ap inc x0", IncPatternFunc),
                new PatternFunc("ap dec x0", DecPatternFunc),
                new PatternFunc("ap ap add x0 x1", AddPatternFunc),
                new PatternFunc("ap ap mul x0 x1", MulPatternFunc),
                new PatternFunc("ap ap div x0 x1", DivPatternFunc),
                new PatternFunc("ap ap eq x0 x1", EqPatternFunc),
                new PatternFunc("ap ap lt x0 x1", LtPatternFunc),
                new PatternFunc("ap neg x0", NegPatternFunc),
                new PatternFunc("ap ap ap s x0 x1 x2", SPatternFunc),
                new PatternFunc("ap ap ap c x0 x1 x2", CPatternFunc),
                new PatternFunc("ap ap ap b x0 x1 x2", BPatternFunc),
                new PatternFunc("ap ap t x0 x1", TPatternFunc),
                new PatternFunc("ap ap f x0 x1", FPatternFunc),
                new PatternFunc("ap i x0", IPatternFunc),
                //new PatternFunc("ap car ap ap cons x0 x1", CarPatternFunc),
                //new PatternFunc("ap cdr ap ap cons x0 x1", CdrPatternFunc),
                new PatternFunc("ap isnil x0", IsNilPatternFunc),
                new PatternFunc("ap ap ap cons x0 x1 x2", ConsPatternFunc),
                new PatternFunc("ap nil x0", NilPatternFunc),

                //new PatternFunc("ap car x0", NotImplementedPatternFunc),
                //new PatternFunc("ap cdr x0", NotImplementedPatternFunc),

                new PatternFunc("ap car x0", CarNonConsPatternFunc),
                new PatternFunc("ap cdr x0", CdrNonConsPatternFunc),

                new PatternFunc("ap x0 x1", ApPatternFunc),
                //new PatternFunc("ap x0 x1", RightApPatternFunc),
                //new PatternFunc("x0", DereferencePatternFunc),
            };

        static LispNode EvaluateOne(
            LispNode root,
            Dictionary<string, LispNode> symbols)
        {
            foreach (var patternFunc in evalList)
            {
                var matches = Match(root, patternFunc.Pattern);
                if (matches != null)
                {
                    var ret = patternFunc.Func(matches, symbols);
                    if (patternFunc.Func != ApPatternFunc)
                    {
                        ;
                    }

                    if (ret != null)
                    {
                        if (patternFunc.Func != ApPatternFunc)
                        {
                            Console.WriteLine("   " + patternFunc.Func.Method.Name);
                            foreach (var item in matches)
                            {
                                //Console.WriteLine($"   {item.Key}: {item.Value}");
                            }
                            //Console.WriteLine($"   -> {ret}");
                        }

                        return ret;
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, LispNode> LeftMatch(
            LispNode root,
            LispNode pattern)
        {
            if (root.Type == LispNodeType.Open)
            {
                var ans = LeftMatch(root.Children.First(), pattern);
                if (ans != null)
                {
                    return ans;
                }
            }

            return Match(root, pattern);
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
            else if (pattern.Text.StartsWith('x'))
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

        public static Tuple<long, string> DemodulateInt(string data)
        {
            var isNeg = data.StartsWith("10");
            var width = data.Skip(2).TakeWhile(i => i == '1').Count();
            var binaryDigits = data.Substring(3 + width, width * 4);
            var remainder = data.Substring(3 + width * 5);
            var ans = Convert.ToInt64(binaryDigits, 2);
            return Tuple.Create(isNeg ? -ans : ans, remainder);
        }


        public static string Modulate(LispNode data)
        {
            if (data.Type == LispNodeType.Token)
            {
                if (data.Text == "nil")
                {
                    return "00";
                }
                else
                {
                    return Modulate(long.Parse(data.Text));
                }
            }
            else
            {
                return "11" + Modulate(data.Children.First().Children.Last()) + Modulate(data.Children.Last());
            }
        }

        public static Tuple<LispNode, string> Demodulate(string s)
        {
            if (s.StartsWith("00"))
            {
                return Tuple.Create(
                    new LispNode() { Type = LispNodeType.Token, Text = "nil" },
                    s.Substring(2));
            }
            else if (s.StartsWith("11"))
            {
                var first = Demodulate(s.Substring(2));
                var second = Demodulate(first.Item2);
                var ans = new LispNode() { Type = LispNodeType.Open };
                ans.Children.Add(new LispNode() { Type = LispNodeType.Open });
                ans.Children.First().Children.Add(new LispNode() { Type = LispNodeType.Token, Text = "cons" });
                ans.Children.First().Children.Add(first.Item1);
                ans.Children.Add(second.Item1);
                return Tuple.Create(ans, second.Item2);
            }
            else
            {
                var value = DemodulateInt(s);
                var ans = new LispNode() { Type = LispNodeType.Token, Text = value.Item1.ToString() };
                return Tuple.Create(ans, value.Item2);
            }
        }
    }
}
