using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IcfpUtils;

namespace Solver
{
    static class Program2
    {
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

        static long AsNum(LispNode n)
        {
            if (n.Type == LispNodeType.Token)
            {
                return long.Parse(n.Text);
            }

            throw new Exception("not a number");
        }
    }
}
