using System;
using System.Numerics;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace IcfpUtils
{
    public enum LispNodeType
    {
        Token,
        Open,
        Close
    }

    public class LispNode
    {
        public LispNode(string s)
        {
            Type = LispNodeType.Token;
            Text = s;
        }

        public LispNode(BigInteger number) : this(number.ToString()) { }

        public LispNode()
        {
            Type = LispNodeType.Open;
        }

        public LispNode(LispNode lhs, LispNode rhs)
        {
            Type = LispNodeType.Open;
            this.Children.Add(lhs);
            this.Children.Add(rhs);
        }

        public LispNode Evaluated { get; set; }
        public LispNodeType Type { get; set; }
        public string Text { get; set; }
        public List<LispNode> Children { get; } = new List<LispNode>();
        public int Line { get; set; }
        public int Column { get; set; }
        public string LineText { get; set; }

        //public override string ToString() =>
        //    Type == LispNodeType.Token ?
        //        $"Token = '{Text}' ({Line}:{Column})" :
        //        $"Children = {Children.Count} ({Line}:{Column})";

        public override string ToString()
        {
            return ToString(indent: 0);
        }

        public string ToString(int indent)
        {
            var sb = new StringBuilder();
            ToStringInternal(sb, 0, indent);
            return sb.ToString();
        }

        private void ToStringInternal(
            StringBuilder sb,
            int depth,
            int indent)
        {
            if (Type == LispNodeType.Token)
            {
                sb.Append(Text);
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

                sb.Append('(');

                var firstChild = true;
                foreach (var child in Children)
                {
                    if (!firstChild)
                    {
                        sb.Append(' ');
                    }

                    child.ToStringInternal(sb, depth + 1, indent);

                    firstChild = false;
                }

                sb.Append(')');
            }
        }
    }

    public static class Lisp
    {
        static MemoryStream GetStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }

        public static LispNode Parse(string s)
        {
            using (var stream = GetStream(s))
            {
                return Parse(stream);
            }
        }

        public static LispNode Parse(Stream stream)
        {
            var stack = new Stack<LispNode>();
            stack.Push(new LispNode() { Type = LispNodeType.Open });

            foreach (var token in Tokenize(stream))
            {
                switch (token.Type)
                {
                    case LispNodeType.Open:
                        stack.Push(token);
                        break;

                    case LispNodeType.Close:
                        var item = stack.Pop();
                        if (!stack.Any())
                        {
                            throw new Exception($"Mismatched ) at line {token.Line} column {token.Column}");
                        }

                        stack.Peek().Children.Add(item);
                        break;

                    case LispNodeType.Token:
                        stack.Peek().Children.Add(token);
                        break;
                }
            }

            var ans = stack.Pop();
            if (stack.Any())
            {
                throw new Exception($"Mismatched ( at line {ans.Line} column {ans.Column}");
            }

            return ans;
        }

        static IEnumerable<string> ReadLines(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        static IEnumerable<LispNode> Tokenize(Stream stream)
        {
            var line = 1;
            foreach (var lineText in ReadLines(stream))
            {
                var escape = false;
                char? quoteChar = null;
                var inComment = false;
                var column = 1;
                var tokenStartColumn = column;
                var token = new StringBuilder();

                foreach (var ch in lineText)
                {
                    if (quoteChar != null)
                    {
                        if (escape)
                        {
                            token.Append(ch);
                            escape = false;
                        }
                        else if (ch == '\\')
                        {
                            escape = true;
                        }
                        else if (ch == quoteChar.Value)
                        {
                            quoteChar = null;

                            yield return new LispNode()
                            {
                                Type = LispNodeType.Token,
                                Line = line,
                                Column = tokenStartColumn,
                                LineText = lineText,
                                Text = token.ToString()
                            };

                            token = new StringBuilder();
                        }
                        else
                        {
                            token.Append(ch);
                        }
                    }
                    else if (!inComment)
                    {
                        switch (ch)
                        {
                            case '\'':
                            case '\"':
                                quoteChar = ch;
                                tokenStartColumn = column;
                                break;

                            case '#':
                                inComment = true;
                                break;

                            case '(':
                                yield return new LispNode()
                                {
                                    Type = LispNodeType.Open,
                                    Line = line,
                                    Column = column,
                                    LineText = lineText,
                                };

                                tokenStartColumn = column + 1;
                                break;

                            case ' ':
                            case '\t':
                            case ')':
                                if (token.Length > 0)
                                {
                                    yield return new LispNode()
                                    {
                                        Type = LispNodeType.Token,
                                        Line = line,
                                        Column = tokenStartColumn,
                                        LineText = lineText,
                                        Text = token.ToString()
                                    };

                                    token = new StringBuilder();
                                }

                                tokenStartColumn = column + 1;

                                if (ch == ')')
                                {
                                    yield return new LispNode()
                                    {
                                        Type = LispNodeType.Close,
                                        Line = line,
                                        Column = column,
                                        LineText = lineText,
                                    };
                                }
                                break;

                            default:
                                token.Append(ch);
                                break;
                        }
                    }

                    ++column;
                }

                if (token.Length > 0)
                {
                    yield return new LispNode()
                    {
                        Type = LispNodeType.Token,
                        Line = line,
                        Column = tokenStartColumn,
                        LineText = lineText,
                        Text = token.ToString()
                    };
                }

                ++line;
            }
        }
    }
}