using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using IcfpUtils;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Net.WebSockets;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Numerics;

namespace IcfpUtils
{
    public static class Common
    {
        static readonly char[] SpaceDelim = new char[] { ' ' };
        public static LispNode Parse(string s)
        {
            var stack = new List<LispNode>();
            foreach (var token in s.Split(SpaceDelim, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()))
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

        public static LispNode Send(LispNode content)
        {
            try
            {
                var apiKey = "f6bc77050a4b4d2995b3f51b50155cdd";

                var httpClient = new HttpClient();
                var result = httpClient.SendAsync(
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        $"https://icfpc2020-api.testkontur.ru/aliens/send?apiKey={apiKey}")
                    {
                        Content = new StringContent(Modulate(content))
                    }).Result;

                var response = Demodulate(result.Content.ReadAsStringAsync().Result).Item1;
                Console.WriteLine($"  -> got {response}");
                Console.WriteLine($"Sent [{Common.Flatten(content)}], received [{Common.Flatten(response)}]");

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Sent [content], received {e}");
                throw;
            }
        }

        public static string Modulate(BigInteger data)
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

            var binaryDigits = ToBinaryString(data);
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

        public static Tuple<BigInteger, string> DemodulateInt(string data)
        {
            var isNeg = data.StartsWith("10");
            var width = data.Skip(2).TakeWhile(i => i == '1').Count();
            var binaryDigits = data.Substring(3 + width, width * 4);
            var remainder = data.Substring(3 + width * 5);
            var ans = FromBinaryString(binaryDigits);
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
                    return Modulate(BigInteger.Parse(data.Text));
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

        static BigInteger FromBinaryString(string s)
        {
            var ans = new BigInteger(0);
            foreach (var c in s)
            {
                ans = ans * 2 + (c == '1' ? 1 : 0);
            }

            return ans;
        }

        static string ToBinaryString(BigInteger x)
        {
            if (x == 0)
            {
                return "0";
            }

            var sb = new List<char>();
            while (x != 0)
            {
                sb.Add(x % 2 != 0 ? '1' : '0');
                x = x / 2;
            }

            return new string(sb.Reverse<char>().ToArray());
        }

        public static LispNode Flatten(LispNode node)
        {
            if (node.Type == LispNodeType.Token)
            {
                return node.Text == "nil" ? new LispNode() : node;
            }

            var ans = new LispNode();
            while (node.Type != LispNodeType.Token)
            {
                ans.Add(Flatten(node.First().Last()));
                node = node.Last();
            }

            return ans;
        }

        public static LispNode Unflatten(LispNode node)
        {
            if (node.Type == LispNodeType.Token)
            {
                return node;
            }

            var ans = new LispNode("nil");
            foreach (var child in node.Reverse())
            {
                ans =
                    new LispNode() {
                        new LispNode() {
                            new LispNode("cons"), Unflatten(child) },
                        ans };
            }

            return ans;
        }
    }
}
