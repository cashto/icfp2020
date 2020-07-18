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
            var protocolResultRoot = new LispNode();
            protocolResultRoot.Children.Add(new LispNode());
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
            var modifiedAnything = true;

            while (modifiedAnything)
            {
                if (debug)
                {
                    //Console.WriteLine("--------");
                    //Console.WriteLine(root);
                    //Console.WriteLine("--------");

                    Console.Write(".");
                }

                modifiedAnything = false;
                foreach (var patternFunc in evalList)
                {
                    if (EvaluateOne(root, patternFunc, symbols))
                    {
                        modifiedAnything = true;
                        break;
                    }
                }
            }

            return root;
        }

        public static LispNode Parse(string s)
        {
            var stack = new List<LispNode>();
            foreach (var token in s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()))
            {
                if (token == "ap")
                {
                    stack.Add(new LispNode());
                }
                else
                {
                    stack.Add(new LispNode(token));

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
            var arg1Tree = args["x0"];
            var arg2Tree = arity < 2 ? null : args["x1"];

            long? arg1 = TryParseInt(arg1Tree);
            long? arg2 = arg2Tree == null ? 0 : TryParseInt(arg2Tree);

            if (arg1.HasValue && arg2.HasValue)
            {
                return new LispNode(fn(arg1.Value, arg2.Value));
            }

            return null;
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
            if (object.ReferenceEquals(
                Dereference(args["x0"], symbols) ?? args["x0"],
                Dereference(args["x1"],symbols) ?? args["x1"]))
            {
                return new LispNode("t");
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
            var newRoot = new LispNode();
            newRoot.Children.Add(new LispNode());
            newRoot.Children.Add(new LispNode());
            newRoot.Children.First().Children.Add(args["x0"]);
            newRoot.Children.First().Children.Add(args["x2"]);
            newRoot.Children.Last().Children.Add(args["x1"]);
            newRoot.Children.Last().Children.Add(args["x2"]);
            return newRoot; 
        }

        static LispNode CPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            // C : ap ap ap c x0 x1 x2   =   ap ap x0 x2 x1
            var newRoot = new LispNode();
            newRoot.Children.Add(new LispNode());
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
            var newRoot = new LispNode();
            newRoot.Children.Add(args["x0"]);
            newRoot.Children.Add(new LispNode());
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

        static LispNode X0PatternFunc(
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
            var ans = new LispNode();
            ans.Children.Add(args["x0"]);
            ans.Children.Add(new LispNode("t"));
            return ans;
        }

        static LispNode CdrNonConsPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode();
            ans.Children.Add(args["x0"]);
            ans.Children.Add(new LispNode("f"));
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
            return new LispNode("0");
        }

        static LispNode Dereference(
            LispNode arg,
            Dictionary<string, LispNode> symbols)
        {
            if (arg.Type != LispNodeType.Token)
            {
                return null;
            }

            if (!symbols.ContainsKey(arg.Text))
            {
                return null;
            }

            return symbols[arg.Text];
        }

        static LispNode DereferencePatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var arg0 = Dereference(args["x0"], symbols);
            var arg1 = Dereference(args["x1"], symbols);
            if (arg0 == null && arg1 == null)
            {
                return null;
            }

            var ans = new LispNode();
            ans.Children.Add(arg0 ?? args["x0"]);
            ans.Children.Add(arg1 ?? args["x1"]);
            return ans;
        }

        static LispNode ConsPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var root = new LispNode();
            root.Children.Add(new LispNode());
            root.Children.First().Children.Add(args["x2"]);
            root.Children.First().Children.Add(args["x0"]);
            root.Children.Add(args["x1"]);
            return root;
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
            var ans = new LispNode();
            ans.Children.Add(new LispNode("inc"));
            ans.Children.Add(args["x0"]);
            return ans;
        }

        static LispNode MakeDecPatternFunc(
            Dictionary<string, LispNode> args,
            Dictionary<string, LispNode> symbols)
        {
            var ans = new LispNode();
            ans.Children.Add(new LispNode("dec"));
            ans.Children.Add(args["x0"]);
            return ans;
        }

        static Func<Dictionary<string, LispNode>, Dictionary<string, LispNode>, LispNode> ConstantPatternFunc(string constant)
        {
            return (args, symbols) => new LispNode(constant);
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
                new PatternFunc("ap ap add x0 0", X0PatternFunc),
                new PatternFunc("ap ap add 0 x0", X0PatternFunc),
                new PatternFunc("ap ap mul x0 1", X0PatternFunc),
                new PatternFunc("ap ap mul 1 x0", X0PatternFunc),
                new PatternFunc("ap ap mul x0 0", ConstantPatternFunc("0")),
                new PatternFunc("ap ap mul 0 x0", ConstantPatternFunc("0")),
                new PatternFunc("ap ap div x0 1", X0PatternFunc),
                new PatternFunc("ap inc ap dec x0", X0PatternFunc),
                new PatternFunc("ap dec ap inc x0", X0PatternFunc),
                new PatternFunc("ap ap add x0 1", MakeIncPatternFunc),
                new PatternFunc("ap ap add 1 x0", MakeIncPatternFunc),
                new PatternFunc("ap ap add x0 -1", MakeDecPatternFunc),
                new PatternFunc("ap ap add -1 x0", MakeDecPatternFunc),
                new PatternFunc("ap ap neg neg x0", X0PatternFunc),

                new PatternFunc("ap s t", ConstantPatternFunc("f")),
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
                new PatternFunc("ap ap t x0 x1", X0PatternFunc),
                new PatternFunc("ap ap f x1 x0", X0PatternFunc),
                new PatternFunc("ap i x0", X0PatternFunc),
                //new PatternFunc("ap car ap ap cons x0 x1", CarPatternFunc),
                //new PatternFunc("ap cdr ap ap cons x0 x1", CdrPatternFunc),
                new PatternFunc("ap isnil nil", ConstantPatternFunc("t")),
                new PatternFunc("ap isnil ap ap cons x0 x1", ConstantPatternFunc("f")),
                new PatternFunc("ap ap ap cons x0 x1 x2", ConsPatternFunc),
                new PatternFunc("ap nil x0", ConstantPatternFunc("t")),

                //new PatternFunc("ap car x0", NotImplementedPatternFunc),
                //new PatternFunc("ap cdr x0", NotImplementedPatternFunc),

                new PatternFunc("ap car x0", CarNonConsPatternFunc),
                new PatternFunc("ap cdr x0", CdrNonConsPatternFunc),

                new PatternFunc("ap x0 x1", DereferencePatternFunc),
            };

        static bool EvaluateOne(
            LispNode root,
            PatternFunc patternFunc,
            Dictionary<string, LispNode> symbols)
        {
            var match = Match(root, patternFunc, symbols);
            if (match != null)
            {
                root.Type = match.Type;
                root.Text = match.Text;
                root.Children.Clear();
                root.Children.AddRange(match.Children);
                return true;
            }
            else if (root.Type == LispNodeType.Open)
            {
                return
                    EvaluateOne(root.Children.First(), patternFunc, symbols) ||
                    EvaluateOne(root.Children.Last(), patternFunc, symbols);
            }

            return false;
        }

        static LispNode Match(
            LispNode root,
            PatternFunc patternFunc,
            Dictionary<string, LispNode> symbols)
        {
            var matches = Match(root, patternFunc.Pattern);
            if (matches != null)
            {
                var ret = patternFunc.Func(matches, symbols);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public static Dictionary<string, LispNode> Match(
            LispNode root,
            LispNode pattern)
        {
            var ans = new List<Tuple<string, LispNode>>();
            return Match(root, pattern, ans) ? ans.ToDictionary(i => i.Item1, i => i.Item2) : null;
        }

        public static bool Match(
            LispNode root,
            LispNode pattern,
            List<Tuple<string, LispNode>> ans)
        {
            if (pattern.Type == LispNodeType.Open)
            {
                if (root.Type != LispNodeType.Open)
                {
                    return false;
                }

                var left = Match(root.Children.First(), pattern.Children.First(), ans);
                if (!left)
                {
                    return false;
                }

                var right = Match(root.Children.Last(), pattern.Children.Last(), ans);
                if (!right)
                {
                    return false;
                }

                return true;
            }
            else if (pattern.Text.StartsWith('x'))
            {
                ans.Add(Tuple.Create(pattern.Text, root));
                return true;
            }
            else
            {
                return root.Text == pattern.Text;
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
                    new LispNode("nil"),
                    s.Substring(2));
            }
            else if (s.StartsWith("11"))
            {
                var first = Demodulate(s.Substring(2));
                var second = Demodulate(first.Item2);
                var ans = new LispNode();
                ans.Children.Add(new LispNode());
                ans.Children.First().Children.Add(new LispNode("cons"));
                ans.Children.First().Children.Add(first.Item1);
                ans.Children.Add(second.Item1);
                return Tuple.Create(ans, second.Item2);
            }
            else
            {
                var value = DemodulateInt(s);
                var ans = new LispNode(value.Item1.ToString());
                return Tuple.Create(ans, value.Item2);
            }
        }

        public static string TreeToList(LispNode tree)
        {
            if (tree.Type == LispNodeType.Token)
            {
                return tree.Text == "nil" ? "" : tree.Text;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            var first = true;

            while (tree.Type == LispNodeType.Open)
            {
                if (!first)
                {
                    sb.Append(' ');
                }

                sb.Append(TreeToList(tree.Children.First().Children.Last()));
                first = false;
                tree = tree.Children.Last();
            }
            
            sb.Append("]");
            return sb.ToString();
        }
    }
}
