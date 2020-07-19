using IcfpUtils;
using Owin;
using Newtonsoft.Json;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace Server
{
    class Server
    {
        public static Dictionary<string, LispNode> Symbols = new Dictionary<string, LispNode>();
        
        static void Main(string[] args)
        {
            var lines = File.ReadLines(@"galaxy.txt");
            foreach (var line in lines)
            {
                var x = line.Split('=');
                var symbol = x[0].Trim();
                var value = Common.Parse(x[1]);

                Symbols[symbol] = value;
            }

            string baseAddress = "http://localhost:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Press any key to continue or any other key to detonate");
                Console.ReadLine();
            }
        }

        public static LispNode Interact(
            LispNode protocol,
            LispNode state,
            string x,
            string y,
            Dictionary<string, LispNode> symbols)
        {
            var vector =
                new LispNode(
                    new LispNode(
                        new LispNode("cons"),
                        new LispNode(x)),
                    new LispNode(y));

            while (true)
            {
                var protocolResultRoot = new LispNode();
                protocolResultRoot.Children.Add(new LispNode());
                protocolResultRoot.Children.First().Children.Add(protocol);
                protocolResultRoot.Children.First().Children.Add(state);
                protocolResultRoot.Children.Add(vector);
                var result = Evaluate(protocolResultRoot, symbols);

                if (result.Children.First().Children.Last().Text == "0")
                {
                    return result;
                }

                state = result.Children.Last().Children.First().Children.Last();
                var data = result.Children.Last().Children.Last().Children.First().Children.Last();
                vector = Common.Send(data);
            }
        }

        public static LispNode JsonToLispNode(JToken o)
        {
            if (o.Type == JTokenType.String)
            {
                return new LispNode(o.ToObject<string>());
            }
            else
            {
                return new LispNode(JsonToLispNode(o.First), JsonToLispNode(o.Last));
            }
        }

        public static string TreeToJson(LispNode node, int indent = 0)
        {
            var sb = new StringBuilder();
            TreeToJsonInternal(sb, node, 0, indent);
            return sb.ToString();
        }

        static void TreeToJsonInternal(
            StringBuilder sb,
            LispNode node,
            int depth,
            int indent)
        {
            if (node.Type == LispNodeType.Token)
            {
                sb.Append('"');
                sb.Append(node.Text);
                sb.Append('"');
            }
            else
            {
                if (indent > 0)
                {
                    sb.Append(Environment.NewLine);
                }

                foreach (var i in Enumerable.Range(0, indent * depth))
                {
                    sb.Append(' ');
                }

                sb.Append('[');

                var firstChild = true;
                foreach (var child in node.Children)
                {
                    if (!firstChild)
                    {
                        sb.Append(", ");
                    }

                    TreeToJsonInternal(sb, child, depth + 1, indent);

                    firstChild = false;
                }

                sb.Append(']');
            }
        }

        public static LispNode Evaluate(
            LispNode expr,
            Dictionary<string, LispNode> symbols)
        {
            if (expr.Evaluated != null)
            {
                return expr.Evaluated;
            }

            LispNode initialExpr = expr;
            while (true)
            {
                var result = TryEval(expr, symbols);

                if (result == expr)
                {
                    initialExpr.Evaluated = result;
                    return result;
                }

                expr = result;
            }
        }

        static readonly LispNode t = new LispNode("t");
        static readonly LispNode f = new LispNode("f");
        static readonly LispNode cons = new LispNode("cons");
        static readonly LispNode nil = new LispNode("nil");

        static LispNode TryEval(
            LispNode expr,
            Dictionary<string, LispNode> symbols)
        {
            if (expr.Evaluated != null)
            {
                return expr.Evaluated;
            }

            if (expr.Type == LispNodeType.Token && symbols.ContainsKey(expr.Text))
            {
                return symbols[expr.Text];
            }

            if (expr.Type == LispNodeType.Open)
            {
                var fun = Evaluate(expr.Children.First(), symbols);
                var x = expr.Children.Last();

                if (fun.Type == LispNodeType.Token)
                {
                    if (fun.Text == "neg") return new LispNode(-AsNum(Evaluate(x, symbols)));
                    if (fun.Text == "i") return x;
                    if (fun.Text == "nil") return t;
                    if (fun.Text == "isnil") return new LispNode(x, new LispNode(t, new LispNode(t, f)));
                    if (fun.Text == "car") return new LispNode(x, t);
                    if (fun.Text == "cdr") return new LispNode(x, f);
                    if (fun.Text == "inc") return new LispNode(AsNum(Evaluate(x, symbols)) + 1);
                    if (fun.Text == "dec") return new LispNode(AsNum(Evaluate(x, symbols)) - 1);
                }

                if (fun.Type == LispNodeType.Open)
                {
                    var fun2 = Evaluate(fun.Children.First(), symbols);
                    var y = fun.Children.Last();

                    if (fun2.Type == LispNodeType.Token)
                    {
                        if (fun2.Text == "t") return y;
                        if (fun2.Text == "f") return x;
                        if (fun2.Text == "add") return new LispNode(AsNum(Evaluate(x, symbols)) + AsNum(Evaluate(y, symbols)));
                        if (fun2.Text == "mul") return new LispNode(AsNum(Evaluate(x, symbols)) * AsNum(Evaluate(y, symbols)));
                        if (fun2.Text == "div") return new LispNode(AsNum(Evaluate(y, symbols)) / AsNum(Evaluate(x, symbols)));
                        if (fun2.Text == "lt") return AsNum(Evaluate(y, symbols)) < AsNum(Evaluate(x, symbols)) ? t : f;
                        if (fun2.Text == "eq") return AsNum(Evaluate(x, symbols)) == AsNum(Evaluate(y, symbols)) ? t : f;
                        if (fun2.Text == "cons") return EvalCons(y, x, symbols);
                    }

                    if (fun2.Type == LispNodeType.Open)
                    {
                        var fun3 = Evaluate(fun2.Children.First(), symbols);
                        var z = fun2.Children.Last();

                        if (fun3.Type == LispNodeType.Token)
                        {
                            if (fun3.Text == "s") return new LispNode(new LispNode(z, x), new LispNode(y, x));
                            if (fun3.Text == "c") return new LispNode(new LispNode(z, x), y);
                            if (fun3.Text == "b") return new LispNode(z, new LispNode(y, x));
                            if (fun3.Text == "cons") return new LispNode(new LispNode(x, z), y);
                        }
                    }
                }
            }

            return expr;
        }

        static LispNode EvalCons(LispNode a, LispNode b, Dictionary<string, LispNode> symbols)
        {
            var res = new LispNode(new LispNode(cons, Evaluate(a, symbols)), Evaluate(b, symbols));
            res.Evaluated = res;
            return res;
        }

        static BigInteger AsNum(LispNode n)
        {
            if (n.Type == LispNodeType.Token)
            {
                return BigInteger.Parse(n.Text);
            }

            throw new Exception("not a number");
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}"
                // defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            const string rootFolder = @"C:\Users\cashto\Documents\GitHub\icfp2020\webroot";

            appBuilder.UseFileServer(
                new FileServerOptions
                {
                    EnableDefaultFiles = true,
                    FileSystem = new PhysicalFileSystem(rootFolder)
                });
        }
    }

    public class GalaxyPostBody
    {
        public JToken state { get; set; }
        public string x { get; set; }
        public string y { get; set; }
    }

    public class GalaxyController : ApiController
    {
        // POST api/galaxy 
        public JToken Post([FromBody] GalaxyPostBody body)
        {
            return
                JsonConvert.DeserializeObject<JToken>(
                    Server.TreeToJson(
                        Server.Interact(
                            Server.Symbols["galaxy"],
                            Server.JsonToLispNode(body.state),
                            body.x,
                            body.y,
                            Server.Symbols)));
        }
    }
}
